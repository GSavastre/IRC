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
using System.Threading;

namespace Client
{
    public partial class Home : Form
    {
        const int port = 7777;
        string server_addr = "";
        public static ircUser current_user;
        List<ircUser> online_users;

        Thread tcpListenerThread = null;

        public Home(string myServer_addr, List<ircUser> online_users)
        {
            InitializeComponent();
            server_addr = myServer_addr;
            //current_user = myCurrent_user;
            //online_users = myOnline_users;

           current_user = new ircUser(0, "Dax", "192.168.0.107");
           online_users = new List<ircUser> {
                new ircUser(1, "Loca", ""),
                new ircUser(2, "Sava", "")
            };

            l_user.Text = current_user.username;

            LoadContacts(online_users);
            StartTcpListenerThread();
        }

        /// <summary>
        ///  Genere la GUI per la lista di utenti online
        /// </summary>
        /// <param users="Lista di Utenti online">
        ///     List<ircUser> contenente gli utenti online
        /// </param>
        void LoadContacts(List<ircUser> users) {
            foreach (ircUser user in online_users) {
                Panel panel = new Panel
                {
                    Size = new Size(252, 48)
                };

                Label label_user = new Label
                {
                    Text = user.username,
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                    Location = new Point(10, 10),
                    Size = new Size(70, 16)
                };

                Button btn = new Button
                {
                    Size = new Size(75, 34),
                    Tag = user,
                    Text = "Start Chat",
                    Location = new Point(165, 7)
                };
                btn.Click += new EventHandler(startChat_Button_Click);

                Button btn_status = new Button
                {
                    Size = new Size(15, 15),
                    Location = new Point(12, 28),
                    BackColor = Color.LimeGreen
                };

                Label label_online = new Label
                {
                    Text = "Online",
                    Font = new Font("Microsoft Sans Serif", 8, FontStyle.Italic),
                    Location = new Point(33, 30),
                    Size = new Size(37, 13)
                };

                Label sep = new Label
                {
                    AutoSize = false,
                    Height = 2,
                    Width = 250,
                    BorderStyle = BorderStyle.Fixed3D
                };

                panel.Controls.Add(label_user);
                panel.Controls.Add(btn);
                panel.Controls.Add(btn_status);
                panel.Controls.Add(label_online);

                flp_contacts.Controls.Add(panel);
                flp_contacts.Controls.Add(sep);
            }
        }

        /// <summary>
        ///  Handler dinamico per instanziare nuove chat.
        /// </summary>
        private void startChat_Button_Click(object sender, EventArgs e)
        {
            Button partner_button = sender as Button;
            ircUser partner = new ircUser(((ircUser)partner_button.Tag).id, ((ircUser)partner_button.Tag).username, ((ircUser)partner_button.Tag).address);
            Form chat = new Chat(partner, server_addr);
            chat.Show();
        }

        private void StartTcpListenerThread()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            TcpClient client;
            listener.Start();
            tcpListenerThread = new Thread(() =>
            {
                tcpListenerThread.IsBackground = true;
                while (true)
                {
                    try
                    {
                        client = listener.AcceptTcpClient();
                        byte[] buffer = new byte[1024];
                        NetworkStream stream = client.GetStream();
                        int len = stream.Read(buffer, 0, buffer.Length);

                        ircMessage msg = (ircMessage)ircMessage.BytesToObj(buffer, len);
                        MessageBox.Show(msg.sender_username + " " + msg.message + " " + msg.receiver_username);
                        
                        /*
                         * TODO*
                         * -Controllo da chi arriva il messaggio
                         * -Mostro messaggio in chat corrispondente
                         */
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
            tcpListenerThread.Start();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                client = new TcpClient(server_addr, port);

                ircMessage msg_logout = new ircMessage(current_user.username, 2);
                stream = client.GetStream();
                stream.Write(ircMessage.ObjToBytes(msg_logout), 0, ircMessage.ObjToBytes(msg_logout).Length);

                stream.Close();
                client.Close();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                stream.Close();
                client.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
