using Arg.Hosting.Sdk;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Arg.Hosting
{
    public class SocketServer : IDisposable
    {
        private const int MaxIncoming = 64;

        private delegate Task ReceiveOnSocketHandler(ISessionData session, ISocketClient client, ISocketMessage message);
        private delegate Task WriteFromSocketHandler(ISessionData session, ISocketClient client, ISocketMessage message);

        private static ReceiveOnSocketHandler _inDelegates = null;
        private static WriteFromSocketHandler _outDelegates = null;

        private readonly IComponentCollection _components;

        private readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();
        private readonly Socket _listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public static ConcurrentDictionary<SocketClient, SessionData> ClientSessions { get; } = new ConcurrentDictionary<SocketClient, SessionData>();

        public IPAddress ListenAddress { get; private set; }
        public int ListenPort { get; private set; }
        public TimeSpan Runtime { get => _stopwatch.Elapsed; }

        internal SocketServer(IPAddress address, int portNumber, IComponentCollection components)
        {
            _components = components;
            
            ListenAddress = address;
            ListenPort = portNumber;
        }

        /// <summary>
        /// Start the server listening loop asynchronousely. You're responsible for keeping the process alive.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task StartAsync()
        {
            _stopwatch.Restart();

            await Task.Run(async () =>
            {
                await ListenAndAccept(_cancelSource.Token);
            }); 
        }

        /// <summary>
        /// Start the server listening loop and block. (cheesy for now) 
        /// </summary>
        public void StartConsole()
        {
            _stopwatch.Restart();

            _ = ListenAndAccept(_cancelSource.Token);

            Console.ReadKey();
        }

        /// <summary>
        /// Listen on the configured socket and init new connections
        /// </summary>
        /// <param name="token"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task ListenAndAccept(CancellationToken token)
        {
            try
            {
                _listenSocket.Bind(new IPEndPoint(ListenAddress, ListenPort));

                _listenSocket.Listen(MaxIncoming);

                while (!token.IsCancellationRequested)
                {
                    var socket = await _listenSocket.AcceptAsync();
                    var client = new SocketClient(socket);
                    _ = StartLoop(client, token); //intentionally calling synchronously so the first await after a connection is AFTER the read/write loop starts at the .WhenAll below
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception|{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates socket reader and buffer writer tasks and waits for both of them to complete.
        /// </summary>
        /// <param name="token"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task StartLoop(SocketClient connection, CancellationToken token)
        {
            Console.WriteLine($"[{connection.Socket.RemoteEndPoint}]: connected");

            var session = new SessionData(connection);

            if(!ClientSessions.TryAdd(connection, session)) // re-connects?
                throw new Exception("Couldn't map client socket to session.");

            _inDelegates = null;
            _outDelegates = null;

            foreach(var component in _components)
            {
                await component.ClientConnects(connection); // only once

                _inDelegates += component.Receive;
                _outDelegates += component.Write;
            }

            var pipe = new Pipe();
            var writing = WriteAsync(connection, pipe.Writer, token); // get the tasks for each operation
            var reading = ReadAsync(connection, pipe.Reader, token);

            await Task.WhenAll(reading, writing); // wait for both to finish
            
            Console.WriteLine($"[{connection.Socket.RemoteEndPoint}]: disconnected");
        }

        /// <summary>
        /// Takes the currently "being read" bytes and creates a <see cref="SocketMessage"/>, calls the input delegates then the output delegates. (for now)
        /// </summary>
        /// <param name="token"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        private static void ApplyBufferToComponents(SocketClient client, byte[] buffer)
        {
            _ = Task.Run(async () => {
                var msg = new SocketMessage(buffer);

                var i = _inDelegates(ClientSessions[client], client, msg);
                var o =_outDelegates(ClientSessions[client], client, msg);

                await Task.WhenAll(i, o);
            });
        }

        private static async Task WriteAsync(SocketClient client, PipeWriter writer, CancellationToken token)
        {
            while (true && ! token.IsCancellationRequested)
            {
                try
                {
                    var memory = writer.GetMemory(512); // Request a minimum of 512 bytes (Memory<byte>) from the PipeWriter

                    int bytesRead = await client.Socket.ReceiveAsync(memory, SocketFlags.None);
                    if (bytesRead == 0)
                        break;

                    writer.Advance(bytesRead); // Tell the PipeWriter how much was read
                }
                catch (Exception e) // analyzers hate this... too generic of a catch clause
                {
                    Debug.WriteLine($"WriteAsync|{client.Socket.RemoteEndPoint}|{e.Message}");
                    break;
                }

                var result = await writer.FlushAsync(); // Make the data available to the PipeReader and check the FlushResult for completion

                if (result.IsCompleted)
                    break;
            }

            writer.Complete();
        }

        private static async Task ReadAsync(SocketClient socket, PipeReader reader, CancellationToken token)
        {
            while (true && !token.IsCancellationRequested)
            {
                var result = await reader.ReadAsync();

                var buffer = result.Buffer; // ReadOnlySequence<byte>
                SequencePosition? position;
                do
                {
                    position = buffer.PositionOf((byte)'\n'); // Check for a '\n' character

                    if (position == null)
                        continue;

                    var chunk = buffer.Slice(0, position.Value);

                    ApplyBufferToComponents(socket, chunk.ToArray());

                    var next = buffer.GetPosition(1, position.Value); // This is equivalent to position + 1
                    buffer = buffer.Slice(next); // Skip what we've already processed including \n
                }
                while (position != null);

                // We sliced the buffer until no more data could be processed
                // Tell the PipeReader how much we consumed and how much we left to process
                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                    break;
            }

            reader.Complete();
        }


        #region SocketServerBuilder

        public sealed class SocketServerBuilder : ISocketServerBuilder
        {
            private int _portNumber = 1337;
            private IPAddress _ip = IPAddress.Any;
            private readonly ServerCompomentCollection _components; // has all the actions
            
            private SocketServerBuilder()
            {
                _components = new ServerCompomentCollection();
            }

            public static SocketServerBuilder Create()
                => new SocketServerBuilder();

            public SocketServer Build() 
                => new SocketServer(_ip, _portNumber, _components);

            public ISocketServerBuilder ListensOn(IPAddress address)
            {
                _ip = address;
                return this;
            }

            public ISocketServerBuilder UsingPort(int portNumber)
            {
                _portNumber = portNumber;
                return this;
            }

            public ISocketServerBuilder WithComponent(IServerMod component)
            {
                _components.Add(component);
                return this;
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cancelSource.Cancel();
                    _cancelSource.Dispose();
                }

                _listenSocket.Close();
                _listenSocket.Dispose();

                ClientSessions.Clear();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true); // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            
            GC.SuppressFinalize(this);
        }

        ~SocketServer()
        {
            Dispose(false); // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        }
        #endregion
    }
}
