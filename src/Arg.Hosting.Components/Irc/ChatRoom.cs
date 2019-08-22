using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Hosting.Components
{
    public class ChatRoom
    {
        public string Name { get; }
        public List<ChatState> Users { get; set; } = new List<ChatState>();

        public ChatRoom(string roomName)
        {
            Name = roomName;
        }
    }
}
