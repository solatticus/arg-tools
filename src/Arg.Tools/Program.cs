using System;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Arg.Tools.Ascii
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var request = WebRequest.CreateHttp(args[0]);
            var picStream = request.GetResponse().GetResponseStream();
            var pic = (Bitmap)Image.FromStream(picStream);

            pic = (Bitmap)pic.Resize(100);

            Console.WriteLine(pic.ToAscii());

            Console.Read();
        }
    }
}
