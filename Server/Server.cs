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
using System.Threading;

namespace Server
{
    class Server
    {
        /*//Messaggio di risposta alla server discovery request
        private static byte[] listenerResponseData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_ACK");

        //Il messaggio di richiesta dovrà corrispondere a questa stringa
        private static byte[] listenerRequestCheck = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");*/

        //Porta su cui il server gestirà le comunicazioni discovery
        private const int discoveryPort = 7778;

        private const int port = 7777;

        

        //Lista di utenti online sul server
        List<ircUser> onlineUsers;
        Thread tcpListnerThread = null;

        public Server()
        {
<<<<<<< HEAD
            Thread discoveryListener = new Thread(new ThreadStart(DiscoveryListener));
            discoveryListener.Start();
=======
            //Usando IPAddress.any indico alla socket che deve ascoltare per attività su tutte le interfacce di rete
            //Usando port indico alla socket che deve ascoltare su quella specifica porta
            serverListener.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));
>>>>>>> ServerSwitchImplementation

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener (ip, port);
<<<<<<< HEAD
            TcpClient client = default(TcpClient);
=======
>>>>>>> ServerSwitchImplementation

            onlineUsers = new List<ircUser>();

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
            
            tcpListnerThread = new Thread(() =>
            {
<<<<<<< HEAD
                //TcpClient client = default(TcpClient);
                client = server.AcceptTcpClient();

                byte[] buffer = new byte[1024];
                NetworkStream stream = client.GetStream();

                stream.Read(buffer, 0, buffer.Length);

                Console.WriteLine(ircMessage.BytesToObj(buffer).message);

                ircMessage myMessage = ircMessage.BytesToObj(buffer);
                switch (myMessage.action)
=======
                while (true)
>>>>>>> ServerSwitchImplementation
                {
                    //Il server deve sempre avere il listener attivato per poter rispondere ai client man mano che lo cercano
                    DiscoveryListener();

                    TcpClient client = default(TcpClient);
                    client = server.AcceptTcpClient();
                    Console.WriteLine(client.ToString());
                
                    byte[] buffer = new byte[1024];
                    NetworkStream stream = client.GetStream();

                    stream.Read(buffer, 0, buffer.Length);

                    Console.WriteLine(ircMessage.BytesToObj(buffer).message);

                    ircMessage msg = ircMessage.BytesToObj(buffer);
                    switch (msg.action)
                    {
                        case 0:                                                             //Registrazione nuovo utente
                            Console.WriteLine("REGISTER_USER_REQUEST Received");
                            Register(msg.message.Split('~')[0], msg.message.Split('~')[1]);     //msg.message contiene username+password; vado a divide la stringa in 2
                            break;

                        case 1: //Login
                            Console.WriteLine("LOGIN_USER_REQUEST Received");
                            //Login() to fix, no need to pass user pwd
                            break;
                        case 2: //Message
                            Console.WriteLine("MESSAGE Received");
                            break;
                        case 3: //Logout
                            Console.WriteLine("Logout case");
                            break;
                    }
                }
            });

            tcpListnerThread.Start();
        }

        #region discoveryListener
        /// <summary>
        ///     Server discovery listener usato per rispondere alle richieste UDP broadcast dei client.
        /// </summary>
        private void DiscoveryListener() {

            //Messaggio di risposta alla server discovery request
            byte[] listenerResponseData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_ACK");

            //Il messaggio di richiesta dovrà corrispondere a questa stringa
            byte[] listenerRequestCheck = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");

            //Creo nuovo socket UDP su cui ascoltare le richieste dei client
            Socket serverListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //Usando IPAddress.any indico alla socket che deve ascoltare per attività su tutte le interfacce di rete
            //Usando port indico alla socket che deve ascoltare su quella specifica porta
            serverListener.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));

            while (true) {
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
        }
        #endregion

        #region userAuth

        private List<ircUser> Login(string username, string password, string address) {

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
        
        private void Register(string username, string password) {
            Console.WriteLine($"Inizio processo di registrazione per {username}");
            DBManager dbManager = new DBManager();

            dbManager.Insert(TableNames.usersTable, new Dictionary<string, string> {
                {"username", username},
                {"password", Crypto.HashPassword(password)}
            });
        }

        #endregion

        void RedirectData() {
            //TODO 
        }
        
    }
}