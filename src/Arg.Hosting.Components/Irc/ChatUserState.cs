﻿using Arg.Hosting.Sdk;
using System;
using System.Text;

namespace Arg.Hosting.Components
{
    [Flags]
    public enum ChatUserStates : byte
    {
        Disconnected = 0,
        Connected = 1,
        Identified = 2,
        Authenticating = 4,
        Authenticated = 8,
        PenaltyBox = 16,
        Banned = 32,
        Mod = 64,
        Admin = 128
    }

    public class ChatUserState
    {
        private readonly ISocketClient _client;
        private string _currentRoom = "lobby";

        public ChatUserStates Flags { get; set; }
        public string Alias { get; set; }
        public Guid ConnectionId { get => _client.ConnectionId; }
        public string Prompt { get => $"{_currentRoom}>"; set => _currentRoom = value; }
        public string LinePrompt { get => $"\n{_currentRoom}>"; set => _currentRoom = value; }

        public ChatUserState(ISocketClient client)
        {
            _client = client;

            Alias = "anonymous";
            Flags = ChatUserStates.Connected;
        }

        public void SendMessage(string message)
        {
            _client.Send(Encoding.UTF8.GetBytes(message));
        }
    }
}