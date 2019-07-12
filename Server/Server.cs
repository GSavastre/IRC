using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using irc;

namespace Server
{
    class Server
    {
        public Server()
        {
            int port = 7777;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener (ip, port);
            TcpClient client = default(TcpClient);

            try
            {
                server.Start();
                Console.WriteLine("Server started...");
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                Console.Read();
            }

            while (true)
            {
                client = server.AcceptTcpClient();

                byte[] buffer = new byte[1024];
                NetworkStream stream = client.GetStream();

                stream.Read(buffer, 0, buffer.Length);

                Console.WriteLine(BytesToObj(buffer).message);
            }
        }

        /// <summary>
        ///  Converte un Oggetto qualsiasi in un array di byte
        /// </summary>
        /// <param msg="obj Message da convertire">
        /// </param>
        private byte[] ObjToBytes(ircMessage msg)
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
        private ircMessage BytesToObj(byte[] msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(msg, 0, msg.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (ircMessage)bf.Deserialize(ms);
            }
        }
    }

    
}
