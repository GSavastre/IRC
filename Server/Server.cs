using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

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
                Console.WriteLine("Waiting...");
                client = server.AcceptTcpClient();

                byte[] buffer = new byte[256];
                NetworkStream stream = client.GetStream();

                stream.Read(buffer, 0, buffer.Length);

                StringBuilder msg = new StringBuilder();

                foreach(byte b in buffer)
                {
                    if (b.Equals(00))
                        break;
                    else
                        msg.Append(Convert.ToChar(b).ToString());
                }
                Console.WriteLine(msg);
            }
        }
    }

    
}
