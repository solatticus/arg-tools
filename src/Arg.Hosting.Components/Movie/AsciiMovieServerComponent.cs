using Arg.Hosting.Sdk;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arg.Hosting.Components.Movie
{
    public class AsciiMovieServerComponent : ServerComponent
    {
        private readonly AsciiMovieTelnetStreamer _streamer = new AsciiMovieTelnetStreamer();

        public AsciiMovieServerComponent()
        {
            _streamer.LoadFile("TestMovie.txt");
        }

        protected override void WhenClientConnects(ISocketClient client)
        {
            base.WhenClientConnects(client);
            
            client.Send(Encoding.ASCII.GetBytes("\u00B1[?3l")); // columns to 80
            
            Task.Run(async () => {
                await _streamer.StreamToClient(client);
            });
        }

        protected override void WhenReceiving(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            base.WhenReceiving(session, client, message);


        }

        protected override void WhenWriting(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            base.WhenWriting(session, client, message);


        }
    }
}
