using System;

namespace Arg.Hosting
{
    [Flags]
    public enum ClientStates
    {
        None = 0,
        Connected = 1,
        Authenticating = 2,
        Authenticated = 4,
    }
}