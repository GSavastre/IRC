using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using irc;
using System.Linq;

namespace Client
{
    public partial class Chat : Form
    {
        string server_addr;
        const int port = 7777;
        TcpClient client = null;
        ircUser partner = null;

        public Chat(ircUser myPartner, string myServer_addr)
        {
            InitializeComponent();
            server_addr = myServer_addr;
            lb_chat.Items.Add(server_addr);
            partner = myPartner;
            l_partner.Text = partner.username;
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            client = new TcpClient(server_addr, port);

            ircMessage msg = new ircMessage(Home.current_user.username , partner.username , tb_msg.Text, 2);

            NetworkStream stream = client.GetStream();
            stream.Write(ircMessage.ObjToBytes(msg), 0, ircMessage.ObjToBytes(msg).Length);

            lb_chat.Items.Add(msg.message);

            tb_msg.Text = "";
            stream.Close();
            client.Close();
        }
    }
}
