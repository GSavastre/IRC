using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace irc
{
    [Serializable]
    public class ircUser
    {
        public int id;
        public string username;
        public IPAddress address;

        public ircUser(int id, string username, string address) {
            this.id = id;
            this.username = username;
            this.address = IPAddress.Parse(address);
        }

        public ircUser(int id, string username, IPAddress address) {
            this.id = id;
            this.username = username;
            this.address = address;
        }
    }
}
