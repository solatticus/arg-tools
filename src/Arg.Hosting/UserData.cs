using System;

namespace Arg.Hosting
{
    [Serializable]
    public class UserData
    {
        public int Id { get; } = -1;
        public string Username { get; }

        public UserData(string username = "username")
        {
            Username = username;
        }
    }
}