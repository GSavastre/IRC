using System;
using System.Collections.Generic;
using System.Text;

namespace irc
{
    [Serializable]
    public class ircUser
    {
        public int id;
        public string username;

        public ircUser(int myId, string myUsername) {
            id = myId;
            username = myUsername;
        }
    }
}
