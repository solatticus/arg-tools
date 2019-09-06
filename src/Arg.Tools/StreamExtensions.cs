using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Tools
{
    public static class StreamExtensions
    {
        public static void CopyTo(this System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[4 * 1024]; //disks write 4k blocks these days
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
