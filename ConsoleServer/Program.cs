using LibraryCustomClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServer
{
    internal class Program
    {
        static Dictionary<string, CustomClient> _allClients;
        static void Main(string[] args)
        {
            _allClients = new Dictionary<string, CustomClient>();

            Console.WriteLine("This is Server");
            var tcpListener = new TcpListener(IPAddress.Any, 443);

            Console.WriteLine("Press enter to start Server");
            Console.ReadLine();

            tcpListener.Start();
            Console.WriteLine("Server started successfully " + $"{tcpListener.Server.LocalEndPoint}");

            while(true)
            {
                Console.WriteLine("Waiting incoming connection...");
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                Console.WriteLine($"Clinet {tcpClient.Client.RemoteEndPoint}" + $" connecting successfully!");

                var customClient = new CustomClient(tcpClient, true);

                customClient.ClientLogin += CustomClient_ClientLogin;
                customClient.ReceiveMessage += CustomClient_ReceiveMessage;
                customClient.StartService();
            }
        }

        private static void CustomClient_ReceiveMessage(MessageProtocol messageReceiveProtocol, CustomClient customClient)
        {
            {
                Console.WriteLine($"Receive message: {messageReceiveProtocol.Sender} > {messageReceiveProtocol.Text}");

                var receivers = _allClients.Keys.Where(k => k != customClient.UserName).Select(k => _allClients[k]).ToList();

                var messageProtocol = new MessageProtocol()
                {
                    MessageType = MessageType.RegularMessage,
                    Receiver = string.Empty,
                    Sender = customClient.UserName,
                    Text = messageReceiveProtocol.Text
                };

                receivers.ForEach(r => r.SendMessageProtocol(messageProtocol));
            }
        }

        private static void CustomClient_ClientLogin(string userName, CustomClient customClient)
        {
            Console.WriteLine($"Client Login: Username '{userName}' " + $"Address = {customClient.Address}");
            _allClients.Add(userName, customClient);
        }

    }
}
