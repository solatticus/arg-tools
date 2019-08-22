using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Arg.Hosting.Sdk
{
    public abstract class ServerComponent : IServerMod
    {
        public string Name { get; protected set; } = "component";
        public bool Enabled { get; set; } = true;

        public Task Receive(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            try
            {
                WhenReceiving(session, client, message);
            }
            catch(Exception oops)
            {
                Debug.WriteLine(ToString() + "|" + oops.Message); // whatever for now
                return Task.FromException(oops);
            }

            return Task.CompletedTask;
        }

        protected virtual void WhenReceiving(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            // untz
        }

        public Task Write(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            try
            {
                WhenWriting(session, client, message);
            }
            catch (Exception oops)
            {
                Debug.WriteLine(ToString() + "|" + oops.Message);
                return Task.FromException(oops);
            }

            return Task.CompletedTask;
        }

        protected virtual void WhenWriting(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            // untz
        }

        public Task ClientConnects(ISocketClient client)
        {
            try
            {
                WhenClientConnects(client);
            }
            catch (Exception oops)
            {
                Debug.WriteLine(ToString() + "|" + oops.Message);
                return Task.FromException(oops);
            }

            return Task.CompletedTask;
        }

        protected virtual void WhenClientConnects(ISocketClient client)
        {
            // untz
        }
    }
}
