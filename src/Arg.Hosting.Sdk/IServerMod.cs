using System;
using System.Threading.Tasks;

namespace Arg.Hosting.Sdk
{
    public interface IServerMod
    {
        string Name { get; }

        Task ClientConnects(ISocketClient client);
        Task Receive(ISessionData sessino, ISocketClient client, ISocketMessage message);
        Task Write(ISessionData sessino, ISocketClient client, ISocketMessage message);
    }
}