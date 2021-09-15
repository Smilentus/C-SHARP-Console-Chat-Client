using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ConsoleChatClient
{
    class Program
    {
        static string userName; // Имя подключившегося
        static string host = "127.0.0.1"; // Айпи подключения --> куда
        private const int port = 7777; // Порт

        static TcpClient client; // Наш клиент
        static NetworkStream stream; // Сетевой поток

        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в чат!");
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            Console.Write("Введите IP-адрес: ");
            host = Console.ReadLine();
            Console.WriteLine("Подключение к " + host + "...");

            client = new TcpClient();

            try
            {
                client.Connect(host, port); // Подключение клиента
                stream = client.GetStream(); // Получаем поток

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // Запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); // Старт потока
                Console.WriteLine("У нас новенький --> {0}", userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                // В случае ошибки предупреждаем
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect(); // В итоге отключаемся
            }
        }

        // Отправка сообщений
        /// <summary>
        /// 0 - Сообщение человека. 1 - Информация
        /// </summary>
        /// <param name="messageType"></param>
        static void SendMessage()
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }

        // Получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // Буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);// Вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); // Соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();// Отключение потока
            if (client != null)
                client.Close();// Отключение клиента
            Environment.Exit(0); // Завершение процесса
        }
    }
}
