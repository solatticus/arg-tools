using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Hosting.Components
{
    internal class B//ytes
    {
        public const byte Iac = 0xFF; 
        
        public const byte Do = 0xFA;    // all wrong for now
        public const byte Dont = 0xFC; 
        public const byte Will = 0xFD;
        public const byte Wont = 0xFE;

        public const byte Echo = 0x01;
        public const byte FlowControl = 0x33;       
        public const byte SupressGoAhead = 0x21;
    }
}
