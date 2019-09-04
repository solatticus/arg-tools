using Arg.Hosting.Sdk;
using System;
using System.Diagnostics;
using System.Text;

namespace Arg.Hosting
{
    [DebuggerDisplay("{value}", Name = "{Line}")]
    public class SocketMessage : ISocketMessage
    {
        protected byte[] _rawBytes;

        public byte FirstByte { get => _rawBytes[0]; }

        public string Line
        {
            get { return Encoding.UTF8.GetString(_rawBytes); }
        }
        
        public SocketMessage(ref byte[] msgBytes)
        {
            _rawBytes = msgBytes;
        }

        public ReadOnlySpan<byte> ToReadOnly(Encoding encoding) 
            => encoding.GetBytes(Line);

        public ref byte[] GetRawBytes() 
            => ref _rawBytes; // awesome?
    }
}