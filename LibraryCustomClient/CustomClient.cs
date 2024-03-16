using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryCustomClient
{
    public class CustomClient
    {
        public event Action<string, CustomClient> ClientLogin;

        public event Action<MessageProtocol, CustomClient> ReceiveMessage;

        public bool isOnline {  get; set; }

        protected bool _isOnServer;

        protected TcpClient _tcpClient;
        protected NetworkStream _stream;

        protected StreamReader _sr;
        protected StreamWriter _sw;

        protected Thread _threadWaitingMessages;

        public string Address => _tcpClient.Client.RemoteEndPoint.ToString();

        public string UserName {  get; set; }

        public void SendMessageProtocol(MessageProtocol messageProtocol)
        {
            var json = JsonConvert.SerializeObject(messageProtocol);

            _sw.WriteLine(json);
            _sw.Flush();
        }

        public void SendLogin(String userName, string password) 
        {
            UserName = userName;
            var messageProtocol = new MessageProtocol()
            {
                MessageType = MessageType.LoginMessage,
                Sender = userName,
                Receiver = null,
                Text = password
            };
            SendMessageProtocol(messageProtocol);
        }

        public void SendPublicMessage(string text)
        {
            var messageProtocol = new MessageProtocol()
            {
                MessageType = MessageType.RegularMessage,
                Sender = UserName,
                Receiver = null,
                Text = text
            };
            SendMessageProtocol(messageProtocol);
        }

        public CustomClient(TcpClient tcpClient, bool isOnServer) 
        {
            _isOnServer = isOnServer;
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _sr = new StreamReader(_stream);
            _sw = new StreamWriter(_stream);
            _threadWaitingMessages = new Thread(WaitingMessages);
        }

        public void StartService()
        {
            isOnline = true;
            _threadWaitingMessages.Start();
        }

        public MessageProtocol ConvertToMessageProtocol(string text)
        {
            return JsonConvert.DeserializeObject<MessageProtocol>(text);
        }

        public void WaitingMessages()
        {
            while (isOnline)
            {
                string receivedText = "";
                try
                {
                    receivedText = _sr.ReadLine();
                }
                catch (Exception ex) 
                {
                    isOnline = false;
                    Console.WriteLine($"Catch Error: {ex.Message}");
                }

                if(string.IsNullOrEmpty(UserName)) 
                {
                    var messageProtocol = ConvertToMessageProtocol(receivedText);
                    UserName = messageProtocol.Sender;

                    Console.WriteLine($"Got Login: Username {messageProtocol.Sender}" + $"Password: {messageProtocol.Text}");

                    ClientLogin?.Invoke(UserName, this);
                }
                else
                {
                    if(!string.IsNullOrEmpty(receivedText)) 
                    {
                        if(_isOnServer)
                        {
                            Console.WriteLine($"{UserName}: '{receivedText}'");
                        }
                        var messageProtocol = ConvertToMessageProtocol(receivedText);

                        ReceiveMessage?.Invoke(messageProtocol, this);
                    }
                }
            }
        }

    }
}
