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
using System.Net;
using System.Threading;

namespace Client
{
    public partial class Login : Form
    {
        string server_addr;
        static int server_port = 7777;
        TcpClient client;
        List<ircUser> online_users = new List<ircUser>(); /// lista di utenti
        TcpListener listener = new TcpListener(IPAddress.Any, server_port);

        delegate void CloseFormCallback();

        public Login(string myServer_addr)
        {          
            InitializeComponent();
            server_addr = myServer_addr;
            this.DialogResult = DialogResult.OK;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            try
            {
                listener.Start();
                client = new TcpClient(server_addr, server_port);

                ircMessage regMessage = new ircMessage(tb_log_username.Text, tb_log_password.Text, 1); //oggetto messagge per Login action = 1

                NetworkStream stream = client.GetStream();
                stream.Write(ircMessage.ObjToBytes(regMessage), 0, ircMessage.ObjToBytes(regMessage).Length);
                
                stream.Close();
                client.Close();

                try
                {
                    TcpClient clientlistener;
                    byte[] buffer = new byte[1024];
                    clientlistener = listener.AcceptTcpClient();
                    NetworkStream streamlistener = clientlistener.GetStream();
                    int len = streamlistener.Read(buffer, 0, buffer.Length);

                    if (((List<ircUser>)ircMessage.BytesToObj(buffer, len)).Count() == 0) {
                        MessageBox.Show("Invalid Login !");
                        listener.Stop();
                        streamlistener.Close();
                        clientlistener.Close();
                    } else {

                        online_users = (List<ircUser>)ircMessage.BytesToObj(buffer, len);
                        listener.Stop();
                        streamlistener.Close();
                        clientlistener.Close();

                        Form home = new Home(server_addr, new ircUser(tb_log_username.Text, Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()), online_users);

                        this.Hide();
                        home.ShowDialog();
                        this.Close();
                    }
                } catch{
                    MessageBox.Show("Connessione al server scaduta, ritorno alla lista di server disponibili.","Errore connessione al server");
                    this.Close();
                }
            } catch {
                MessageBox.Show("Connessione al server scaduta, ritorno alla lista di server disponibili.", "Errore connessione al server");
                this.Close();
            }
        }

        private void btn_switch_reg_Click(object sender, EventArgs e)
        {
            try
            {
                listener.Stop();             
                this.DialogResult = DialogResult.Yes; //messaggio che ritorna la finestra dialogo se si vuole fare switch
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
