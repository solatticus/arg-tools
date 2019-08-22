using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arg.Hosting.Components
{
    internal enum TelnetDeclarations
    {
        Will,
        Wont,
        Do,
        Dont,
        None
    }

    internal enum TelnetOptions : byte
    {
        Null = 0x00,
        Echo = 0x01,
        SupressGoAhead = 0x21,
        FlowControl = 0x33,

    }

    public class BasicTelnetComponent : ServerComponent
    {
        //TODO: Lookup Telnet console options flags  
                                                     /* B.Iac, B.Wont, B.Echo */
        private readonly byte[] helloBytes = new byte[] { 0xFF, 0xFA, 0x01 };  // something like this, will look up

        public BasicTelnetComponent()
        {
            Name = "Basic Telnet";
        }

        protected override void WhenClientConnects(ISocketClient client)
        {
            client.Send(helloBytes);
        }

        protected override void WhenReceiving(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            //TODO: Find what bytes thing actually are - process works until here :D

            if (message.FirstByte != B.Iac) // wrong B.Iac value, hell... could be wrong acronym even.
                return; // ignore the packet if it doesn't have any options? whatever

            var options = new List<TelnetPacketOption>();

            TelnetPacketOption option = null;

            byte b;
            byte lastByte = 0x00;

            var pos = 0;
            var bytes = message.GetRawBytes(); // so is this bytes 'by ref'?

            do
            {
                b = bytes[pos++]; // byte for the current position

                if (b == B.Iac) // if it's an identifier for IAC then make a new option
                {
                    if (lastByte == b) // if two IAC bytes (255, 0xFF) then the next byte starts content (if I recall, will look it up)
                        break;
                    
                    option = new TelnetPacketOption();
                    options.Add(option);
                }

                option.Declaration = (bytes[pos++]) switch
                {
                    B.Will => TelnetDeclarations.Will,
                    B.Wont => TelnetDeclarations.Wont,
                    B.Do => TelnetDeclarations.Do,
                    B.Dont => TelnetDeclarations.Dont,

                    _ => TelnetDeclarations.None,
                };

                option.Value = (bytes[pos++]) switch
                {
                    (byte)TelnetOptions.Echo => TelnetOptions.Echo,
                    (byte)TelnetOptions.SupressGoAhead => TelnetOptions.SupressGoAhead,
                    (byte)TelnetOptions.FlowControl => TelnetOptions.FlowControl,
                    
                    // moar

                    _ => TelnetOptions.Null,
                };

                lastByte = b;
                
            }
            while (true);

            var input = Encoding.ASCII.GetString(bytes, pos, bytes.Length - 1);

            client.Send(Encoding.ASCII.GetBytes(input));

            //Do the actual processing for a message - lots
            //refactor this to post work units to a queue?
            
            //TODO: Process Messages and .Add to some queue
        }
    }
}
