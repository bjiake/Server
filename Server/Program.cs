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
            Thread.Sleep(500);
        }
        public static void SendCards(Card[] card, Socket Player)
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";
            Cards = ConvertSuit(card, Cards);
            Cards = ConvertValue(card, Cards);
            Console.WriteLine(Cards);
            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);

            Cards = null;
            data = null;
            builder.Clear();
            //sleep();
            //for (int i = 0; i < 2; i++)
            //{
            //    SendSuitOfTableForClient(card[i], Player);
            //}
            ////Console.WriteLine("Данные масти игроку отправлены");
            //sleep();

            //for (int i = 0; i < 2; i++)
            //{
            //    SendValueCardOfTableForClient(card[i], Player);
            //}
            ////Console.WriteLine("Данные значения карты игроку отправлены");

        }
        public static void SendFlope(Card[] card, Socket Player, Socket PlayerTwo)
        {
            stage++;//3
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";
            Cards = ConvertSuit(card, Cards);
            Cards = ConvertValue(card, Cards);

            Console.WriteLine(Cards);

            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);
            PlayerTwo.Send(data);


            data = null;
            Cards = null;
            builder.Clear();
            //sleep();
            //for (int i = 0; i < 3; i++)
            //{
            //    SendSuitOfTableForClient(card[i], Player);
            //}
            ////Console.WriteLine("Данные масти флопа отправлены");
            //sleep();
            //for (int i = 0; i < 3; i++)
            //{
            //    SendValueCardOfTableForClient(card[i], Player);
            //}
            ////Console.WriteLine("Данные значения карты флопа отправлены");

        }
        public static void SendTurn(Card[] card, Socket Player, Socket PlayerTwo)
        {
            //stage 3
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";
            Cards = ConvertSuit(card, Cards);
            Cards = ConvertValue(card, Cards);

            Console.WriteLine(Cards);

            data = Encoding.Unicode.GetBytes(Cards);
            Player.Send(data);
            PlayerTwo.Send(data);


            data = null;
            Cards = null;
            builder.Clear();
            //stage++;//3
            //SendSuitOfTableForClient(card[stage], Player);

            ////Console.WriteLine("Данные масти игроку отправлены");
            //sleep();

            //SendValueCardOfTableForClient(card[stage], Player);

            ////Console.WriteLine("Данные значения карты игроку отправлены");
            //sleep();
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

                    //SendPlayerData(message, Player, PlayerTwo);
                    //message = "Игроки подключились!\n";
                    //SendPlayerData(message, Player, PlayerTwo);
                    //message = "Вы 1 игрок\n";
                    //SendAlonePlayerData(message, Player);
                    //message = "Вы 2 игрок\n";
                    //SendAlonePlayerData(message, PlayerTwo);

                    DealCards DealCard = new DealCards();
                    DealCard.Deal();

                    DealCard.DisplayPlayersCard();
                    SendCards(DealCard.PlayerHand, Player);
                    //sleep();
                    SendCards(DealCard.PlayerHand, PlayerTwo);
                    //sleep();
                    DealCard.DisplayFlope();
                    //SendFlope(DealCard.TableCards, PlayerTwo);
                    SendFlope(DealCard.TableCards, Player, PlayerTwo);
                    //SendTurn(DealCard.TableCards, Player, PlayerTwo);
                    //sleep();
                    

                    //DealCard.DisplayTern();
                    //SendTurn(DealCard.TableCards, Player);
                    //sleep();
                    //SendFlope(DealCard.TableCards, PlayerTwo);

                    //DealCard.DisplayRiver();
                    //SendRiver(DealCard.TableCards, Player);
                    //sleep();
                    //SendFlope(DealCard.TableCards, PlayerTwo);


                    // получаем сообщение
                    //RecievePlayerData(Player);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public static string ConvertSuit(Card[] card, string Cards)
        {

            for (int i = 0; i < stage; i++)
            {//
                switch (card[i].MySuit)
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
            }
            return Cards;
        }
        public static string ConvertValue(Card[] card, string Cards)
        {
            for(int i = 0; i < stage; i++)
            {
                switch (card[i].MyValue)
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
            }
            return Cards;
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