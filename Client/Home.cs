using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using irc;
using System.Net.NetworkInformation;

namespace Client
{
    public partial class Home : Form
    {
        ircUser current_user;
        List<ircUser> online_users;

        public Home(/*ircUser myCurrent_user, List<ircUser> myOnline_users*/)
        {
            InitializeComponent();
            //current_user = myCurrent_user;
            //online_users = myOnline_users;

            current_user = new ircUser(0, "Dax");
            online_users = new List<ircUser> {
                new ircUser(1, "Loca"),
                new ircUser(2, "Sava")
            };

            l_user.Text = current_user.username;

            LoadContacts(online_users);

            string response = DiscoverServers();
            MessageBox.Show($"Indirizzo: {response}", "Indirizzo del server trovato");
        }

        string DiscoverServers() {
            //La porta su cui il server ascolterà le richieste di discovery
            const int port = 7778;
            string serverIp = "?";

            //Messaggio di discovery
            byte[] requestData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");
            byte[] replyDataConf = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_ACK");

            //Prendo gli indirizzi di tutte le interfacce di questo pc
            string hostname = Dns.GetHostName();
            IPHostEntry allLocalNetworkAddresses = Dns.Resolve(hostname);

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
                client.Bind(new IPEndPoint(ip,0));

                //Creo un'interfaccia dal quale inviare il messaggio di discovery broadcast sulla porta definita del server
                IPEndPoint allEndPoint = new IPEndPoint(IPAddress.Broadcast, port) ;

                //Abilito broadcast sul socket client
                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                //Invio il messaggio di discovery
                client.SendTo(requestData,allEndPoint);
                MessageBox.Show("Ho mandato un messaggio in broadcast", "Client notice");

                try {
                    //Creo oggetto per il server
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any,0);
                    EndPoint tempRemoteEP = (EndPoint)sender;
                    byte[] buffer = new byte[replyDataConf.Length];

                    //Ricevo dal server, non aspetto più di 3 secondi
                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);

                    //Ricevo il messaggio, l'interfaccia remota provverà le informazioni necessarie come indirizzo IP e porta
                    client.ReceiveFrom(buffer, ref tempRemoteEP);
                    string response = Encoding.ASCII.GetString(buffer);
                    MessageBox.Show($"Received {response} from {tempRemoteEP.ToString()}", "Notice");

                    if (response.Equals(Encoding.ASCII.GetString(replyDataConf))) {
                        //Qui bisognerebbe filtrare l'indirizzo IP dall'interfaccia remota
                        serverIp = tempRemoteEP.ToString();

                        //Ho ricevuto la mia conferma dal server, non c'è bisogno di continuare a ciclare le altre interfacce
                        break;
                    }

                } catch (Exception) {
                    //No server answered try next.
                }
            }

            return serverIp;
        }

        
        /// <summary>
        ///  Genere la GUI per la lista di utenti online
        /// </summary>
        /// <param users="Lista di Utenti online">
        ///     List<ircUser> contenente gli utenti online
        /// </param>
        void LoadContacts(List<ircUser> users) {
            foreach (ircUser user in online_users) {
                Panel panel = new Panel();
                panel.Size = new Size(252, 48);

                Label label_user = new Label();
                label_user.Text = user.username;
                label_user.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
                label_user.Location = new Point(10, 10);
                label_user.Size = new Size(70, 16);

                Button btn = new Button();
                btn.Size = new Size(75, 34);
                btn.Tag = user;
                btn.Text = "Start Chat";
                btn.Location = new Point(165, 7);
                
                Button btn_status = new Button();
                btn_status.Size = new Size(15, 15);
                btn_status.Location = new Point(12, 28);
                btn_status.BackColor = Color.LimeGreen;

                Label label_online = new Label();
                label_online.Text = "Online";
                label_online.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Italic);
                label_online.Location = new Point(33, 30);
                label_online.Size = new Size(37, 13);

                Label sep = new Label();
                sep.AutoSize = false;
                sep.Height = 2;
                sep.Width = 250;
                sep.BorderStyle = BorderStyle.Fixed3D;
                
                panel.Controls.Add(label_user);
                panel.Controls.Add(btn);
                panel.Controls.Add(btn_status);
                panel.Controls.Add(label_online);

                flp_contacts.Controls.Add(panel);
                flp_contacts.Controls.Add(sep);
            }
        }
    }
}
