using System;
using System.Collections.Generic;
using System.Text;
using Arg.Hosting.Sdk;

namespace Arg.Hosting.Components.Irc.SlashCommands
{
    public class JoinCommand : SlashCommand
    {
        public JoinCommand()
        {   
            //                          /      j    o     i     n
            _identifier = new byte[] { 0x2F, 0x6A, 0x6F, 0x69, 0x6E };
        }

        public override void Execute(ref byte[] contentBytes, ServerState state, ISessionData session, ISocketClient client)
        {
            client.Send(Encoding.ASCII.GetBytes($"Joined {Encoding.ASCII.GetString(contentBytes)}\n"));
        }
    }
}
