# Alternate Reality Game Tools
A growing set of tools to run or host an ARG. 

* Written in C# 8 using [DotNet Core 3.0 Preview 9](https://github.com/dotnet/corefx)
  * As of this writing, DotNet Core 3.0 releases late September 2019. (Soon!)
 
##### Current Tools

* TCP Server implementation using the new DotNet Core System.IO.Pipelines code. This was inspired by a blog post from [@davidfowl](http://twitter.com/davidfowl).
  * Pluggable components to extend the base TCP connection, sending & receiving functionality. There are a couple tests right now:
    * TCP Echo Component
    * IRC Component
    * ASCII Movie Streamer Component (`this is very important`)
  * Most of the TELNET IAC commands and options are defined for use.
  * Fluent builder syntax inspired by DotNet Core's HostBuilder
  
##### Ascii Movie Streamer via Telnet
Here's all you need to start a server that streams an ASCII movie to each connected client. It's hacky, but it works ;)

````` csharp
        static void Main(string[] args)
        {
            var server = SocketServerBuilder.Create()
                            .ListensOn(IPAddress.Any)
                            .UsingPort(1337)
                            //.WithComponent(new BasicTelnetComponent()) // sends IAC WONT ECHO etc.
                            //.WithComponent(new ChatServerComponent()) // very basic
                            .WithComponent(new AsciiMovieServerComponent())
                            .Build();

            server.StartConsole();
        }
`````
-The .txt file movie was taken from [@nitram509](https://github.com/nitram509/ascii-telnet-server)
