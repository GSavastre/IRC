﻿using System;
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
        //porta predefinita del server
        const int port = 7777;                      
        string server_addr = "";
        //utente attuale (che ha fatto login)
        public static ircUser current_user;
        //lista utenti online
        List<ircUser> online_users = null;
        
        TcpListener listener = null;
        Thread tcpListenerThread = null;

        //lista di chat attive
        List<ChatBox> chatList = new List<ChatBox>();
    
        public Home(string myServer_addr, ircUser myCurrent_user, List<ircUser> myOnline_users)
        {
            InitializeComponent();
            server_addr = myServer_addr;
            current_user = myCurrent_user;
            online_users = myOnline_users;
            
            l_user.Text = current_user.username;

            LoadContacts();
            StartTcpListenerThread();
        }

        /// <summary>
        ///  Genera la GUI per la lista di utenti online
        /// </summary>
        /// <param users="Lista di Utenti online">
        ///     List<ircUser> contenente gli utenti online
        /// </param>
        private void LoadContacts() {
            flp_contacts.Controls.Clear();
            foreach (ircUser user in online_users) {
                //aggiungo nuovo utente alla lista(tranne me stesso)
                //creazione dinamica dei bottoni nella lista degli utenti attivi
                if (user.username != current_user.username)
                {
                    Panel panel = new Panel
                    {
                        Size = new Size(252, 48)
                    };

                    Label label_user = new Label
                    {
                        Text = user.username,
                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                        Location = new Point(10, 10),
                        Size = new Size(70, 16)
                    };

                    Button btn = new Button
                    {
                        Size = new Size(75, 34),
                        Tag = user,
                        Text = "Start Chat",
                        Location = new Point(165, 7)
                    };

                    btn.Click += new EventHandler(startChat_Button_Click);

                    Button btn_status = new Button
                    {
                        Size = new Size(15, 15),
                        Location = new Point(12, 28),
                        BackColor = Color.LimeGreen
                    };

                    Label label_online = new Label
                    {
                        Text = "Online",
                        Font = new Font("Microsoft Sans Serif", 8, FontStyle.Italic),
                        Location = new Point(33, 30),
                        Size = new Size(37, 13)
                    };

                    Label sep = new Label
                    {
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

        //Funzioni delegate da richiamare con la funzione Invoke da un altro Thread
        delegate void LoadContactsCallback();
        delegate void CreateChatCallback(ircMessage msg, string server_addr);
        delegate void UpdateChatCallback(ChatBox chatBox, ircMessage msg);

        /// <summary>
        ///  Handler dinamico per instanziare nuove chat.
        /// </summary>
        private void startChat_Button_Click(object sender, EventArgs e)
        {
            Button partner_button = sender as Button;
            ChatBox chat = new ChatBox(((ircUser)partner_button.Tag).username, server_addr, this);
            chatList.Add(chat);
            chat.Show();
        }

        /// <summary>
        ///  Avvia un thread per l'ascolto di messaggi dal Server
        /// </summary>
        private void StartTcpListenerThread()
        {
            //istanzio il listner
            listener = new TcpListener(IPAddress.Any, port);
            TcpClient client;
            listener.Start();
            //creo collegamento tra le funzioni di callback e le funzioni del thread main da richiamare
            LoadContactsCallback contactsCallback = new LoadContactsCallback(LoadContacts);
            CreateChatCallback createChatCallback = new CreateChatCallback(CreateChatBox);
            UpdateChatCallback updateChatCallBack = new UpdateChatCallback(UpdateChat);
            //creo nuovo thread
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
                        
                        try         //prova a convertire cio che riceve in ircMessage, se funziona -> message box, altrimenti va in catch
                        {
                            ircMessage newMessage = (ircMessage)ircMessage.BytesToObj(buffer, len);
                            /*Controllo se la lista di chat e' vuota , vado direttamente nell else e creo la chat
                             *Se esistono gia' chat, ciclo la lista di chat finche' non trovo quella con l utente da cui mi arriva il messaggio 
                             *Aggiorno visualmente la chat
                             */
                            if (chatList.Count != 0)
                            {
                                bool found = false;
                                foreach (ChatBox cb in chatList)
                                {
                                    if (cb.Text == newMessage.sender_username)
                                    {
                                        Invoke(updateChatCallBack, cb, newMessage);
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    Invoke(createChatCallback, newMessage, server_addr);
                                }
                            }
                            else
                            {
                                Invoke(createChatCallback,  newMessage, server_addr);
                            }
                        }
                        catch       //prova a convertire cio che riceve in List<ircUser> , se funziona mostra quanti utenti online ci sono
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
            //avvio il thread di ascolto
            tcpListenerThread.Start();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Invio ircMessage con l'istruzione di logout e chiudo il form
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
            //chiudo thread ed esco dall'applicativo
            tcpListenerThread.Abort();
            Application.Exit();
        }

        /// <summary>
        ///  Crea nuova chat in base al messaggio che mi arriva
        /// </summary>
        private void CreateChatBox(ircMessage msg, string server_addr)
        {
            ChatBox chatBox = new ChatBox(msg.sender_username, server_addr, this);
            chatBox.AddMessage(msg.sender_username + " : " + msg.message);
            chatList.Add(chatBox);
            chatBox.Show();
        }

        /// <summary>
        ///  Aggiorna chat esistente
        /// </summary>
        private void UpdateChat(ChatBox chatBox, ircMessage msg)
        {
            chatBox.AddMessage(msg.sender_username + " : " + msg.message);
        }

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
                default:
                    listener.Stop();
                    tcpListenerThread.Abort();
                    break;
            }
        }

        /// <summary>
        ///  Rimuove chat dalla lista di chat attive
        /// </summary>
        public void EndChat(ChatBox chatBox)
        {
            chatList.Remove(chatBox);
        }

    }
}
