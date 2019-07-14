using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using irc;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Web.Helpers;

namespace Server
{
    class Server
    {
        //Messaggio di risposta alla server discovery request
        private static byte[] listenerResponseData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_ACK");

        //Il messaggio di richiesta dovrà corrispondere a questa stringa
        private static byte[] listenerRequestCheck = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");

        //Porta su cui il server gestirà le comunicazioni discovery
        private const int discoveryPort = 7778;

        private const int port = 7777;

        //Creo nuovo socket UDP su cui ascoltare le richieste dei client
        Socket serverListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //Lista di utenti online sul server
        List<ircUser> onlineUsers;

        public Server()
        {
            //Usando IPAddress.any indico alla socket che deve ascoltare per attività su tutte le interfacce di rete
            //Usando port indico alla socket che deve ascoltare su quella specifica porta
            serverListener.Bind(new IPEndPoint(IPAddress.Any,discoveryPort));

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Console.WriteLine("Server listening...");
            TcpListener server = new TcpListener (ip, port);
            //TcpClient client = default(TcpClient);

            onlineUsers = new List<ircUser>();

            Login(username, password, IPAddress.Loopback);

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
                //Il server deve sempre avere il listener attivato per poter rispondere ai client man mano che lo cercano
                DiscoveryListener();

                TcpClient client = default(TcpClient);
                client = server.AcceptTcpClient();

                byte[] buffer = new byte[1024];
                NetworkStream stream = client.GetStream();

                stream.Read(buffer, 0, buffer.Length);

                Console.WriteLine(ircMessage.BytesToObj(buffer).message);
            }
        }
        #region discoveryListener
        /// <summary>
        ///     Server discovery listener usato per rispondere alle richieste UDP broadcast dei client.
        /// </summary>
        private void DiscoveryListener() {
            /* Il sender è colui che invia una richiesta discovery
             * in questo caso sarà un client che può avere un qualsiasi
             * indirizzo e usare una qualsiasi porta
             */
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempRemoteEP = (EndPoint)sender;
            
            //Il messaggio in byte ricevuto dal client
            byte[] buffer = new byte[listenerRequestCheck.Length];

            //Riceve messaggi da chiunque
            serverListener.ReceiveFrom(buffer, ref tempRemoteEP);

            Console.WriteLine($"Server got {Encoding.ASCII.GetString(buffer)} from {tempRemoteEP.ToString()}");

            //Controllo validità del messaggio di richiesta
            if (Encoding.ASCII.GetString(buffer).Equals(Encoding.ASCII.GetString(listenerRequestCheck))) {
                Console.WriteLine($"Sending {Encoding.ASCII.GetString(listenerResponseData)} to {tempRemoteEP.ToString()}");

                //Invio risposta al client
                serverListener.SendTo(listenerResponseData, tempRemoteEP);
            } else {
                Console.WriteLine($"Bad message from {tempRemoteEP.ToString()} not replying...");
            }
        }
        #endregion

        #region userAuth

        private List<ircUser> Login(string username, string password, IPAddress address) {

            Console.WriteLine($"Inizio processo di login per {username}");

            DBManager dbManager = new DBManager();

            DataTable result = dbManager.Query(TableNames.usersTable, $"SELECT * FROM {TableNames.usersTable} WHERE username = '{username}'");
            
            if (result.Rows.Count != 1) {
                Console.WriteLine("Login fallito!");
                return null;
            } else {
                if (Crypto.VerifyHashedPassword(result.Rows[0]["password"].ToString(), password)) {
                    onlineUsers.Add(new ircUser((int)result.Rows[0]["user_id"], username, address));
                } else {
                    Console.WriteLine("Login fallito!");
                    return null;
                }
            }

            return this.onlineUsers;
        }

        //TODO: Add password repeat control on Client
        private void Register(string username, string password) {
            Console.WriteLine($"Inizio processo di registrazione per {username}");
            DBManager dbManager = new DBManager();

            dbManager.Insert(TableNames.usersTable, new Dictionary<string, string> {
                {"username", username},
                {"password", Crypto.HashPassword(password)}
            });
        }

        #endregion

        #region messageConversion

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
        #endregion
    }
}