using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using irc;

namespace Client
{
    public partial class ChatBox : Form
    {
        string server_addr;
        const int port = 7777;
        TcpClient client = null;
        string partner_username;

        public ChatBox(string partner_username, string server_addr)
        {
            InitializeComponent();
            this.partner_username = partner_username;
            this.server_addr = server_addr;
            Text = partner_username;
        }

        public void AddMessage(String message) {
            lb_chat.Items.Add(message);
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient(server_addr, port);

                ircMessage msg = new ircMessage(Home.current_user.username, partner_username, tb_msg.Text, 2);

                NetworkStream stream = client.GetStream();
                stream.Write(ircMessage.ObjToBytes(msg), 0, ircMessage.ObjToBytes(msg).Length);

                lb_chat.Items.Add(msg.message);

                tb_msg.Text = ""; //Ripulisce casella di scrittura del form
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chat send exception : " + ex.Message);
            }
        }
    }
}
