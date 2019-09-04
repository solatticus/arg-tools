using System;
using System.Collections.Generic;
using System.Text;
using Arg.Hosting.Sdk;

namespace Arg.Hosting.Components.Irc.SlashCommands
{
    public class ListRoomsCommand : SlashCommand
    {
        public ListRoomsCommand()
        {
            //                          /      l    i     s     t
            _identifier = new byte[] { 0x2F, 0x6C, 0x69, 0x73, 0x74 };
        }

        public override void Execute(ref byte[] contentBytes, ServerState state, ISessionData session, ISocketClient client)
        {
            var sb = new StringBuilder("\n");
            foreach (var room in state.Rooms)
                sb.AppendLine(room.Name);

            client.Send(Encoding.ASCII.GetBytes($"Rooms available:\n{sb}\n"));
        }
    }
}
