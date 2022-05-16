using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer
{
    class Program : DealCards
    {
        static int port = 2115; // порт для приема входящих запросов
        static string HostName = "127.0.0.1";// Порт сервера 176.196.126.194 
        //Порт локальной 127.0.0.1

        public static void RecievePlayerData(Socket Player)
        {
            string message = "Ждём игроков";
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных
            do
            {
                bytes = Player.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Player.Available > 0);

            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

            // отправляем ответ
            message = "ваше сообщение доставлено";
            data = Encoding.Unicode.GetBytes(message);
            Player.Send(data);
            data = null;
            bytes = 0;
            builder.Clear();
        }
        
        public static void SendPlayerData(string message, Socket Player, Socket PlayerTwo)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            data = Encoding.Unicode.GetBytes(message);
            Player.Send(data);
            PlayerTwo.Send(data);
            builder.Clear();
            data = null;
        }

        public static void SendAlonePlayerData(string message, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            data = Encoding.Unicode.GetBytes(message);
            Player.Send(data);
            builder.Clear();
            data = null;
        }


        static void Main()
        {
            Console.Title = "Блэйк Джек";

            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Clear();

            Console.BufferWidth = 120;
            Console.WindowWidth = Console.BufferWidth;
            Console.WindowHeight = 40;
            Console.BufferHeight = Console.WindowHeight;

            

            




            // получаем адреса для запуска сокета
            var IpPoint = new IPEndPoint(IPAddress.Parse(HostName), port);

            // создаем сокет
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(IpPoint);

                // начинаем прослушивание очередь входящих потоков
                listenSocket.Listen(5);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    string message = "Ждём игроков\n";


                    Socket Player = listenSocket.Accept();

                    message = "Игрок 1 подключился\n";
                    SendAlonePlayerData(message, Player);
                    Socket PlayerTwo = listenSocket.Accept();

                    SendPlayerData(message, Player, PlayerTwo);
                    message = "Игрок 2 подключился\n";
                    SendPlayerData(message, Player, PlayerTwo);

                    DealCards DealCard = new DealCards();
                    DealCard.Deal();

                    Console.Clear();//Остановился на ошибке TableDeck, который пустой
                    SendCardsOfTableForClient(Player);
                    SendCardsOfTableForClient(PlayerTwo);
                    //Console.ReadLine();

                    // получаем сообщение
                    RecievePlayerData(Player);
                    RecievePlayerData(PlayerTwo);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendCardsOfTableForClient(Socket Player)
        {
            DealCards dealCards = new();
            for (int i = 0; i < 3; i++)
            {
                Player.Send(SendSuitOfTableForClient(dealCards.TableCards[i], Player));
                Player.Send(SendValueCardOfTableForClient(dealCards.TableCards[i], Player));
                //Остановился на передаче значения карты
            }
        }

        public static byte[] SendValueCardOfTableForClient(Card card, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string CardValue = " ";
            switch (card.MyValue)
            {
                case Card.VALUE.Ace:
                    CardValue = "Ace";
                    break;
                case Card.VALUE.King:
                    CardValue = "King";
                    break;
                case Card.VALUE.Queen:
                    CardValue = "Queen";
                    break;
                case Card.VALUE.Jack:
                    CardValue = "Jack";
                    break;
                case Card.VALUE.Ten:
                    CardValue = "Ten";
                    break;
                case Card.VALUE.Nine:
                    CardValue = "Nine";
                    break;
                case Card.VALUE.Eight:
                    CardValue = "Eight";
                    break;
                case Card.VALUE.Seven:
                    CardValue = "Seven";
                    break;
                case Card.VALUE.Six:
                    CardValue = "Six";
                    break;
                case Card.VALUE.Five:
                    CardValue = "Five";
                    break;
                case Card.VALUE.Four:
                    CardValue = "Four";
                    break;
                case Card.VALUE.Three:
                    CardValue = "Three";
                    break;
                case Card.VALUE.Two:
                    CardValue = "Two";
                    break;
            }

            data = Encoding.Unicode.GetBytes(CardValue);
            return data;
        }

        public static byte[] SendSuitOfTableForClient(Card card, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string CardSuit = " ";
            switch (card.MySuit)
            {
                //Массив с 1 элементом, значения масти по коду CP437, Вывод
                case Card.SUIT.Hearts:
                    CardSuit = "\u2665";// Hearts
                    break;
                case Card.SUIT.Diamonds:
                    CardSuit = "\u2666";// Diamonds
                    break;
                case Card.SUIT.Clubs:
                    CardSuit = "\u2660"; // Spades
                    break;
                case Card.SUIT.Spades:
                    CardSuit = "\u2663"; // Clubs
                    break;
            }

            data = Encoding.Unicode.GetBytes(CardSuit);
            return data;
            //Player.Send(data);
            //builder.Clear();
            //data = null;
        }
    }
}