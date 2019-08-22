using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arg.Hosting
{
    public class SocketClient : ISocketClient
    {
        public Guid ConnectionId { get; }
        public ClientStates State { get; set; } = ClientStates.None;
        public Socket Socket { get; }
        public DateTime ConnectedOn { get; }
        
        public event SocketClientEventHandler Disconnected;

        public SocketClient(Socket clientSocket)
        {
            Socket = clientSocket;
            ConnectedOn = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        private void OnDisconnect()
        {
            Disconnected?.Invoke(this, new SocketClientEventArgs());
        }

        public void Send(byte[] bytes) => Socket.Send(bytes);

        public ValueTask<int> SendAsync(byte[] bytes, CancellationToken token) 
            => Socket.SendAsync(bytes, SocketFlags.None, token);
    }
}
