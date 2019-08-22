using Arg.Hosting.Sdk;
using System;
using System.Net.Sockets;
#nullable enable

namespace Arg.Hosting
{
    public class SessionData : ISessionData
    {
        private readonly SocketClient _socketClient;
        public UserData? User { get; set; } = new UserData("anonymous");

        public DateTime ConnectedTime { get; } = DateTime.UtcNow;
        public Guid SessionId { get; } = Guid.NewGuid();
        
        public bool IsConnected { get => _socketClient.State == ClientStates.Connected; }
        public SessionData(SocketClient socketClient) 
            => _socketClient = socketClient;
    }
}