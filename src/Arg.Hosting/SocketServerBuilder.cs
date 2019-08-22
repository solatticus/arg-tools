using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Arg.Hosting
{
    public sealed class SocketServerBuilder : ISocketServerBuilder
    {
        private int _portNumber = 1337;
        private IPAddress _ip = IPAddress.Any;
        private readonly IComponentCollection _components; // has all the actions

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
}
