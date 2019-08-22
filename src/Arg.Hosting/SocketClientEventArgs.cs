using System;

namespace Arg.Hosting
{
    public delegate void SocketClientEventHandler(object sender, SocketClientEventArgs e);
    public class SocketClientEventArgs : EventArgs
    {
        //meh
    }
}