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

            Message regMessage = new Message(tb_log_username.Text, tb_log_password.Text, 0); //oggetto messagge per registrazione action = 0

            int msgLenght = Encoding.ASCII.GetByteCount(tb_log_password.Text);  //lunghezza in byte del messaggio
            byte[] msg_data = new byte[msgLenght];                      //inzializzo array con dim = lunghezza del messaggio
            msg_data = Encoding.ASCII.GetBytes(tb_log_password.Text);           //inserisco nell array il messaggio in byte

            NetworkStream stream = client.GetStream();
            stream.Write(msg_data, 0, msgLenght);
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
