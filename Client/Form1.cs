using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public partial class Form1 : Form
    {
        string server_addr = "127.0.0.1";
        int server_port = 7777;
        TcpClient client;

        public Form1()
        {
            InitializeComponent();
            
            try
            {
                client = new TcpClient(server_addr, server_port);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client Istance Exception : " + ex.ToString());
            }
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            int msgLenght = Encoding.ASCII.GetByteCount(tb_test.Text);  //lunghezza in byte del messaggio
            byte[] msg_data = new byte[msgLenght];                      //inzializzo array con dim = lunghezza del messaggio
            msg_data = Encoding.ASCII.GetBytes(tb_test.Text);           //inserisco nell array il messaggio in byte

            NetworkStream stream = client.GetStream();
            stream.Write(msg_data, 0, msgLenght);
            stream.Close();
        }
    }
}
