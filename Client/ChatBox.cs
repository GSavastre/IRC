using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using irc;

namespace Client
{
    public partial class ChatBox : Form
    {
        string server_addr;
        const int port = 7777;
        TcpClient client = null;
        string partner_username;
        Home home_reference = null; //variabile di referenza per utilizzare il form home 

        public ChatBox(string partner_username, string server_addr, Home home)
        {
            InitializeComponent();
            this.partner_username = partner_username;
            this.server_addr = server_addr;
            Text = partner_username;
            home_reference = home;
        }

        /// <summary>
        /// aggiunge messaggio al form chat
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(String message) {
            lb_chat.Items.Add(message);
        }

        /// <summary>
        /// Invio del messaggio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_send_Click(object sender, EventArgs e)
        {
            try
            {
                if (tb_msg.Text != "")
                {
                    client = new TcpClient(server_addr, port);

                    ircMessage msg = new ircMessage(Home.current_user.username, partner_username, tb_msg.Text, 2);

                    NetworkStream stream = client.GetStream();
                    stream.Write(ircMessage.ObjToBytes(msg), 0, ircMessage.ObjToBytes(msg).Length);
                    lb_chat.Items.Add("You : " + msg.message);

                    tb_msg.Text = ""; //Ripulisce casella di scrittura del form
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chat send exception : " + ex.Message);
            }
        }
        
        /// <summary>
        /// Chiudendo il form dalla "x" si stoppa il listenere e si chiude il thread di ascolto
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    home_reference.EndChat(this);
                    break;
            }
        }
        
    }
}
