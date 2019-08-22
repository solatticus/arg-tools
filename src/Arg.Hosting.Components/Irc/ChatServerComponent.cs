using Arg.Hosting.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Arg.Hosting.Components
{
    public class ChatServerComponent : ServerComponent
    {
        private readonly string _welcomeMessage = "Chat v0.1.1\n\nEnter an alias>";
        private readonly ConcurrentDictionary<ISocketClient, ChatState> _clientStates = new ConcurrentDictionary<ISocketClient, ChatState>();

        private readonly bool _allowAnonymous = false;

        public ChatServerComponent()
        {
            Name = "Chat";
        }

        protected override void WhenClientConnects(ISocketClient client)
        {
            var newUser = new ChatState(client);

            if (!_clientStates.TryAdd(client, newUser))
                return;

            if(!_allowAnonymous)
                newUser.Flags |= ChatStates.Authenticating;

            client.Send(Encoding.UTF8.GetBytes(_welcomeMessage));
        }

        protected override void WhenReceiving(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            var user = _clientStates.Values.Where(c => c.ConnectionId == client.ConnectionId).SingleOrDefault();
            
            if(user == null)
            {
                Debug.WriteLine($"{client.ConnectionId}: Invalid Connection");
                return;
            }

            var success = (user.Flags) switch // discarding to use cooler syntax
            {
                ChatStates.Connected 
                | ChatStates.Authenticated => ProcessMessage(session, client, message),

                ChatStates.Connected 
                | ChatStates.Authenticating => ProcessPendingAuth(session, client, message),

                _ => false
            };

            Debug.WriteLine($"{user.ConnectionId} {success}");
        }

        private bool ProcessPendingAuth(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            var user = _clientStates.Values.Where(c => c.ConnectionId == client.ConnectionId).SingleOrDefault();

            //TODO: Error handling in chat server component

            var msg = message.GetRawBytes();

            if (msg.Length == 0 || msg.Length > 16)
            {
                client.Send(_("Invalid alias" + user.LinePrompt));
                return false;
            }

            //TODO: Assign alias to state etc... probably things not thought of yet ;)

            return true;
        }

        private bool ProcessMessage(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            var user = _clientStates.Values.Where(c => c.ConnectionId == client.ConnectionId).SingleOrDefault();

            return true;
        }

        private static byte[] _(string toEncode)
            => Encoding.UTF8.GetBytes(toEncode);
    }
}