using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using irc;
using System.Net;
using System.Threading;
using System.Collections.Generic;

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
                }
                else
                    MessageBox.Show("Le password non coincidono. Riprova !");
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

        private void StartTcpListenerThread()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, server_port);
            TcpClient client = null;
            listener.Start();
            Thread tcpListenerThread = null;
            tcpListenerThread = new Thread(() =>
            {
                tcpListenerThread.IsBackground = true;
                while (true)
                {
                    try
                    {
                        client = listener.AcceptTcpClient();
                        byte[] buffer = new byte[1024];
                        NetworkStream stream = client.GetStream();
                        int len = stream.Read(buffer, 0, buffer.Length);

                        if (ircMessage.BytesToObj(buffer, len).ToString().Equals("IRCSERVER_INVALID_LOGIN"))
                        {
                            MessageBox.Show("Invalid Login !");
                            break;
                        }
                        else
                        {
                            List<ircUser> online_users = null;//(List<ircUser>)ircMessage.BytesToObj(buffer);

                            Form myHome = new Home(server_addr, online_users);
                            this.Hide();
                            myHome.ShowDialog();
                            this.Close();
                            break;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
            tcpListenerThread.Start();
        }
    }
}
