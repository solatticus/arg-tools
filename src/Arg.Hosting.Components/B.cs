using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Hosting.Components
{
    /// <summary>
    /// NVT Printer commands for telnet/xterm screen control
    /// </summary>
    public enum NvtCommands : byte
    {
        NULL = 0,
        LineFeed = 10,
        CarriageReturn = 13,
        BELL = 7,
        BackSpace = 8,
        HorizontalTab = 9,
        VerticalTab = 11,
        FormFeed = 12, // basically clearscreen
    }

    /// <summary>
    /// IAC command bytes
    /// </summary>
    public enum Iac : byte
    {
        Iac = 255,

        Will = 251,
        Wont = 252,
        Do = 253,
        Dont = 254,

        SubNegoatieBegin = 250,
        SubNegotiateEnd = 240,

        NoOperation = 241,
        DataMark = 242,
        Break = 243,
        GoAhead = 249,
        
        Echo = 1,
        SuppressGoAhead = 3,
        Status = 5,
        TimingMark = 6,
        LineMode = 34,
        LineFeedUse = 16,
        ExtendedAscii = 17,
        TerminalType = 24,
        EndOfRecord = 25,
        RemoteFlowControl = 33,
    }
    /// <summary>
    /// A class housing the byte constants for commands and option negotiations
    /// </summary>
    internal class B
    {
        public const byte Iac = 255;

        public const byte Do = 253; 
        public const byte Dont = 254;
        public const byte Will = 251;
        public const byte Wont = 252;

        public const byte Echo = 01;
        public const byte FlowControl = 33;
        public const byte SupressGoAhead = 21;
    }
}
