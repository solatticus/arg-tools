using Arg.Hosting.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arg.Hosting
{
    internal class ServerCompomentCollection : List<IServerMod>, IComponentCollection
    {
        public Task Execute()
        {
            return Task.CompletedTask;
        }
    }
}