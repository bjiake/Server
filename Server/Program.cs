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
        public static int stage = 2;
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
        public static void sleep()
        {
            Thread.Sleep(500);
        }
        public static void SendCards(Card[] card, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";      
            for(int i = 0; i < 2; i++) { Cards = ConvertSuit(card[i], Cards); }
            for (int i = 0; i < 2; i++) { Cards = ConvertValue(card[i], Cards); }
            //Console.WriteLine(Cards);
            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);

            Cards = null;
            data = null;
            builder.Clear();
        }
        public static void SendFlope(Card[] card, Socket Player, Socket PlayerTwo)
        {
            stage++;//3
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";
            for (int i = 0; i < 3; i++)
            {
                Cards = ConvertSuit(card[i], Cards);
            }
            for(int i = 0; i < 3; i ++)
            {
                Cards = ConvertValue(card[i], Cards);
            }

            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);
            PlayerTwo.Send(data);

            data = null;
            Cards = null;
            builder.Clear();
        }
        public static void SendTurn(Card card, Socket Player, Socket PlayerTwo)
        {
            //stage 3
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = null;
            Cards = ConvertSuit(card, Cards);
            Cards = ConvertValue(card, Cards);

            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);
            PlayerTwo.Send(data);

            data = null;
            Cards = null;
            builder.Clear();
            stage++;//4
        }

        public static void SendRiver(Card card, Socket Player, Socket PlayerTwo)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = null;
            Cards = ConvertSuit(card, Cards);
            Cards = ConvertValue(card, Cards);

            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);
            PlayerTwo.Send(data);

            data = null;
            Cards = null;
            builder.Clear();
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

                    //SendPlayerData(message, Player, PlayerTwo);
                    //message = "Игроки подключились!\n";
                    //SendPlayerData(message, Player, PlayerTwo);
                    //message = "Вы 1 игрок\n";
                    //SendAlonePlayerData(message, Player);
                    //message = "Вы 2 игрок\n";
                    //SendAlonePlayerData(message, PlayerTwo);

                    DealCards DealCard = new DealCards();
                    DealCard.Deal();

                    DealCard.DisplayPlayerCard();
                    DealCard.DisplayPlayerTwoCard();
                    //stage 2
                    SendCards(DealCard.PlayerHand, Player);
                    SendCards(DealCard.PlayerTwoHand, PlayerTwo);

                    //stage 3
                    DealCard.DisplayFlope();
                    SendFlope(DealCard.TableCards, Player, PlayerTwo);
                    sleep();

                    //stage 3
                    DealCard.DisplayTurn();
                    SendTurn(DealCard.TableCards[stage], Player, PlayerTwo);//ОШИБКА ОТПРАВЛЕНИЯ ТЕРНА
                    sleep();
                    //stage 4
                    DealCard.DisplayRiver();
                    SendRiver(DealCard.TableCards[stage], Player, PlayerTwo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public static string ConvertSuit(Card card, string Cards)
        {

            switch (card.MySuit)
            {
                //Массив с 1 элементом, значения масти по коду CP437, Вывод
                case Card.SUIT.Hearts:
                    Cards = Cards + "0";// Hearts
                    break;

                case Card.SUIT.Clubs:
                    Cards = Cards + "1"; // Spades
                    break;

                case Card.SUIT.Diamonds:
                    Cards = Cards + "2";// Diamonds
                    break;

                case Card.SUIT.Spades:
                    Cards = Cards + "3"; // Clubs
                    break;
            }
            return Cards;
        }
        public static string ConvertValue(Card card, string Cards)
        {

            switch (card.MyValue)
            {
                case Card.VALUE.Ace:
                    Cards = Cards + "24";
                    break;
                case Card.VALUE.King:
                    Cards = Cards + "23";
                    break;
                case Card.VALUE.Queen:
                    Cards = Cards + "22";
                    break;
                case Card.VALUE.Jack:
                    Cards = Cards + "21";
                    break;
                case Card.VALUE.Ten:
                    Cards = Cards + "20";
                    break;
                case Card.VALUE.Nine:
                    Cards = Cards + "19";
                    break;
                case Card.VALUE.Eight:
                    Cards = Cards + "18";
                    break;
                case Card.VALUE.Seven:
                    Cards = Cards + "17";
                    break;
                case Card.VALUE.Six:
                    Cards = Cards + "16";
                    break;
                case Card.VALUE.Five:
                    Cards = Cards + "15";
                    break;
                case Card.VALUE.Four:
                    Cards = Cards + "14";
                    break;
                case Card.VALUE.Three:
                    Cards = Cards + "13";
                    break;
                case Card.VALUE.Two:
                    Cards = Cards + "12";
                    break;
            }

            return Cards;
        }
    }
}