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
        static MoneySystem PlayerMoney = new();
        static MoneySystem PlayerTwoMoney = new();

        public static int playerMoney = 5000;//Деньги игрока
        public static int playerTwoMoney = 5000;

        public static int bank;//Банк дилера
        public static int betPlayer;
        public static int betPlayerTwo;
        public static int generalBetPlayer;//Общая ставка игрок 1
        public static int generalBetPlayerTwo;//Общая ставка игрок 2

        public static int smallBlind;//smallBlind делает первый ход
        public static int bigBlind;

        public static bool roundContinue = true;//Для проведения раунда
        public static bool breakRound = false;
        public static int round = 1;//Счет раунда, каждые 5 раундов блайнды * 2, каждый 2й раунд smallBlind у 1 игрока

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
            for (int i = 0; i < 3; i++)//Конвертирование значений карт в Cards
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

        public static void displayInformation()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            ClearLine(30); ClearLine(31); ClearLine(32); ClearLine(33); ClearLine(34); ClearLine(35); ClearLine(36); ClearLine(37); ClearLine(38);
            Console.SetCursorPosition(0, 30);
        }
        public static void displayBank(int y)//Отобразить банк
        {
            ClearLine(y);
            Console.SetCursorPosition(0, y);
            Console.WriteLine("Банк:" + bank);
        }

        public enum PlayersMove
        {
            Check,
            Call,
            Raise,
            Fold
        }

        public static PlayersMove MyPlayerMove { get; set; }

        public static Winner CheckFirstTurn()
        {
            //1 чел коллит
            //2 чел рэйз
            //код идет
            displayInformation();
            
            if (round % 2 == 0)
            {
                Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаша текущая ставка:" + generalBetPlayer + "\nБанк:" + bank);

                //На сколько поднял ставка первый игрок
                betPlayer = PlayerMoney.ChoosePlayer(playerMoney, generalBetPlayer, generalBetPlayerTwo, smallBlind, bigBlind);
                if (MyPlayerMove == PlayersMove.Fold)
                {
                    return Winner.PlayerTwoWin;//Fold, выигрыш достается второму игроку
                }
                
                generalBetPlayer += betPlayer;//Общая ставка 1 игрока
                playerMoney -= betPlayer;//Банк 1 игрока
                bank += betPlayer;//Общий банк

                ClearLine(30); Console.SetCursorPosition(0, 30);//Чистка консоли
                Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаша текущая ставка:" + generalBetPlayer + "\nБанк:" + bank);
                ClearLine(36); ClearLine(37);//Чистка консоли

                displayBank(31);//Отобразить банк диллера
                Console.SetCursorPosition(0, 34);
                breakRound = roundContinue;

                //На сколько поднял ставку первый игрок
                Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                betPlayerTwo = PlayerTwoMoney.ChoosePlayer(playerTwoMoney, generalBetPlayerTwo, generalBetPlayer, smallBlind, bigBlind);
                if (MyPlayerMove == PlayersMove.Fold)
                {
                    return Winner.PlayerOneWin;//Фолд 2 игрока, выигрыш 1
                }
                generalBetPlayerTwo += betPlayerTwo;
                playerTwoMoney -= betPlayerTwo;
                breakRound = roundContinue;
                bank += betPlayerTwo;

                displayBank(35);
                ClearLine(30);
                ClearLine(36); ClearLine(37);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                if (MyPlayerMove == PlayersMove.Raise)
                {

                    do
                    {
                        //На сколько поднял ставку первый игрок
                        Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                        betPlayer = PlayerMoney.ChoosePlayer(playerTwoMoney, generalBetPlayerTwo, generalBetPlayer, smallBlind, bigBlind);

                        generalBetPlayerTwo += betPlayerTwo;
                        playerTwoMoney -= betPlayerTwo;
                        breakRound = roundContinue;
                        bank += betPlayerTwo;
                        displayBank(35);
                        if (MyPlayerMove == PlayersMove.Fold)
                        {
                            return Winner.PlayerTwoWin;//Фолд 1 выигрыш 2
                        }

                        
                        ClearLine(30);
                        ClearLine(36); ClearLine(37);
                        Console.SetCursorPosition(0, 30);
                        Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);

                        //Ход 2 игрока
                        Console.WriteLine("Ваш баланс(1):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);
                        betPlayerTwo = PlayerMoney.ChoosePlayer(playerTwoMoney, generalBetPlayerTwo, generalBetPlayer, smallBlind, bigBlind);
                        
                        generalBetPlayerTwo += betPlayerTwo;
                        playerTwoMoney -= betPlayerTwo;
                        breakRound = roundContinue;
                        bank += betPlayerTwo;
                        displayBank(35);

                        if (MyPlayerMove == PlayersMove.Fold)
                        {
                            return Winner.PlayerOneWin;//Фолд 2 игрока, выигрыш 1
                        }

                        ClearLine(30);
                        Console.SetCursorPosition(0, 30);
                        Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаша текущая ставка:" + generalBetPlayer + "\nБанк:" + bank);
                        ClearLine(36); ClearLine(37);
                    }
                    while (generalBetPlayer == generalBetPlayerTwo);
                }
                return Winner.NoOneWins;
            }
            else
            {
                //Ход 2 игрока
                Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                betPlayerTwo = PlayerTwoMoney.ChoosePlayer(playerTwoMoney, generalBetPlayerTwo, generalBetPlayer, smallBlind, bigBlind); 
                
                generalBetPlayerTwo += betPlayerTwo;
                playerTwoMoney -= betPlayerTwo;
                breakRound = roundContinue;
                bank += betPlayerTwo;

                if (MyPlayerMove == PlayersMove.Fold)
                {
                    return Winner.PlayerOneWin;//Фолд 2 игрока, выигрыш 1
                }

                ClearLine(30);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                displayBank(31);
                Console.SetCursorPosition(0, 34);
                ClearLine(36); ClearLine(37);


               //Ход 1 игрока
                Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаша текущая ставка:" + generalBetPlayer + "\nБанк:" + bank);
                betPlayer = PlayerMoney.ChoosePlayer(playerMoney, generalBetPlayer, generalBetPlayerTwo, smallBlind, bigBlind);

                generalBetPlayer += betPlayer;
                playerMoney -= betPlayer;
                breakRound = roundContinue;
                bank += betPlayer;
                displayBank(35);

                if (MyPlayerMove == PlayersMove.Fold)
                {
                    return Winner.PlayerTwoWin;//Фолд 1 игрока, выигрыш 2
                }

                ClearLine(30);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаша текущая ставка:" + generalBetPlayer + "\nБанк:" + bank);
                ClearLine(36); ClearLine(37);

                if (MyPlayerMove == PlayersMove.Raise)
                {

                    do
                    {
                        //Ход 2 игрока
                        Console.WriteLine("Ваш баланс(1):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);
                        betPlayerTwo = PlayerMoney.ChoosePlayer(playerTwoMoney, generalBetPlayerTwo, generalBetPlayer, smallBlind, bigBlind);
                        
                        generalBetPlayerTwo += betPlayerTwo;
                        playerTwoMoney -= betPlayerTwo;
                        breakRound = roundContinue;
                        bank += betPlayerTwo;
                        displayBank(35);

                        if (MyPlayerMove == PlayersMove.Fold)
                        {
                            return Winner.PlayerOneWin;//Фолд 2 игрока, выигрыш 1
                        }

                        ClearLine(30);
                        Console.SetCursorPosition(0, 30);
                        Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаша текущая ставка:" + generalBetPlayer + "\nБанк:" + bank);
                        ClearLine(36); ClearLine(37);

                        //На сколько поднял ставку первый игрок
                        Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                        betPlayer = PlayerMoney.ChoosePlayer(playerTwoMoney, generalBetPlayerTwo, generalBetPlayer, smallBlind, bigBlind);

                        generalBetPlayerTwo += betPlayerTwo;
                        playerTwoMoney -= betPlayerTwo;
                        breakRound = roundContinue;
                        bank += betPlayerTwo;

                        if (MyPlayerMove == PlayersMove.Fold)
                        {
                            return Winner.PlayerTwoWin;//Фолд 2 выигрыш 1
                        }

                        displayBank(35);
                        ClearLine(30);
                        ClearLine(36); ClearLine(37);
                        Console.SetCursorPosition(0, 30);
                        Console.WriteLine("Ваш баланс(2):" + playerTwoMoney + "\tВаша текущая ставка:" + generalBetPlayerTwo + "\nБанк:" + bank);// + "\nБаланс противника:" + playerMoney + "\tТекущая ставка противника:" + generalBetPlayer);
                    }
                    while (generalBetPlayer == generalBetPlayerTwo);
                }

                return Winner.NoOneWins;//Никто не фолдил
            }
        }

        public static void getBlinds()
        {
            if (round % 2 != 0)
            {
                if(playerTwoMoney >= smallBlind)
                {   
                    generalBetPlayerTwo += smallBlind;
                    bank += smallBlind;
                    playerTwoMoney -= smallBlind;
                }
                else
                {
                    generalBetPlayerTwo += playerTwoMoney;
                    bank += playerTwoMoney;
                    playerTwoMoney = 0;
                }
                
                if(playerMoney >= bigBlind)
                {
                    generalBetPlayer += bigBlind;
                    bank += bigBlind;
                    playerMoney -= bigBlind;
                }
                else
                {
                    generalBetPlayer += playerMoney;
                    bank += playerMoney;
                    playerMoney = 0;
                }
                
            }
            else
            {
                if (playerMoney >= smallBlind)
                {
                    generalBetPlayer += smallBlind;
                    bank += smallBlind;
                    playerMoney -= smallBlind;
                }
                else
                {
                    generalBetPlayer += playerMoney;
                    bank += playerMoney;
                    playerMoney = 0;
                }

                if(playerTwoMoney >= bigBlind)
                {
                    generalBetPlayerTwo += bigBlind;
                    bank += bigBlind;
                    playerTwoMoney -= bigBlind;
                }
                else
                {
                    generalBetPlayerTwo += playerTwoMoney;
                    bank += playerTwoMoney;
                    playerTwoMoney = 0;
                }
            }
        }

        public static void RaiseBlinds()
        {
            if (round % 5 == 0)
            {
                smallBlind *= 2;
                bigBlind *= 2;
            }
        }

        public static void ClearLine(int line)
        {
            Console.MoveBufferArea(0, line, Console.BufferWidth, 1, Console.BufferWidth, line, ' ', Console.ForegroundColor, Console.BackgroundColor);
        }

        public static void displayWinner()
        {
            ClearLine(30); ClearLine(31); ClearLine(32); ClearLine(33); ClearLine(34); ClearLine(35);
            Console.SetCursorPosition(0, 30);
            Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаш баланс(2):" + playerTwoMoney);
        }


        public enum Winner
        {
            NoOneWins,
            PlayerOneWin,
            PlayerTwoWin
        }

        public static Winner myWinner { get; set; }


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
            
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(IpPoint);

                // начинаем прослушивание очередь входящих потоков
                //listenSocket.Listen(5);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)//Цикл на подключение
                {
                    //Socket player = listenSocket.Accept();
                    //Socket playerTwo = listenSocket.Accept();
                    smallBlind = 25;
                    bigBlind = smallBlind * 2;

                    while (playerMoney > 0 && playerTwoMoney > 0)//Цикл саму игру
                    {
                       

                        while (true)//Цикл на раунд
                        {
                            Console.Clear();//Чистка от прошлого раунда и т.п
                            DealCard.Deal();//выдача карт

                            RaiseBlinds();//Поднять блайнды
                            getBlinds();//Взять блайнды с игроков

                            
                            //Разобратся с checkfold raise
                            DealCard.DisplayPlayerCard();
                            DealCard.DisplayPlayerTwoCard();
                            //stage 2
                            //SendCards(DealCard.playerHand, player);
                            //SendCards(DealCard.playerTwoHand, playerTwo);
                            
                            myWinner = CheckFirstTurn();

                            if(myWinner == Winner.PlayerTwoWin)//Fold, выигрыш достается второму игроку
                            {
                                playerTwoMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }
                            else if(myWinner == Winner.PlayerOneWin)
                            {
                                playerMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }

                            //stage 3
                            DealCard.DisplayFlope();
                            //SendFlope(DealCard.dealerCards, player, playerTwo);
                            sleep();
                            myWinner = CheckFirstTurn();
                            if (myWinner == Winner.PlayerTwoWin)
                            {
                                playerTwoMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }
                            else if (myWinner == Winner.PlayerOneWin)
                            {
                                playerMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }

                            //stage 3
                            DealCard.DisplayTurn();
                            //SendTurn(DealCard.dealerCards[stage], player, playerTwo);
                            sleep();
                            myWinner = CheckFirstTurn();
                            if (myWinner == Winner.PlayerTwoWin)
                            {
                                playerTwoMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }
                            else if (myWinner == Winner.PlayerOneWin)
                            {
                                playerMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }

                            //stage 4
                            DealCard.DisplayRiver();
                            //SendRiver(DealCard.dealerCards[stage], player, playerTwo);
                            sleep();

                            myWinner = CheckFirstTurn();
                            if (myWinner == Winner.PlayerTwoWin)
                            {
                                playerTwoMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }
                            else if (myWinner == Winner.PlayerTwoWin)
                            {
                                playerMoney += bank;
                                displayWinner();
                                ClearData();
                                break;
                            }
                            //stage 5
                            ClearLine(30); ClearLine(31); ClearLine(32); ClearLine(33); ClearLine(34); ClearLine(35);
                            
                            Console.SetCursorPosition(0, 30);
                            
                            DealCard.EvaluateHands();//Допилить вывод победителя card[7]
                            Console.WriteLine("Ваш баланс(1):" + playerMoney + "\tВаш баланс(2):" + playerTwoMoney);
                            Console.ReadLine();
                            ClearData();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void ClearData()
        {
            generalBetPlayer = 0;
            generalBetPlayerTwo = 0;
            bank = 0;
            ++round;
            roundContinue = false;
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
    //SendPlayerData(message, player, playerTwo);
    //message = "Игроки подключились!\n";
    //SendPlayerData(message, player, playerTwo);
    //message = "Вы 1 игрок\n";
    //SendAlonePlayerData(message, player);
    //message = "Вы 2 игрок\n";
    //SendAlonePlayerData(message, playerTwo);
}
//do
//{
//    DealCard.Deal();
//    Console.SetCursorPosition(1, 28);
//    DealCard.EvaluateHands();//Допилить вывод победителя построить на основании card[7]
//}
//while (DealCard.playerHandEvaluate != Hand.FlushRoyal && DealCard.playerTwoHandEvaluate != Hand.FlushRoyal);
//DealCard.Deal();
//DealCard.DisplayPlayerCard();
//DealCard.DisplayPlayerTwoCard();
//DealCard.DisplayFlope();
//DealCard.DisplayTurn();
//DealCard.DisplayRiver();
//Console.SetCursorPosition(1, 28);
//DealCard.EvaluateHands();//Допилить вывод победителя построить на основании card[7]