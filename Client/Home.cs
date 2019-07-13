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

            DiscoverServers();

            current_user = new ircUser(0, "Dax");
            online_users = new List<ircUser> {
                new ircUser(1, "Loca"),
                new ircUser(2, "Sava")
            };

            l_user.Text = current_user.username;
        }

        void DiscoverServers() {
            UdpClient client = new UdpClient();
            byte[] requestData = Encoding.ASCII.GetBytes("DISCOVER_IRCSERVER_REQUEST");
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

            client.EnableBroadcast = true;
            client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 7777));

            string serverResponse = Encoding.ASCII.GetString(client.Receive(ref serverEndPoint));
            Console.WriteLine($"Received {serverResponse} from {serverEndPoint.Address.ToString()}");

            client.Close();
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
