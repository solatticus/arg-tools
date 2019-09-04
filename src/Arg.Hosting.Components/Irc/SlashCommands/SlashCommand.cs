using System;
using System.Collections.Generic;
using System.Text;
using Arg.Hosting.Sdk;

namespace Arg.Hosting.Components.Irc.SlashCommands
{
    public abstract class SlashCommand : ISlashCommand
    {
        protected byte[] _identifier = {};
        public virtual byte[] Identifier { get => _identifier; }

        public abstract void Execute(ref byte[] bytes, ServerState state, ISessionData session, ISocketClient client);
    }
}
