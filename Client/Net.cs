using SingIn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public static class Net
    {
        // отправка сообщений
        public static void SendMessage(string message, NetworkStream stream)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        
        // получение нового сообщения
        public async static Task<string> ReceiveNewMessage(NetworkStream stream, TcpClient client)
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = (await stream.ReadAsync(data, 0, data.Length));
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            string message = builder.ToString();
            if (message.Contains("NewMessage"))
            {
                return message;
            }
            else
            {
                return "Error";
            }
        }
        public static void Disconnect(NetworkStream stream, TcpClient client)
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

    }
}
