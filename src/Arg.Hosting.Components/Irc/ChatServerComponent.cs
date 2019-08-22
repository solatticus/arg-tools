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
        private readonly ConcurrentDictionary<ISocketClient, ChatUserState> _clientStates = new ConcurrentDictionary<ISocketClient, ChatUserState>();

        public ChatServerComponent()
        {
            Name = "Chat";
        }

        protected override void WhenClientConnects(ISocketClient client)
        {
            if (!_clientStates.TryAdd(client, new ChatUserState(client)))
                return;

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

            var success = ((byte)user.Flags) switch // discarding to use cooler syntax
            {
                (byte)ChatUserStates.Authenticated => ProcessMessage(session, client, message),
                (byte)ChatUserStates.Authenticating => ProcessPendingAuth(session, client, message),
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