using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace irc
{
    [Serializable]
    public class ircUser
    {
        public string username;
        public string address;

        public ircUser(string username, string address) {
            this.username = username;
            this.address = address;
        }

    }
}
