using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arg.Hosting.Sdk
{
    public interface ISocketClient
    {
        public Guid ConnectionId { get; }
        void Send(byte[] bytes);
        ValueTask<int> SendAsync(byte[] bytes, CancellationToken token);
    }
}