using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Message
    {
        public IPAddress ipSender;
        public int portSender;
        public IPAddress ipReciver;
        public int portReciver;
        public string message;
        public int hashCode; //TODO

        public Message(IPAddress myIpSender, int myPortSender, IPAddress myIpReciver, int myPortReciver, string myMessage)
        {
            ipSender = myIpSender;
            portSender = myPortSender;
            ipReciver = myIpReciver;
            portReciver = myPortReciver;
            message = myMessage;
        }

    }
}
