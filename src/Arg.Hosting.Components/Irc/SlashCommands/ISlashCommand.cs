using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Hosting.Components.Irc
{
    public interface ISlashCommand
    {
        byte[] Identifier { get; }
        void Execute(ref byte[] bytes, ServerState state, ISessionData session, ISocketClient client);
    }
}
