using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Server
    {
        public Server()
        {
            TcpListener server = null;
            try
            {
                // Settiamo tcpListener con indirizzo e porta 7777.
                Int32 port = 7777;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                
                server = new TcpListener(localAddr, port);  // TcpListener server = new TcpListener(port);

                server.Start();  // Start server per richieste clietn.

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Loop di ascolto
                while (true)
                {
                    
                    Console.Write("Socket-->"+ " Indirizzo: "+localAddr+ " Port: "+ port + "\n\nWaiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();    //Si accetta la richiesta del client
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();  //Oggeto stream per leggere e scrivere

                    int i;
                    // Loop per ricevere tutti i dati inviati dal client
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) //Se ricevo qualcosa leggiamo il messaggio
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i); //Conversione da byte a ASCII
                        Console.WriteLine("Received: {0}", data);

                        
                        data = data.ToUpper();  // stringa in Upper case

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Ritorna un messaggio di conferma al client
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();

        }

    }

    
}
