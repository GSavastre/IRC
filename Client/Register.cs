using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using irc;

namespace Client
{
    public partial class Register : Form
    {
        string server_addr;
        int server_port = 7777;
        TcpClient client;

        public Register(string myServer_addr)
        {
            InitializeComponent();

            server_addr = myServer_addr;
            this.DialogResult = DialogResult.OK;
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            try
            {
                if (tb_password_repeat.Text == tb_password.Text)
                {
                    client = new TcpClient(server_addr, server_port);

                    ircMessage regMessage = new ircMessage(tb_username.Text, tb_password.Text, 0); //oggetto messagge per registrazione action = 0

                    NetworkStream stream = client.GetStream();
                    stream.Write(ircMessage.ObjToBytes(regMessage), 0, ircMessage.ObjToBytes(regMessage).Length);
                    stream.Close();
                    client.Close();

                    ///TODO: ASPETTARE ESITO REGISTRAZIONE E POI CREARE UTENTE 
                    //ircUser curruntUser = new ircUser(id, username, address)

                    Form myHome = new Home(server_addr);
                    this.Hide();
                    myHome.Show();
                    this.Close();
                }
                else
                    MessageBox.Show("LE PASSWORD NON COINCIDONO");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_switch_login_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
