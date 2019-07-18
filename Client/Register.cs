using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using irc;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public partial class Register : Form
    {
        string server_addr;
        static int server_port = 7777;
        TcpClient client;
        List<ircUser> online_users; /// lista di utenti online
        TcpListener listener = new TcpListener(IPAddress.Any, server_port);
        

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
                    listener.Start();
                    try
                    {
                        TcpClient clientlistener;
                        byte[] buffer = new byte[1024];
                        clientlistener = listener.AcceptTcpClient();
                        NetworkStream streamlistener = clientlistener.GetStream();
                        int len = streamlistener.Read(buffer, 0, buffer.Length);

                        if (((List<ircUser>)ircMessage.BytesToObj(buffer, len)).Count() == 0)
                        {
                            MessageBox.Show("Invalid Register !");
                            listener.Stop();
                            streamlistener.Close();
                            clientlistener.Close();
                        }
                        else
                        {
                            online_users = (List<ircUser>)ircMessage.BytesToObj(buffer, len);
                            listener.Stop();
                            streamlistener.Close();
                            clientlistener.Close();

                            Form home = new Home(server_addr, new ircUser(tb_username.Text, Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()), online_users);

                            this.Hide();
                            home.ShowDialog();
                            this.Close();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Connessione al server scaduta, ritorno alla lista di server disponibili.", "Errore connessione al server");
                        this.Close();
                    }
                }
                else
                    MessageBox.Show("Le password non coincidono. Riprova !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_switch_login_Click(object sender, EventArgs e)
        {
            listener.Stop();
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
