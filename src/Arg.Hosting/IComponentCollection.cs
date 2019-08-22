using Arg.Hosting.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arg.Hosting
{
    public interface IComponentCollection : IList<IServerMod>
    {
        Task Execute();
    }
}