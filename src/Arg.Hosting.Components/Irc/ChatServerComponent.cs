using Arg.Hosting.Components.Irc;
using Arg.Hosting.Components.Irc.SlashCommands;
using Arg.Hosting.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Arg.Hosting.Components
{
    public class ChatServerComponent : ServerComponent
    {
        private const string NL = "\r\n";
        private readonly string _welcomeMessage = "Chat v0.1.2" + NL + "Enter an alias>";
        private readonly ConcurrentDictionary<ISocketClient, ChatState> _clientStates = new ConcurrentDictionary<ISocketClient, ChatState>();
        private readonly ServerState _serverState = new ServerState();
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

            user.Alias = Encoding.ASCII.GetString(msg);
            user.Commands.Add(new JoinCommand()); //TODO: Populate commands based on access? Dunno
            user.Commands.Add(new ListRoomsCommand());

            client.Send(_($"Welcome {user.Alias}\n>"));

            user.Flags = ChatStates.Connected | ChatStates.Authenticated;

            return true;
        }

        private bool ProcessMessage(ISessionData session, ISocketClient client, ISocketMessage message)
        {
            var user = _clientStates.Values.Where(c => c.ConnectionId == client.ConnectionId).SingleOrDefault();

            byte[] bytes = message.GetRawBytes();

            string result = (bytes[0]) switch
            {
                (byte)'/' => ProcessSlashCommand(user, session, client, ref bytes),
                (byte)'?' => GetHelpText(),
                _ => "Bad command or file name." // :)
            };

            client.Send(Encoding.ASCII.GetBytes(result + "\n"));

            return true;
        }

        private string GetHelpText() 
            => "\n/join <room>\tJoins a room.\n/list       \tLists available rooms.\n>";

        private string ProcessSlashCommand(ChatState userState, ISessionData session, ISocketClient client, ref byte[] bytes)
        {
            byte[] tmpBytes;
            ISlashCommand command = null;
            foreach(var cmd in userState.Commands)
            {
                tmpBytes = cmd.Identifier;
                if (bytes.IndexOfSequence(ref tmpBytes) > -1)
                    command = cmd;
            }

            if (command != null)
            {
                var content = bytes.Skip(command.Identifier.Length)
                                    .Take(bytes.Length - command.Identifier.Length)
                                    .ToArray();

                command.Execute(ref content, _serverState, session, client);
                return string.Empty;
            }

            return "Invalid slash command.";
        }

        private static byte[] _(string toEncode)
            => Encoding.UTF8.GetBytes(toEncode);
    }
}