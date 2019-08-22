using Arg.Hosting.Components;
using System;
using System.Net;
using static Arg.Hosting.SocketServer;

namespace Arg.Hosting.Demo.BasicTelnet
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = SocketServerBuilder.Create()
                            .ListensOn(IPAddress.Any)
                            .UsingPort(1337)
                            //.WithComponent(new BasicTelnetComponent()) // sends IAC WONT ECHO etc.
                            .WithComponent(new ChatServerComponent()) // very basic
                            .Build();

            server.StartConsole();
        }
    }
}
