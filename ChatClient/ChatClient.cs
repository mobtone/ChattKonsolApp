using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
     class ChatClient
    {
        static void Main(string[] args)
        {
            Console.Write("Ange serverns IP och port (t.ex. 127.0.0.1:8888):");
            string input = Console.ReadLine();
            string[] parts = input.Split(':'); //användaren skriver in IP och Port som inbjudningslänk till servern
            string serverIp = parts[0];     //Detta lagras i en array och siffrorna delas vid :kolon och lagrar i array med index 0 och 1
            int port = int.Parse(parts[1]);

            TcpClient client = new TcpClient();
            client.Connect(serverIp, port);

            NetworkStream stream = client.GetStream();
            Thread recieveThread = new Thread(() => RecieveMessage(stream));
            recieveThread.Start();

            Console.WriteLine("Ansluten! Skriv ditt namn:");
            string userName = Console.ReadLine();
            Console.WriteLine("Börja chatta!");

            while (true)
            {
                string message = Console.ReadLine();
                string fullMessage = $"{userName}: {message}";
                byte[] buffer = Encoding.UTF8.GetBytes(fullMessage);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        static void RecieveMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
        }
    }
}
