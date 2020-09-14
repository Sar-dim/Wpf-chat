using SingIn;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Client
{
    public class Net : Messanger
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        public TcpClient client;
        public NetworkStream stream;
        public Thread receiveThread;
        public Net(string login)
        {
            client = new TcpClient();
            client.Connect(host, port); //подключение клиента
            stream = client.GetStream(); // получаем поток
            SendMessage(login);
            receiveThread = new Thread(new ThreadStart(ReceiveMessage));// запускаем новый поток для получения данных
            receiveThread.Start(); //старт потока
        }
        // отправка сообщений
        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        // получение сообщений
        public void ReceiveMessage()
        {
            int found = 0;
            string textMessage = "";
            string sender = "";
            string recepient = "";
            string date = "";
            string time = "";
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    if (message.Contains("NewMessage"))
                    {
                        message = message.Substring(10);
                        found = message.IndexOf("&");
                        sender = message.Substring(0, found);
                        message = message.Substring(found + 1);
                        found = message.IndexOf("&");
                        recepient = message.Substring(0, found);
                        message = message.Substring(found + 1);
                        found = message.IndexOf("&");
                        textMessage = message.Substring(0, found);
                        message = message.Substring(found + 1);
                        found = message.IndexOf("&");
                        date = message.Substring(0, found);
                        message = message.Substring(found + 1);
                        time = message;
                    }
                }
                catch
                {
                    Disconnect();//соединение было прервано
                }
                AddNewReceivedMessage(textMessage, sender, date, time);            
            }
        }
        public void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

    }
}
