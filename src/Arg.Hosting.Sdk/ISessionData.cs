﻿using System;
using System.Collections.Generic;

namespace Arg.Hosting.Sdk
{
    public interface ISessionData
    {
        DateTime ConnectedTime { get; }
        Guid SessionId { get; }
        bool IsConnected { get; }
    }
}