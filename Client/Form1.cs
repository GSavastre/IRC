using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using irc;

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
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            client = new TcpClient(server_addr, server_port);

            ircMessage msg = new ircMessage(IPAddress.Parse(server_addr), server_port, "bot_user", tb_test.Text);

            NetworkStream stream = client.GetStream();
            stream.Write(ObjToBytes(msg), 0, ObjToBytes(msg).Length);

            stream.Close();
            client.Close();
        }

        /// <summary>
        ///  Converte un Oggetto qualsiasi in un array di byte
        /// </summary>
        /// <param msg="obj Message da convertire">
        /// </param>
        private byte[] ObjToBytes(ircMessage msg)
        {

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, msg);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///  Converte un array di byte 
        /// </summary>
        /// <param msg="array di byte da convertire">
        /// </param>
        private ircMessage BytesToObj(byte[] msg)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(msg, 0, msg.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (ircMessage)bf.Deserialize(ms);
            }
        }
    }
}
