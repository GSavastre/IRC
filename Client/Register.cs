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
        int server_port = 7777;
        TcpClient client;
        Thread tcpListenerThread = null; //thread client listener
        List<ircUser> online_users = null; /// lista di utenti
        TcpListener listener = null;

        public Register(string myServer_addr)
        {
            InitializeComponent();

            server_addr = myServer_addr;
            this.DialogResult = DialogResult.OK;
            StartTcpListenerThread();
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

                    /*if (!tcpListenerThread.IsAlive)
                    {
                        Form myHome = new Home(server_addr, online_users);
                        this.Hide();
                        myHome.ShowDialog();
                        this.Close();
                    }*/
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
            client.Close();
            tcpListenerThread.Abort();
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void StartTcpListenerThread()
        {
            listener = new TcpListener(IPAddress.Any, server_port);
            TcpClient client = null;
            listener.Start();
            
            tcpListenerThread = new Thread(() =>
            {
                bool loop = true;
                while (loop)
                {
                    try
                    {
                        client = listener.AcceptTcpClient();
                        byte[] buffer = new byte[1024];
                        NetworkStream stream = client.GetStream();
                        int len = stream.Read(buffer, 0, buffer.Length);

                        if (((List<ircUser>)ircMessage.BytesToObj(buffer, len)).Count() == 0)
                        {
                            MessageBox.Show("Invalid Register !");                           
                        }
                        else if(len!=0)
                        {
                            List<ircUser> online_users = (List<ircUser>)ircMessage.BytesToObj(buffer,len);
                            listener.Stop();
                            loop = false;
                        }
                        len = 0;
                        stream.Close();
                        client.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }
    }
}
