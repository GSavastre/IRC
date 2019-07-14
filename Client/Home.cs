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

namespace Client
{
    public partial class Home : Form
    {
        ircUser current_user;
        List<ircUser> online_users;

        public Home(/*ircUser myCurrent_user, List<ircUser> myOnline_users*/)
        {
            InitializeComponent();
            //current_user = myCurrent_user;
            //online_users = myOnline_users;

           /* current_user = new ircUser(0, "Dax");
            online_users = new List<ircUser> {
                new ircUser(1, "Loca"),
                new ircUser(2, "Sava")
            };*/

            l_user.Text = current_user.username;

            LoadContacts(online_users);
        }

        
        /// <summary>
        ///  Genere la GUI per la lista di utenti online
        /// </summary>
        /// <param users="Lista di Utenti online">
        ///     List<ircUser> contenente gli utenti online
        /// </param>
        void LoadContacts(List<ircUser> users) {
            foreach (ircUser user in online_users) {
                Panel panel = new Panel();
                panel.Size = new Size(252, 48);

                Label label_user = new Label();
                label_user.Text = user.username;
                label_user.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
                label_user.Location = new Point(10, 10);
                label_user.Size = new Size(70, 16);

                Button btn = new Button();
                btn.Size = new Size(75, 34);
                btn.Tag = user;
                btn.Text = "Start Chat";
                btn.Location = new Point(165, 7);
                
                Button btn_status = new Button();
                btn_status.Size = new Size(15, 15);
                btn_status.Location = new Point(12, 28);
                btn_status.BackColor = Color.LimeGreen;

                Label label_online = new Label();
                label_online.Text = "Online";
                label_online.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Italic);
                label_online.Location = new Point(33, 30);
                label_online.Size = new Size(37, 13);

                Label sep = new Label();
                sep.AutoSize = false;
                sep.Height = 2;
                sep.Width = 250;
                sep.BorderStyle = BorderStyle.Fixed3D;
                
                panel.Controls.Add(label_user);
                panel.Controls.Add(btn);
                panel.Controls.Add(btn_status);
                panel.Controls.Add(label_online);

                flp_contacts.Controls.Add(panel);
                flp_contacts.Controls.Add(sep);
            }
        }
    }
}
