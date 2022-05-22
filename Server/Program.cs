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
        public static int stage = 2;//stage 2 Карты игроков, stage 3 Карты флопа и терна, stage 4 ривер
        //Порт локальной 127.0.0.1

        public static void RecievePlayerData(Socket player)//Позже нужный мусор забей хуй
        {
            string message = "Ждём игроков";
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных
            do
            {
                bytes = player.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (player.Available > 0);


            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

            // отправляем ответ
            message = "ваше сообщение доставлено";
            data = Encoding.Unicode.GetBytes(message);
            player.Send(data);
            data = null;
            bytes = 0;
            builder.Clear();
        }
        
        public static void SendPlayerData(string message, Socket player, Socket playerTwo)//Позже нужный мусор забей хуй
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            data = Encoding.Unicode.GetBytes(message);
            player.Send(data);
            playerTwo.Send(data);
            builder.Clear();
            data = null;
        }

        public static void SendAlonePlayerData(string message, Socket player)//Позже нужный мусор забей хуй
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            data = Encoding.Unicode.GetBytes(message);
            player.Send(data);
            builder.Clear();
            data = null;
        }
        public static void sleep()//Пауза в коде на 0.5с
        {
            Thread.Sleep(500);
        }
        public static void SendCards(Card[] card, Socket player)//Отправка карт игрока
        {
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";//Значения карты в виде 031213, где 0 масть - 1 карты, 3 масть - 2 карты,
                               //12 значение - 1 карты, 13 значение - 2 карты      
            for (int i = 0; i < 2; i++) { Cards = ConvertSuit(card[i], Cards); }//Конвертирование масти карты в Cards
            for (int i = 0; i < 2; i++) { Cards = ConvertValue(card[i], Cards); }//Конвертирование значений карт в Cards

            data = Encoding.Unicode.GetBytes(Cards);//Конвертирование Cards для отправки данных
            player.Send(data);//Отправка данных игроку
            //Очистка
            Cards = null;
            data = null;
        }
        public static void SendFlope(Card[] card, Socket player, Socket playerTwo)//Отправка флопа
        {
            stage++;//3 стадия игры
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = " ";//Значения карты в виде 210102205
                               //Где 2 масть - 1 карты, 1 масть - 2 карты, 0 масть - 3 карты,
                               //10 значение - 1 карты, 22 значение - 2 карты, 05 - значение 3 карты
            for (int i = 0; i < 3; i++)//Конвертирование масти карты в Cards
            {
                Cards = ConvertSuit(card[i], Cards);
            }
            for(int i = 0; i < 3; i ++)//Конвертирование значений карт в Cards
            {
                Cards = ConvertValue(card[i], Cards);
            }

            data = Encoding.Unicode.GetBytes(Cards);//Конвертирование Cards для отправки данных
            player.Send(data);//Отправка данных игроку
            playerTwo.Send(data);//Отправка данных второму игроку
            //Очистка
            data = null;
            Cards = null;
        }
        public static void SendTurn(Card card, Socket player, Socket playerTwo)
        {
            //stage 3
            //StringBuilder builder = new StringBuilder();
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = null;
            Cards = ConvertSuit(card, Cards);//Конвертирование масти карты в Cards
            Cards = ConvertValue(card, Cards);

            data = Encoding.Unicode.GetBytes(Cards);
            player.Send(data);
            playerTwo.Send(data);

            data = null;
            Cards = null;
            //builder.Clear();
            stage++;//4
        }

        public static void SendRiver(Card card, Socket player, Socket playerTwo)
        {
            byte[] data = new byte[256]; // буфер для получаемых данных

            string Cards = null;
            Cards = ConvertSuit(card, Cards);//Конвертирование масти карты в Cards
            Cards = ConvertValue(card, Cards);

            data = Encoding.Unicode.GetBytes(Cards);
            player.Send(data);
            playerTwo.Send(data);

            data = null;
            Cards = null;
        }

        static void Main()
        {
            Console.Title = "Блэйк Джек сервер";

            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            Console.BackgroundColor = ConsoleColor.Gray;
            
            Console.Clear();

            Console.BufferWidth = 120;
            Console.WindowWidth = Console.BufferWidth;
            Console.WindowHeight = 40;
            Console.BufferHeight = Console.WindowHeight;

            // получаем адреса для запуска сокета
            var IpPoint = new IPEndPoint(IPAddress.Parse(HostName), port);

            // создаем сокет
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            DealCards DealCard = new DealCards();
            do
            {
                DealCard.Deal();
                Console.SetCursorPosition(1, 28);
                DealCard.EvaluateHands();//Допилить вывод победителя построить на основании card[7]
            }
            while (DealCard.playerHandEvaluate != Hand.StraightFlush && DealCard.playerTwoHandEvaluate != Hand.StraightFlush);
            DealCard.DisplayPlayerCard();
            DealCard.DisplayPlayerTwoCard();
            DealCard.DisplayFlope();
            DealCard.DisplayTurn();
            DealCard.DisplayRiver();
            Console.SetCursorPosition(1, 28);
            DealCard.EvaluateHands();//Допилить вывод победителя построить на основании card[7]

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                //listenSocket.Bind(IpPoint);

                // начинаем прослушивание очередь входящих потоков
                //listenSocket.Listen(5);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    //Socket player = listenSocket.Accept();
                    //Socket playerTwo = listenSocket.Accept();

                    //SendPlayerData(message, player, playerTwo);
                    //message = "Игроки подключились!\n";
                    //SendPlayerData(message, player, playerTwo);
                    //message = "Вы 1 игрок\n";
                    //SendAlonePlayerData(message, player);
                    //message = "Вы 2 игрок\n";
                    //SendAlonePlayerData(message, playerTwo);

                    //DealCards DealCard = new DealCards();
                    //DealCard.Deal();

                    //DealCard.DisplayPlayerCard();
                    //DealCard.DisplayPlayerTwoCard();
                    ////stage 2
                    //SendCards(DealCard.playerHand, player);
                    //SendCards(DealCard.playerTwoHand, playerTwo);

                    ////stage 3
                    //DealCard.DisplayFlope();
                    //SendFlope(DealCard.dealerCards, player, playerTwo);
                    //sleep();

                    ////stage 3
                    //DealCard.DisplayTurn();
                    //SendTurn(DealCard.dealerCards[stage], player, playerTwo);//ОШИБКА ОТПРАВЛЕНИЯ ТЕРНА
                    //sleep();
                    ////stage 4
                    //DealCard.DisplayRiver();
                    //SendRiver(DealCard.dealerCards[stage], player, playerTwo);
                    //sleep();

                    //Console.SetCursorPosition(1, 28);
                    //DealCard.EvaluateHands();//Допилить вывод победителя card[7]
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public static string ConvertSuit(Card card, string Cards)
        {

            switch (card.MySuit)//Card - это Общие карты в классе Card, card - Это текущие карты(Например карты игрока или стола)
            {
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
            //Числа 2х значные для удобства распаковки у клиента
            switch (card.MyValue)//Card - это Общие карты в классе Card, card - Это текущие карты(Например карты игрока или стола)
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