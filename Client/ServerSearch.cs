using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Client {
    public partial class ServerSearch : Form {

        //Thread che una volta avviato invia messagi di discovery sulla rete locale
        Thread discoveryThread;
        Thread pingServersThread;

        //Lista di server disponibili Key: Nome server, Value: Indirizzo
        //TOFIX: Per adesso se due server hanno lo stesso nome, la lista di server non aggiungerà il secondo server
        Dictionary<string, string> serversList;
        int serverPingPort = 7779;

        Login loginForm;

        #region AddToList delegate method

        delegate void AddToListCallback(string text);
        delegate void RemoveFromListCallback(KeyValuePair<string, string> server);

        private void RemoveFromList(KeyValuePair<string, string> server) { 

            serversList.Remove(server.Key);
            lbServer.Items.Remove(server.Key);
        }

        /// <summary>
        /// Aggiunge un elemento alla lista di server disponibili
        /// </summary>
        /// <param name="text"><see cref="string"/> di struttura nome:indirizzo</param>
        private void AddToList(string text) {
            //InvokeRequired confronta l'identificativo del thread chiamante
            //con l'identificativo del thread di creazione
            //Se questi due thread sono diversi, ritorna true.
            if (this.lbServer.InvokeRequired) {
                AddToListCallback d = new AddToListCallback(AddToList);
                this.Invoke(d, new object[] { text });
            } else {
                string[] serverInfo = text.Split(':').ToArray();

                //Controllo che l'array sia stato creato correttamente
                if (serverInfo.Count().Equals(2)) {
                    //Serverinfo[]{nome, indirizzo}
                    string serverName = serverInfo[0];
                    string serverAddress = serverInfo[1];

                    //Controllo che non ci sia già il server nella lista Dictionary
                    if (!serversList.ContainsKey(serverName)) {
                        serversList.Add(serverName, serverAddress);

                        //Controllo che il server non sia già stato aggiunto alla lista di server disponibili
                        if (!lbServer.Items.Contains(serverName)) {

                            lbServer.Items.Add(serverName);
                        }
                    }
                }
            }
        }
        #endregion
     
        void PingServer()
        {
            RemoveFromListCallback remove = new RemoveFromListCallback(RemoveFromList);
            while (true)
            {

                if (serversList != null)
                {
                    foreach (KeyValuePair<string,string> server in serversList.ToList())
                    {
                        
                        TcpClient tcpClient = new TcpClient();

                        try
                        {
                            tcpClient.Connect(server.Value, serverPingPort);
                            tcpClient.Close();
                        }
                        catch (Exception)
                        {
                            Invoke(remove,server);
                        }

                        try
                        {
                            tcpClient.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
        }


        #region ServerDiscovery
        /// <summary>
        /// Funzione che invia richieste UDP in broadcast sulla LAN per la scoperta di server disponibili (Da usare in un thread)
        /// </summary>
        void DiscoverServers() {
            //La porta su cui il server ascolterà le richieste di discovery UDP
            const int port = 7778;

            //Messaggio di discovery
            byte[] requestData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");

            //Messaggio di risposta del server, così non tengo conto di possibili server malevoli che rispondono ad ogni richiesta broadcast
            byte[] replyDataConf = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_ACK");

            //Prendo gli indirizzi di tutte le interfacce di questo pc
            string hostname = Dns.GetHostName();

            #pragma warning disable CS0618 // Type or member is obsolete
            IPHostEntry allLocalNetworkAddresses = Dns.Resolve(hostname);
            #pragma warning restore CS0618 // Type or member is obsolete

            while (true) {
                //Attraverso tutte le interfacce
                foreach (IPAddress ip in allLocalNetworkAddresses.AddressList) {

                    //Creo una socket per ogni interfaccia su cui inviare e ricevere il messaggio di discovery
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    /*
                     * Si sceglie 0 perché la porta dovrebbe essere determinata in modo automatico dal sistema
                     * non possiamo usare la porta es.7777 fissa sul client perché ci potrebbero essere altri servizi
                     * che utilizzano quella porta, sul server deve essere fissa siccome è un punto di riferimento per i
                     * client, il server invece saprà su che porta inviare i suoi messaggi ai client in base alle informazioni
                     * allegate ai messaggi
                     */
                    client.Bind(new IPEndPoint(ip, 0));

                    //Creo un'interfaccia dal quale inviare il messaggio di discovery broadcast sulla porta definita del server
                    IPEndPoint allEndPoint = new IPEndPoint(IPAddress.Broadcast, port);

                    //Abilito broadcast sul socket client
                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                    //Invio il messaggio di discovery
                    client.SendTo(requestData, allEndPoint);
                    //MessageBox.Show("Ho mandato un messaggio in broadcast", "Client notice");

                    try {
                        //Creo oggetto per il server
                        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint tempRemoteEP = (EndPoint)sender;
                        byte[] raw = new byte[1024];
                        
                        //Ricevo dal server, non aspetto più di 3 secondi
                        client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);
                        int length = client.ReceiveFrom(raw, ref tempRemoteEP);

                        byte[] buffer = new byte[length];

                        for (int i = 0; i < length; i++) {
                            buffer[i] = raw[i];
                        }

                        //Ricevo il messaggio, l'interfaccia remota provverà le informazioni necessarie come indirizzo IP e porta
                        try {

                            //response[]{nome, indirizzo}
                            string[] response = Encoding.ASCII.GetString(buffer).Split(':').ToArray();

                            if (response[0].Equals(Encoding.ASCII.GetString(replyDataConf))) {
                                //Creo la stringa con struttura nomeServer:indirizzoServer
                                string serverInfo = response[1] + ":" + tempRemoteEP.ToString().Split(':').ToArray()[0];
                                //Chiamo metodo delegato per l'aggiunta del server
                                AddToList(serverInfo);
                            }
                        } catch (Exception e) {
                            MessageBox.Show(e.Message);
                        }

                    } catch {
                       //MessageBox.Show(e.Message);
                    }
                }
            }
        }
#endregion

        /// <summary>
        /// Costruttore ServerSearch()
        /// </summary>
        public ServerSearch() {
            InitializeComponent();

            discoveryThread = new Thread(new ThreadStart(DiscoverServers))
            {
                IsBackground = true //Settiamo thread come background così quando si chiude il main thread si chiudono anche quelli in background
            };
            serversList = new Dictionary<string, string>();

            pingServersThread = new Thread(new ThreadStart(PingServer))
            {
                IsBackground = true //Settiamo thread come background così quando si chiude il main thread si chiudono anche quelli in background
            };

            discoveryThread.Start();
            pingServersThread.Start();
        }

        /// <summary>
        /// Dopo aver selezionato il server tenta la connessione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmit_Click(object sender, EventArgs e) {

            string selectedServerName = lbServer.GetItemText(lbServer.SelectedItem);
            string selectedServerAddress = string.Empty;

            //Prendo l'indirizzo del server con nome selectedServerName
            if (!serversList.TryGetValue(selectedServerName, out selectedServerAddress)) {
                selectedServerAddress = string.Empty;
            }
            
            //Avvio il form di Login solo se si è selezionato un server dalla lista
            if (string.IsNullOrEmpty(selectedServerAddress)) {
                MessageBox.Show("Prima di continuare devi scegliere un server disponibile","Avviso");
            } else {

                discoveryThread.Suspend(); //sospendiamo il thread
                pingServersThread.Suspend();

                this.Hide();
                loginForm = new Login(selectedServerAddress);
                Form regForm;
                bool loop = true;
                while (loop) {
                    if (loginForm.ShowDialog() == DialogResult.Yes) {
                        regForm = new Register(selectedServerAddress);
                        if (regForm.ShowDialog() != DialogResult.Yes) {
                            loop = false;
                        }
                    } else { 
                        loop = false;
                    }
                }
                discoveryThread.Resume();   //riattiviamo il thread se torniamo qui
                pingServersThread.Resume();
                try
                {
                    this.Show();
                }
                catch {}
            }
        }

        private void BtnReset_Click(object sender, EventArgs e) {
            discoveryThread.Abort();
            this.Close();
        }
    }
}
