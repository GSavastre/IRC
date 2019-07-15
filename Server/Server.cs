using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Web.Helpers;
using System.Threading;
using irc;
using System.Linq;

namespace Server
{
    class Server
    {
        
        //Porta su cui il server gestirà le comunicazioni discovery
        private const int discoveryPort = 7778;

        private const int port = 7777;
        
        //Lista di utenti online sul server
        List<ircUser> onlineUsers;
        Thread tcpListnerThread = null;

        public Server()
        {
            Thread discoveryListener = new Thread(new ThreadStart(DiscoveryListener));
            discoveryListener.Start();
            
            IPAddress ip = IPAddress.Any;
            TcpListener server = new TcpListener (ip, port);
            
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

            tcpListnerThread = new Thread(() => {

                TcpClient client = default(TcpClient);

                while (true) {
                
                    client = server.AcceptTcpClient();

                    byte[] buffer = new byte[1024];
                    NetworkStream stream = client.GetStream();

                    stream.Read(buffer, 0, buffer.Length);

                    ircMessage msg = ircMessage.BytesToObj(buffer);
                    switch (msg.action) {
                        case 0: //Registrazione nuovo utente
                            Console.WriteLine("REGISTER_USER_REQUEST Received");
                            Register(msg.message.Split(':')[0], msg.message.Split(':')[1]);     //msg.message contiene username+password; vado a dividere la stringa in 2
                            break;

                        case 1: //Login
                            Console.WriteLine("LOGIN_USER_REQUEST Received");
                            string senderAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            Send(senderAddress, Login(msg.message.Split(':')[0],msg.message.Split(':')[1], senderAddress));
                            break;
                        case 2: //Message
                            Console.WriteLine("MESSAGE Received");
                            break;
                        case 3: //Logout
                            Console.WriteLine("LOGOUT_REQUEST received");
                            Logout(msg.sender_username);
                            break;
                    }
                }
            });
            tcpListnerThread.Start();
            /*Console.Write("Inizio processo di login\nUsername: ");
            string uname = Console.ReadLine().Trim();
            Console.Write("Password: ");
            string password = Console.ReadLine().Trim();

            if (Login(uname, password, IPAddress.Loopback.ToString()) != null) {
                Console.WriteLine("Login avvenuto con successo!");
            } else {
                Console.WriteLine("Login fail!");
            }

            Console.Write("Logout iniziato\nUserame: ");
            Logout(Console.ReadLine().Trim());
            Console.ReadLine();*/
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

        private ircMessage Login(string username, string password, string address) {

            Console.WriteLine($"Inizio processo di login per {username}");
            string invalidRequest = "IRCSERVER_INVALID_LOGIN";

            //Ricerco se l'utente è già presente nella lista di utenti online su questo server
            ircUser loggedUser = onlineUsers.Where(user => user.username.Equals(username)).FirstOrDefault();

            if (loggedUser == null) {
                DBManager dbManager = new DBManager();

                DataTable result = dbManager.Query(TableNames.usersTable, $"SELECT * FROM {TableNames.usersTable} WHERE username = '{username}'");

                if (result.Rows.Count != 1) {
                    Console.WriteLine($"Login fallito per {username}");
                    return new ircMessage("", "", invalidRequest, 0);
                } else {
                    if (Crypto.VerifyHashedPassword(result.Rows[0]["password"].ToString(), password)) {
                        onlineUsers.Add(new ircUser((int)result.Rows[0]["user_id"], username, address));
                    } else {
                        Console.WriteLine($"Login fallito per {username}");
                        return new ircMessage("", "", invalidRequest, 0);
                    }
                }
                Console.WriteLine($"Login avvenuto con successo per {username}");
                return new ircMessage("", "", "successo", 0);
            }
            Console.WriteLine($"Login fallito per {username}");
            return new ircMessage("", "", invalidRequest, 0);
        }
        
        private void Register(string username, string password) {
            Console.WriteLine($"Inizio processo di registrazione per {username}");
            DBManager dbManager = new DBManager();

            dbManager.Insert(TableNames.usersTable, new Dictionary<string, string> {
                {"username", username},
                {"password", Crypto.HashPassword(password)}
            });
        }

        private void Logout(string username) {
            Console.WriteLine($"Inizio il processo di LOGOUT per {username}");
            //Cerco se l'utente è presente nella lista di utenti online su questo server
            ircUser loggedUser = onlineUsers.Where(user=>user.username.Equals(username)).FirstOrDefault();

            if (loggedUser != null) {
                onlineUsers.Remove(loggedUser);
                Console.WriteLine($"Utente {username} trovato ed eliminato con successo dalla lista di utenti online");
            } else {
                Console.WriteLine($"Utente {username} non trovato");
            }
        }

        #endregion

        void RedirectData() {
            //TODO 
        }

        void Send(string destAddres,ircMessage message) {
            TcpClient sender = new TcpClient(destAddres, port);
            Byte[] data = ircMessage.ObjToBytes(message);

            NetworkStream stream = sender.GetStream();

            stream.Write(data, 0, data.Length);
        }
    }
}