using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arg.Hosting.Components.Movie
{
    public class AsciiMovieTelnetStreamer
    {
        private const int FRAMEDELAY = 67; // arbitrary but works
        private const int FRAMEHEIGHT = 14; // 14 lines per frame, first byte is the number of times to repeat the frame
        private const int FRAMEWIDTH = 67;

        private const string CRLF = "\r\n";

        private FileStream _file = null;
        private StreamReader _reader = null;

        private static byte[] _clearScreen = Encoding.ASCII.GetBytes("\u001B[2J");
        private static byte[] _topLeft = Encoding.ASCII.GetBytes("\u001B[H");

        public void LoadFile(string filePath)
        {
            _file = File.OpenRead(filePath);
            _reader = new StreamReader(_file, Encoding.ASCII);
        }

        public Task StreamToClient(ISocketClient client)
        {
            using var src = new CancellationTokenSource();

            byte[] frameBytes;
            var sb = new StringBuilder();
            int line = 0;
            byte loopCount = 1;
            while (!_reader.EndOfStream)
            {
                for (var i = 0; i < FRAMEHEIGHT; i++) // read the whole next frame
                {
                    line++;
                    var lineTemp = _reader.ReadLine() + CRLF;

                    if (i == 0)
                        loopCount = Encoding.ASCII.GetBytes(lineTemp)[0];

                    sb.Append(lineTemp + CRLF); // \r\n to the terminal client
                }
                
                frameBytes = Encoding.ASCII.GetBytes(sb.ToString()); // first byte should be the loopcount
                frameBytes[0] = Encoding.ASCII.GetBytes(" ")[0]; // space instead of framecount
                
                sb.Clear();
                
                for (var z = 0; z < loopCount; z++)
                {
                    client.Send(_clearScreen);
                    client.Send(frameBytes);
                    Thread.Sleep(20);
                }
            }
            return Task.CompletedTask;
        }
    }
}
