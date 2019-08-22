using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Arg.Hosting.Components
{
    /*  http://www.faqs.org/rfcs/rfc854.html - Telnet spec
     * 
     *        NAME               CODE              MEANING

              SE                  240    End of subnegotiation parameters.
              NOP                 241    No operation.
              Data Mark           242    The data stream portion of a Synch.
                                         This should always be accompanied
                                         by a TCP Urgent notification.
              Break               243    NVT character BRK.
              Interrupt Process   244    The function IP.
              Abort output        245    The function AO.
              Are You There       246    The function AYT.
              Erase character     247    The function EC.
              Erase Line          248    The function EL.
              Go ahead            249    The GA signal.
              SB                  250    Indicates that what follows is
                                         subnegotiation of the indicated
                                         option.
              WILL (option code)  251    Indicates the desire to begin
                                         performing, or confirmation that
                                         you are now performing, the
                                         indicated option.
              WON'T (option code) 252    Indicates the refusal to perform,
                                         or continue performing, the
                                         indicated option.
              DO (option code)    253    Indicates the request that the
                                         other party perform, or
                                         confirmation that you are expecting
                                         the other party to perform, the
                                         indicated option.
              DON'T (option code) 254    Indicates the demand that the
                                         other party stop performing,
                                         or confirmation that you are no
                                         longer expecting the other party
                                         to perform, the indicated option.
              IAC                 255    Data Byte 255.
     * */

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
            /*  <-- INCOMING -->
             *  255 253 1    IAC DO ECHO
                255 253 31   IAC DO NAWS
                255 251 1    IAC WILL ECHO
                255 251 3    IAC WILL SUPPRESS-GO-AHEAD
             * */

            /*  <-- REPLY WITH -->
             *  255 252 1    IAC WONT ECHO
                255 252 31   IAC WONT NAWS
                255 254 1    IAC DONT ECHO
                255 254 3    IAC DONT SUPPRESS-GO-AHEAD
             * */

            // https://www.iana.org/assignments/telnet-options/telnet-options.xhtml

            
            var options = new List<TelnetPacketOption>();

            TelnetPacketOption option = null;

            byte b;
            byte lastByte = 0x00;

            var pos = 0;
            var bytes = message.GetRawBytes(); // so is this bytes 'by ref'?

            if (message.FirstByte != B.Iac) // wrong B.Iac value, hell... could be wrong acronym even.
                goto DoneWithOptions; // ignore the packet if it doesn't have any options? whatever

            do
            {
                b = bytes[pos++]; // byte for the current position

                if (b == B.Iac) // if it's an identifier for IAC then make a new option
                {
                    Debug.Write("IAC ");
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

                Debug.Write(Enum.GetName(typeof(TelnetDeclarations), option.Declaration) + " ");

                option.Value = (bytes[pos++]) switch
                {
                    (byte)TelnetOptions.Echo => TelnetOptions.Echo,
                    (byte)TelnetOptions.SupressGoAhead => TelnetOptions.SupressGoAhead,
                    (byte)TelnetOptions.FlowControl => TelnetOptions.FlowControl,
                    
                    // moar

                    _ => TelnetOptions.Null,
                };

                Debug.WriteLine(Enum.GetName(typeof(TelnetOptions), option.Value));

                lastByte = b;
                
            }
            while (true);

            DoneWithOptions:

            var input = Encoding.ASCII.GetString(bytes, pos, bytes.Length - 1);

            client.Send(Encoding.ASCII.GetBytes(input));

            //Do the actual processing for a message - lots
            //refactor this to post work units to a queue?
            
            //TODO: Process Messages and .Add to some queue
        }
    }
}
