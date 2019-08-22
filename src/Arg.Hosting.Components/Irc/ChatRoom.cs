using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Hosting.Components
{
    public class ChatRoom
    {
        public string Name { get; }
        public List<ChatUserState> Users { get; set; } = new List<ChatUserState>();

        public ChatRoom(string roomName)
        {
            Name = roomName;
        }
    }
}
