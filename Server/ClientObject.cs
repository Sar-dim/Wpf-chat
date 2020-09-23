using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using DBConnection;

namespace Server
{
    class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal string UserName { get; private set; }
        TcpClient client;
        ServerObject server; // объект сервера
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                string originalMessage = "";
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                UserName = message;
                Console.WriteLine(UserName + " вошел в чат");
                // в бесконечном цикле получаем сообщения от клиента
                DBconnection dBconnection = new DBconnection();
                while (true)
                {
                    int found = 0;
                    string textMessage = "";
                    string sender = "";
                    string recepient = "";
                    string date = "";
                    string time = "";
                    try
                    {
                        message = GetMessage();
                        originalMessage = message;
                        Console.WriteLine($"{UserName}: {message}");
                        if (message.Contains("NewMessage"))
                        {
                            message = message.Substring(10);
                            Console.WriteLine($"{UserName}: {message}");
                            found = message.IndexOf("&");
                            sender = message.Substring(0, found);
                            Console.WriteLine($"sender: {sender}");
                            message = message.Substring(found + 1);
                            Console.WriteLine($"{UserName}: {message}");
                            found = message.IndexOf("&");
                            recepient = message.Substring(0, found);
                            Console.WriteLine($"recepient: {recepient}");
                            message = message.Substring(found + 1);
                            Console.WriteLine($"{UserName}: {message}");
                            found = message.IndexOf("&");
                            textMessage = message.Substring(0, found);
                            Console.WriteLine($"textMessage: {textMessage}");
                            message = message.Substring(found + 1);
                            Console.WriteLine($"{UserName}: {message}");
                            found = message.IndexOf("&");
                            date = message.Substring(0, found);
                            Console.WriteLine($"date: {date}");
                            message = message.Substring(found + 1);                           
                            time = message;
                            Console.WriteLine($"time: {time}");
                            server.SendMessage(originalMessage, recepient);
                            dBconnection.ConnectDB();
                            string query = $"INSERT INTO Messages(sender, recepient, text, date, time) VALUES ('{sender}', '{recepient}', " +
                                         $"'{textMessage}', '{date}', '{time}');";
                            dBconnection.InsertQuery(query);
                            dBconnection.ForceClose();
                        }
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", UserName);
                        Console.WriteLine(message);
                        break;
                    }  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);
            if (builder.ToString() == "")
            {
                server.RemoveConnection(this.Id);
                Close();
            }
            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
