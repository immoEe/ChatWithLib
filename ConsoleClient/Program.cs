using LibraryCustomClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is Client");

            var tcpClient = new TcpClient();
            Console.WriteLine("Enter something to connect to server");
            Console.ReadLine();

            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 443);
            Console.WriteLine("Successfully connecting");

            var customClient = new CustomClient(tcpClient, false);
            customClient.ReceiveMessage += CustomClient_ReciveMessages;
            customClient.StartService();

            Console.Write("Please enter login > ");
            string login = Console.ReadLine();
            Console.Write("Please enter password > ");
            string password = Console.ReadLine();
            customClient.SendLogin(login, password);

            while (true)
            {
                Console.Write($"Please input message > ");
                string message = Console.ReadLine();
                customClient.SendPublicMessage(message);
            }

        }


        private static void CustomClient_ReciveMessages(MessageProtocol messageReceiveProtocol, CustomClient customClient)
        {
            Console.WriteLine();
            Console.WriteLine($"\t\t\t\tGot message from: {messageReceiveProtocol.Sender} > {messageReceiveProtocol.Text}");

        }
    }
}
