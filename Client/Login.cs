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
                client = new TcpClient(server_addr, server_port);

                listener.Start();
                ircMessage regMessage = new ircMessage(tb_log_username.Text, tb_log_password.Text, 1); //oggetto messagge per Login action = 1

                NetworkStream stream = client.GetStream();
                stream.Write(ircMessage.ObjToBytes(regMessage), 0, ircMessage.ObjToBytes(regMessage).Length);
                
                stream.Close();
                client.Close();

                try{
                while (true)
                {
                    TcpClient clientlistener;
                    clientlistener = listener.AcceptTcpClient();
                    byte[] buffer = new byte[1024];
                    NetworkStream streamlistener = clientlistener.GetStream();
                    int len = streamlistener.Read(buffer, 0, buffer.Length);

                    if (((List<ircUser>)ircMessage.BytesToObj(buffer, len)).Count() == 0)
                    {
                        MessageBox.Show("Invalid Login !");
                        streamlistener.Close();
                        clientlistener.Close();
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Login effettuato !");
                        online_users = (List<ircUser>)ircMessage.BytesToObj(buffer, len);
                            
                        streamlistener.Close();
                        clientlistener.Close();

                        MessageBox.Show("Home Apertura");
                        Form myHome = new Home(server_addr, online_users);

                        this.Hide();
                        myHome.ShowDialog();
                        this.Close();
                        break;
                    }
                }

            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
            }
                
            /*
            int msgLenght = Encoding.ASCII.GetByteCount("Richiedo Login");  //lunghezza in byte del messaggio 
            byte[] msg_data = new byte[msgLenght];                      //inzializzo array con dim = lunghezza del messaggio 
            msg_data = Encoding.ASCII.GetBytes("Richiedo Login");           //inserisco nell array il messaggio in byte 
                

            NetworkStream stream = client.GetStream();
            stream.Write(msg_data, 0, msgLenght);
            stream.Close();*/

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
