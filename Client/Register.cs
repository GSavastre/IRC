using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using irc;

namespace Client
{
    public partial class Register : Form
    {
        string server_addr = "127.0.0.1";
        int server_port = 7777;
        TcpClient client;

        public Register()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.OK;
        }

        private void btn_register_Click(object sender, EventArgs e)
        {

            client = new TcpClient(server_addr, server_port);

            ircMessage regMessage = new ircMessage(tb_username.Text, tb_password.Text, 0); //oggetto messagge per registrazione action = 0

            NetworkStream stream = client.GetStream();
            stream.Write(ircMessage.ObjToBytes(regMessage), 0, ircMessage.ObjToBytes(regMessage).Length);
            stream.Close();
            client.Close();  
        }

        private void btn_switch_login_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
