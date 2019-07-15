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
        string server_addr;
        int server_port = 7777;
        TcpClient client;

        public Login(string myServer_addr)
        {          
            InitializeComponent();
            server_addr = myServer_addr;
            this.DialogResult = DialogResult.OK; ;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient(server_addr, server_port);

                ircMessage regMessage = new ircMessage(tb_log_username.Text, tb_log_password.Text, 1); //oggetto messagge per Login action = 1

                NetworkStream stream = client.GetStream();
                stream.Write(ircMessage.ObjToBytes(regMessage), 0, ircMessage.ObjToBytes(regMessage).Length);

                stream.Close();
                client.Close();

                ///TODO: ASPETTARE ESITO LOGIN E POI CREARE UTENTE 
                //ircUser curruntUser = new ircUser(id, username, address)

                Form myHome = new Home(server_addr);
                this.Hide();
                myHome.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_switch_reg_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes; //messaggio che ritorna la finestra dialogo se si vuole fare switch
            this.Close();
        }
    }
}
