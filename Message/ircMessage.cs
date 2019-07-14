using System;
using System.Net;

namespace irc
{
    [Serializable]
    public class ircMessage
    {
        public IPAddress ipSender;
        public int portSender;
        public string receiver_username;
        public string message;
        //public int hashCode; TODO

        public ircMessage(IPAddress myIpSender, int myPortSender, string myReceiver_username, string myMessage)
        {
            ipSender = myIpSender;
            portSender = myPortSender;
            receiver_username = myReceiver_username;
            message = myMessage;
        }

    }
}
