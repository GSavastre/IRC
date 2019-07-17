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
using System.Net;
using irc;
using System.Net.NetworkInformation;
using System.Threading;

namespace Client
{
    public partial class Home : Form
    {
        const int port = 7777;
        string server_addr = "";
        public static ircUser current_user;
        List<ircUser> online_users = null;

        TcpListener listener = null;
        Thread tcpListenerThread = null;

        List<Chat> chatList = new List<Chat>();

        IList<Thread> chatThreads;
        List<string> chatNames;

        public Home(string myServer_addr, ircUser myCurrent_user, List<ircUser> myOnline_users)
        {
            InitializeComponent();
            server_addr = myServer_addr;
            current_user = myCurrent_user;
            online_users = myOnline_users;
            
            l_user.Text = current_user.username;

            chatThreads = new List<Thread>();
            chatNames = new List<string>();

            LoadContacts();
            StartTcpListenerThread();
        }
        /// <summary>
        ///  Handler dinamico per instanziare nuove chat.
        /// </summary>
        private void startChat_Button_Click(object sender, EventArgs e)
        {
            Button partner_button = sender as Button;
            string receiverUsername = ((ircUser)partner_button.Tag).username;
            //Thread chatThread = chatThreads.Where(thread => thread.Name.Equals(receiverUsername)).FirstOrDefault();
            ircMessage newMessage = new ircMessage(receiverUsername, current_user.username,null, 2);

            //Se non esistono ancora chat aperte OPPURE se non esistono chat dal sender (Ridondante, basterebbe il chatThread)
            //if (chatThread.Equals(null)) {

                //Messaggio nuovo creo nuovo thread con nuova finestra di chat MA NON LO ESEGUO
                try {
                    Thread chatThread = new Thread(new ParameterizedThreadStart(UpdateChat));
                    chatThread.Name = receiverUsername;
                    //chatThread.IsBackground = true;
                    chatThreads.Add(chatThread);
                    chatThread.Start(newMessage);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, $"Errore nella creazione della chat");
                }
            //}

            //Eseguo il thread che abbia come nome sender_username
            /*if (chatThread.Equals(null)) {
                MessageBox.Show($"Errore nella creazion della chat con l'utente {receiverUsername}");
            } else {
                chatThread.Start(null);
            }*/
        }

        private void StartTcpListenerThread()
        {
            listener = new TcpListener(IPAddress.Any, port);
            TcpClient client;
            listener.Start();
            LoadContactsCallback contactsCallback = new LoadContactsCallback(LoadContacts);
            tcpListenerThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        client = listener.AcceptTcpClient();
                        byte[] buffer = new byte[1024];
                        NetworkStream stream = client.GetStream();
                        int len = stream.Read(buffer, 0, buffer.Length);
                        
                        try         //prova a convertire cio che riceve in ircMessage, se funziona -> message box
                        {
                            #region oldChatSync
                            /*ircMessage newMessage = (ircMessage)ircMessage.BytesToObj(buffer, len);
                            if (chatList.Count != 0)
                            {
                                foreach (Chat chat in chatList)
                                {
                                    if (chat.Text == newMessage.sender_username)
                                    {
                                        chat.AddMessage(newMessage.message);
                                    }
                                    else
                                    {
                                        Chat newChat = new Chat(newMessage.sender_username, server_addr);
                                        newChat.Show();
                                        newChat.AddMessage(newMessage.message);
                                        chatList.Add(newChat);
                                    }
                                }
                            }
                            else {
                                Chat newChat = new Chat(newMessage.sender_username, server_addr);
                                newChat.AddMessage(newMessage.message);
                                newChat.ShowDialog();
                                chatList.Add(newChat);
                            }*/
                            #endregion

                            ircMessage newMessage = (ircMessage)ircMessage.BytesToObj(buffer, len);
                            //MessageBox.Show($"Ricevuto {newMessage.message} da {newMessage.sender_username}");

                            //Ottengo il thread che abbia il nome del sender, può ritornare null in caso non ci sia tale thread
                            Thread chatThread = chatThreads.Where(thread => thread.Name.Equals(newMessage.sender_username)).FirstOrDefault();

                            //Se non esistono ancora chat aperte OPPURE se non esistono chat dal sender (Ridondante, basterebbe il chatThread)
                            if (chatThreads.Count == 0 || chatThread.Equals(null)) {

                                //Messaggio nuovo creo nuovo thread con nuova finestra di chat MA NON LO ESEGUO
                                try {
                                    //chatThread = new Thread(() => UpdateChat(newMessage));
                                    chatThread = new Thread(new ParameterizedThreadStart(UpdateChat));
                                    chatThread.Name = newMessage.sender_username;
                                    //chatThread.IsBackground = true;

                                    chatThreads.Add(chatThread);
                                } catch (Exception ex) {
                                    MessageBox.Show(ex.Message,$"Errore nella creazione della chat");
                                }
                            }

                            //Eseguo il thread che abbia come nome sender_username
                            if (chatThread.Equals(null)) {
                                MessageBox.Show($"Errore nel recupero della chat dall'utente {newMessage.sender_username}");
                            } else {
                                //chatThread.Abort();
                                chatThread.Start(newMessage);
                            }


                        } catch       //prova a convertire cio che riceve in List<ircUser> , se funziona mostra quanti utenti online ci sono
                        {
                            online_users = (List<ircUser>)ircMessage.BytesToObj(buffer, len);
                            Invoke(contactsCallback);       //uso invoke perche devo chiamare il metodo LoadContacts che e' sul main thread
                        }
                    }
                    catch 
                    {
                        //MessageBox.Show(e.Message);
                    }
                }
            });
            tcpListenerThread.Start();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                client = new TcpClient(server_addr, port);

                ircMessage msg_logout = new ircMessage(current_user.username, 3);
                stream = client.GetStream();
                stream.Write(ircMessage.ObjToBytes(msg_logout), 0, ircMessage.ObjToBytes(msg_logout).Length);

                stream.Close();
                client.Close();
                listener.Stop();
                tcpListenerThread.Abort();

                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                stream.Close();
                client.Close();
            }

            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            tcpListenerThread.Abort();
            Application.Exit();
        }

        private void Home_Closing(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Chiusura Form Home");
        }
        
        public void UpdateChat(object obj) {

            ircMessage message = (ircMessage)obj;

            Chat chat;


            if (!chatNames.Contains(message.sender_username)) {
                chat = new Chat(message.sender_username, server_addr);
                chatNames.Add(message.sender_username);
            } else {
                chat = (Chat)Application.OpenForms[message.sender_username];
            }

            if (chat != null) {
                if (message.message != null) {
                    chat.AddMessage(message.message);
                }

                chat.ShowDialog();
            }
        }

        /// <summary>
        ///  Genere la GUI per la lista di utenti online
        /// </summary>
        /// <param users="Lista di Utenti online">
        ///     List<ircUser> contenente gli utenti online
        /// </param>
        private void LoadContacts() {
            flp_contacts.Controls.Clear();
            foreach (ircUser user in online_users) {
                if (user.username != current_user.username) {
                    Panel panel = new Panel {
                        Size = new Size(252, 48)
                    };

                    Label label_user = new Label {
                        Text = user.username,
                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                        Location = new Point(10, 10),
                        Size = new Size(70, 16)
                    };

                    Button btn = new Button {
                        Size = new Size(75, 34),
                        Tag = user,
                        Text = "Start Chat",
                        Location = new Point(165, 7)
                    };

                    btn.Click += new EventHandler(startChat_Button_Click);

                    Button btn_status = new Button {
                        Size = new Size(15, 15),
                        Location = new Point(12, 28),
                        BackColor = Color.LimeGreen
                    };

                    Label label_online = new Label {
                        Text = "Online",
                        Font = new Font("Microsoft Sans Serif", 8, FontStyle.Italic),
                        Location = new Point(33, 30),
                        Size = new Size(37, 13)
                    };

                    Label sep = new Label {
                        AutoSize = false,
                        Height = 2,
                        Width = 250,
                        BorderStyle = BorderStyle.Fixed3D
                    };

                    panel.Controls.Add(label_user);
                    panel.Controls.Add(btn);
                    panel.Controls.Add(btn_status);
                    panel.Controls.Add(label_online);

                    flp_contacts.Controls.Add(panel);
                    flp_contacts.Controls.Add(sep);
                }
            }
        }

        delegate void LoadContactsCallback();
    }
}
