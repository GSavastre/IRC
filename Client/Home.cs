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
            string response = DiscoverServers();
            MessageBox.Show(response, "Indirizzo del server trovato");
        }

        string DiscoverServers() {
            /*UdpClient client = new UdpClient();
            byte[] requestData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

            client.EnableBroadcast = true;
            client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 7777));

            string serverResponse = Encoding.ASCII.GetString(client.Receive(ref serverEndPoint));
            Console.WriteLine($"Received {serverResponse} from {serverEndPoint.Address.ToString()}");

            client.Close();*/

            /*IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast,7777);
            byte[] requestData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics) {
                if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) {
                    continue;
                }

                if (adapter.Supports(NetworkInterfaceComponent.IPv4) == false) {
                    continue;
                }

                try {

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    foreach (var ua in adapterProperties.UnicastAddresses) {
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork) {
                            string responseString = "";
                            Socket bcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            bcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                            bcSocket.ReceiveTimeout = 200;
                            IPEndPoint myLocalEndPoint = new IPEndPoint(ua.Address, 7777);
                            bcSocket.Bind(myLocalEndPoint);
                            bcSocket.SendTo(requestData, broadcastEP);

                            byte[] serverResponse = new byte[3];

                            do {
                                try {
                                    bcSocket.Receive(serverResponse);
                                    responseString = Encoding.ASCII.GetString(serverResponse);
                                } catch {
                                    break;
                                }
                            } while (bcSocket.ReceiveTimeout != 0);
                            bcSocket.Close();
                            return responseString;
                        }
                    }
                } catch (Exception e) {
                    MessageBox.Show(e.Message, "Errore in spedizione broadcast");
                }
            }

            return "";*/

            const int port = 7777;
            string serverIp = "?";
            byte[] requestData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");

            //Get all addresses
            string hostname = Dns.GetHostName();
            IPHostEntry allLocalNetworkAddresses = Dns.Resolve(hostname);

            foreach (IPAddress ip in allLocalNetworkAddresses.AddressList) {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                client.Bind(new IPEndPoint(ip,0));

                IPEndPoint allEndPoint = new IPEndPoint(IPAddress.Broadcast, port) ;

                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                client.SendTo(requestData,allEndPoint);
                MessageBox.Show("Ho mandato il messaggio in broadcast", "Client notice");

                try {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any,0);
                    EndPoint tempRemoteEP = (EndPoint)sender;
                    byte[] buffer = new byte[3];

                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);

                    client.ReceiveFrom(buffer, ref tempRemoteEP);
                    string response = Encoding.ASCII.GetString(buffer);
                    MessageBox.Show($"Received {response} from {tempRemoteEP.ToString()}", "Notice");

                    if (response == "ACK") {
                        serverIp = tempRemoteEP.ToString();

                        break;
                    }

                } catch (Exception) {
                    //No server answered try next.
                }


            }

            return serverIp;
        }



        
        void LoadContacts(List<ircUser> users) {
            foreach (ircUser user in online_users) {
                Panel panel = new Panel();
                panel.Size = new Size(252, 48);

                Label label_user = new Label();
                label_user.Text = user.username;
                label_user.Location = new Point(10, 10);

                Button btn = new Button();
                btn.Size = new Size(75, 34);
                btn.Tag = user;
                btn.Text = "Start Chat";
                btn.Location = new Point(165, 7);

                panel.Controls.Add(label_user);
                panel.Controls.Add(btn);

                flp_contacts.Controls.Add(panel);
            }
        }
    }
}
