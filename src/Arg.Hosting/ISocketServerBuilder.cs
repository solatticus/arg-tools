using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Arg.Hosting
{
    public interface ISocketServerBuilder
    {
        SocketServer Build();
        ISocketServerBuilder UsingPort(int portNumber);
        ISocketServerBuilder ListensOn(IPAddress address);
        ISocketServerBuilder WithComponent(IServerMod component);
    }
}
