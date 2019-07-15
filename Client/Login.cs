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
using irc;

namespace Client
{
    public partial class Login : Form
    {
        string server_addr = "127.0.0.1";
        int server_port = 7777;
        TcpClient client;

        public Login(string myServer_addr)
        {
            InitializeComponent();
            this.DialogResult = DialogResult.OK; ;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {

            client = new TcpClient(server_addr, server_port);

            ircMessage regMessage = new ircMessage(tb_log_username.Text, tb_log_password.Text, 1); //oggetto messagge per Login action = 1

            NetworkStream stream = client.GetStream();
            stream.Write(ircMessage.ObjToBytes(regMessage), 0, ircMessage.ObjToBytes(regMessage).Length);

            stream.Close();
            client.Close();
        }

        private void btn_switch_reg_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
