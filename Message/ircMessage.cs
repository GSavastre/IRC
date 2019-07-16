using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace irc
{
    [Serializable]
    public class ircMessage
    {
        public string sender_username { get; set; }
        public string receiver_username { get; set; }
        public string message { get; set; }
        public int action { get; set; }
        //public int hashCode; TODO

        public ircMessage(string myUsername, string myReceiver_username, string myMessage, int myAction) //costruttore Message
        {
            sender_username = myUsername;
            receiver_username = myReceiver_username; 
            message = myMessage;
            action = myAction;
        }

        public ircMessage(string myUsername, string myPassword, int myAction) //costruttore per Registrazione(0) e Login(1)
        {
            message = myUsername + ":" + myPassword;
            action = myAction; //azione di invio messaggio
        }

        public ircMessage(string myUsername, int myAction) //costruttore per Logout(3)
        {
            sender_username = myUsername;
            action = myAction; //azione di invio messaggio
        }

        /// <summary>
        ///  Converte un Oggetto qualsiasi in un array di byte
        /// </summary>
        /// <param msg="obj Message da convertire">
        /// </param>
        public static byte[] ObjToBytes(object msg)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, msg);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///  Converte un array di byte 
        /// </summary>
        /// <param msg="array di byte da convertire">
        /// </param>
        public static object BytesToObj(byte[] msg, int len)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(msg, 0, len);
                ms.Seek(0, SeekOrigin.Begin);
                object m = bf.Deserialize(ms);
                return m;
            }
        }

    }
}
