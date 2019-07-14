using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*
            Form loginForm = new Login();
            Form regForm = new Register();

            bool loop = true;
            while (loop)
            {
                if (loginForm.ShowDialog() == DialogResult.Yes) {

                    if (regForm.ShowDialog() == DialogResult.Yes) { }
                    else
                        loop = false;
                }
                else
                    loop = false;
            }
            */
            Application.Run(new Home("192.168.0.107"));
        }
    }
}
