using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ChattKonsolApp
{
    class ChatServer
    {
        //detta är fält som definierar en lista av tcp-klienter (transmission control protocol) för att kunna 
        //kommunicera över internet. dataöverföringsprotokoll.
        //delivers a stream of bytes between applications running on hosts communicating via an IP network

        //TcpListener finns i biblioteket System.Net.Sockets
        //Listens for connections from TCP network clients
        //Sedan definieras fältet för porten som servern ska köras på

        private static List<TcpClient> clients = new List<TcpClient>();
        private static TcpListener server;
        private const int port = 8888;

        static void Main()
        {

            server = new TcpListener(IPAddress.Any, port);

            server.Start(); //servern lyssnar på klient-requests, när användaren i klienten skriver in IP-adress
                            //följt av porten så godkäns användarens förfrågan att ansluta till servern

            Console.WriteLine($"Server startad på {GetLocalIPAddress()}:{port}");
            Console.WriteLine($"Inbjudningslänk: {GetLocalIPAddress()}:{port}");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                lock (clients) clients.Add(client);
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj; //nytt objekt, instans av klassen TcpClient
            NetworkStream stream = client.GetStream(); //tar emot network stream från klienten (chatClient)
            byte[] buffer = new byte[1024]; //skapar en ny array av bytes, 8bitars värden från 0 till 255. 
                                             //new byte [1024] betyder att vi allockerar minne för 1024 bytes (1 KB), alltså hur mycket plats som ska finnas på servern
                                            //används ofra som en buffert för att ta emot data från nätverket.
                                            //när data tas emot från nätverket fylles buffer-arrayen med inkommande bytes
            int bytesRead; //en variabel som lagrar siffor för att läsa av antal bytes i streamen? 

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
                BroadcastMessage(message, client);
            }

            lock (clients) clients.Remove(client);
            client.Close();
        }

        static void BroadcastMessage(string message, TcpClient sender)
        {
            lock (clients)
            {
                foreach (var client in clients)
                {
                    if (client != sender)
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = Encoding.UTF8.GetBytes(message);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        static string GetLocalIPAddress()
        {
            string localIP = "127.0.0.1";
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            return localIP;
        }

    }
}
