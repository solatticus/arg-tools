using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Arg.Hosting.Components.Irc
{
    public class ServerState
    {
        public ConcurrentBag<ChatRoom> Rooms { get; } = new ConcurrentBag<ChatRoom>();

        public ServerState()
        {
            Rooms.Add(new ChatRoom("Lobby"));
            Rooms.Add(new ChatRoom("Red Room"));
            Rooms.Add(new ChatRoom("Basement"));
            Rooms.Add(new ChatRoom("Banter"));
        }
    }
}
