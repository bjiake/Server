using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpServer
{
    class Program : DealCards
    {
        static int port = 2115; // порт для приема входящих запросов
        static string HostName = "127.0.0.1";// Порт сервера 176.196.126.194 
        public static int stage = 1;
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
        public static void RecieveApplyData(Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных

            do
            {
                bytes = Player.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Player.Available > 0);

            Console.WriteLine("Ответ клиента:" + builder.ToString());
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
        public static void sleep()
        {
            Thread.Sleep(2000);
        }
        public static void SendCards(Card[] card, Socket Player)
        {
            for (int i = 0; i < 2; i++)
            {
                SendSuitOfTableForClient(card[i], Player);
            }
            //Console.WriteLine("Данные масти игроку отправлены");
            sleep();

            for (int i = 0; i < 2; i++)
            {
                SendValueCardOfTableForClient(card[i], Player);
            }
            //Console.WriteLine("Данные значения карты игроку отправлены");
            sleep();
        }
        public static void SendFlope(Card[] card, Socket Player)
        {
            stage++;//2
            for (int i = 0; i < 3; i++)
            {
                SendSuitOfTableForClient(card[i], Player);
            }
            //Console.WriteLine("Данные масти флопа отправлены");
            sleep();
            for (int i = 0; i < 3; i++)
            {
                SendValueCardOfTableForClient(card[i], Player);
            }
            //Console.WriteLine("Данные значения карты флопа отправлены");
            sleep();
        }
        public static void SendTurn(Card[] card, Socket Player)
        {
            stage++;//3
            SendSuitOfTableForClient(card[stage], Player);

            //Console.WriteLine("Данные масти игроку отправлены");
            sleep();

            SendValueCardOfTableForClient(card[stage], Player);

            //Console.WriteLine("Данные значения карты игроку отправлены");
            sleep();
        }

        public static void SendRiver(Card[] card, Socket Player)
        {
            stage++;//4
            SendSuitOfTableForClient(card[stage], Player);

            //Console.WriteLine("Данные масти игроку отправлены");
            sleep();

            SendValueCardOfTableForClient(card[stage], Player);

            //Console.WriteLine("Данные значения карты игроку отправлены");
            sleep();
        }

        static void Main()
        {
            Console.Title = "Блэйк Джек сервер";

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
                    Socket PlayerTwo = listenSocket.Accept();

                    SendPlayerData(message, Player, PlayerTwo);
                    message = "Игроки подключились!\n";
                    SendPlayerData(message, Player, PlayerTwo);
                    message = "Вы 1 игрок\n";
                    SendAlonePlayerData(message, Player);
                    message = "Вы 2 игрок\n";
                    SendAlonePlayerData(message, PlayerTwo);

                    DealCards DealCard = new DealCards();
                    DealCard.Deal();

                    
                    SendCards(DealCard.PlayerHand, Player);
                    SendCards(DealCard.PlayerHand, PlayerTwo);
                    DealCard.DisplayPlayersCard();

                    SendFlope(DealCard.TableCards, Player);
                    SendFlope(DealCard.TableCards, PlayerTwo);
                    DealCard.DisplayFlope();

                    SendTurn(DealCard.TableCards, Player);
                    SendFlope(DealCard.TableCards, PlayerTwo);
                    DealCard.DisplayTern();

                    SendRiver(DealCard.TableCards, Player);
                    SendFlope(DealCard.TableCards, PlayerTwo);
                    DealCard.DisplayRiver();

                    // получаем сообщение
                    //RecievePlayerData(Player);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendValueCardOfTableForClient(Card card, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string CardValue = " ";
            switch (card.MyValue)
            {
                case Card.VALUE.Ace:
                    CardValue = "24";
                    break;
                case Card.VALUE.King:
                    CardValue = "23";
                    break;
                case Card.VALUE.Queen:
                    CardValue = "22";
                    break;
                case Card.VALUE.Jack:
                    CardValue = "21";
                    break;
                case Card.VALUE.Ten:
                    CardValue = "20";
                    break;
                case Card.VALUE.Nine:
                    CardValue = "19";
                    break;
                case Card.VALUE.Eight:
                    CardValue = "18";
                    break;
                case Card.VALUE.Seven:
                    CardValue = "17";
                    break;
                case Card.VALUE.Six:
                    CardValue = "16";
                    break;
                case Card.VALUE.Five:
                    CardValue = "15";
                    break;
                case Card.VALUE.Four:
                    CardValue = "14";
                    break;
                case Card.VALUE.Three:
                    CardValue = "13";
                    break;
                case Card.VALUE.Two:
                    CardValue = "12";
                    break;
            }

            data = Encoding.Unicode.GetBytes(CardValue);
            Player.Send(data);
        }

        public static void SendSuitOfTableForClient(Card card, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string CardSuit = " ";
            switch (card.MySuit)
            {
                //Массив с 1 элементом, значения масти по коду CP437, Вывод
                case Card.SUIT.Hearts:
                    CardSuit = "0";// Hearts
                    break;
                case Card.SUIT.Diamonds:
                    CardSuit = "2";// Diamonds
                    break;
                case Card.SUIT.Clubs:
                    CardSuit = "1"; // Spades
                    break;
                case Card.SUIT.Spades:
                    CardSuit = "3"; // Clubs
                    break;
            }

            data = Encoding.Unicode.GetBytes(CardSuit);
            Player.Send(data);

            builder.Clear();
            data = null;
            //Player.Send(data);
            //builder.Clear();
            //data = null;
        }
    }
}