using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    class Message
    {
        public IPAddress ipSender;
        public int portSender;
        public string reciver_username;
        public string message;
        public int action; //Azione richiesta al server 0: Registrazione 1:Login 2:Message 2:Logout

        public Message(IPAddress myIpSender, int myPortSender, string myReciver_username, string myMessage, int myAction)
        {
            ipSender = myIpSender;
            portSender = myPortSender;
            reciver_username = myReciver_username;
            message = myMessage;
            action = myAction; //azione di invio messaggio
        }

        public Message(string myUsername, string myPassword, int myAction) //costruttore per Registrazione e Login
        {
            message = myUsername + "~" + myPassword;
            action = myAction; //azione di invio messaggio
        }

        public Message(string myUsername,int myAction) //costruttore per Logout
        { 
            message = myUsername;
            action = myAction; //azione di invio messaggio
        }
    }
}
