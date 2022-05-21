using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer  
{
    class DealCards : Deck
    {
        public Card[] PlayerHand;
        public Card[] PlayerTwoHand;
        public Card[] TableCards;

        private Card[] SortedPlayerTwoHand;
        private Card[] SortedPlayerHand;
        private Card[] SortedTableCards;

        public DealCards()
        {
            PlayerHand = new Card[2];
            PlayerTwoHand = new Card[2];
            TableCards = new Card[5];

            SortedPlayerHand = new Card[2];
            SortedPlayerTwoHand = new Card[2];
            SortedTableCards = new Card[5];
        }
        
        public void Deal()
        {
            SetUpDeck();//Создание колоды карт
            GetHand();//Тасование в руки
            SortCards();//Сортировка для сравнивания
            //DisplayPlayersCard();//Показать карты игрока
            //Motion();//Сделать ход(bet, fold, raise, check, call)
            //DisplayFlope();//Флоп
            ////Сделать ход(bet, fold, raise, check, call)
            //DisplayTern();//Терн
            ////Сделать ход(bet, fold, raise, check, call)
            //DisplayRiver();//Ривер
            ////Сделать ход(bet, fold, raise, check, call)
            ////EvalueateHands();//Подсчет очков
        }

        
        public void Fold()
        {

        }

        public void Check()
        {

        }

        public void Raise()
        {

        }
        class DillerAmount
        {
            public int TotalPot = 0;
        }
        class Amount
        {
            public int StarterPot = 5000;//Начальные деньги
            public int FinalPot;
            public int SmallBlind = 20;
            public int BigBlind = 20 * 2;
        }
        public void Motion()
        {
            Amount PlayerAmount = new();
            DillerAmount dillerAmount = new();
            string Choose;
            int y = 27;
            int x = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x, y);

            Console.WriteLine($"Ваш банк: {PlayerAmount.StarterPot}");
            Console.WriteLine("Сделайте ход: [1]Fold [2]Check [3]Raise");

            while (true)
            {
                Choose = Console.ReadLine();
                if (Choose == "Fold")
                {
                    Fold();
                    break;
                }
                else if (Choose == "Check")
                {
                    Check();
                    break;
                }
                else if (Choose == "Raise")
                {
                    Raise();
                    break;
                }
                else
                {
                    Console.WriteLine("Некорректный ввод, пожайлуста, повторите:");
                }
            }
        }

        public void SortCards()//Сортировка карт для удобного сравнивания
        {
            var QueryPlayer = from hand in PlayerHand
                              orderby hand.MyValue
                              select hand;
            var QueryPlayerTwo = from hand in PlayerTwoHand
                              orderby hand.MyValue
                              select hand;
            var QueryDealer = from hand in TableCards
                              orderby hand.MyValue
                              select hand;

            var index = 0;
            foreach (var element in QueryPlayer.ToList())
            {
                SortedPlayerHand[index] = element;
                index++;
            }

            index = 0;
            foreach (var element in QueryPlayerTwo.ToList())
            {
                SortedPlayerTwoHand[index] = element;
                index++;
            }

            index = 0;
            foreach (var element in QueryDealer.ToList())
            {
                SortedTableCards[index] = element;
                index++;
            }
        }
        public void GetHand()//Раздача карт
        {
            //5 карт дилера
            for (int i = 0; i < 5; i++)
            {
                TableCards[i] = getDeck[i];
            }
            //2 карты 1 игроку
            for (int i = 5; i < 7; i++)
            {
                PlayerHand[i - 5] = getDeck[i];
            }
            //
            for (int i = 7; i < 9; i++)
            {
                PlayerTwoHand[i - 7] = getDeck[i];
            }
        }

        
        public void DisplayPlayerCard()
        {
            //Отображение карт игрока
            int y = 14;//Перемещение в место для карт игрока
            int x = 0;
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Карты 1 Игрока");
            y = 15;
            Console.SetCursorPosition(x, y);
            for (int i = 5; i < 7; i++)
            {
                DrawCards.DrawCardOutLine(x, y);
                DrawCards.DrawCardSuitValue(PlayerHand[i - 5], x, y);
                x++;
            }
        }
        public void DisplayPlayerTwoCard()
        {
            //Отображение карт игрока
            int y = 14;//Перемещение в место для карт игрока
            int x = 4;
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Карты 2 Игрока");
            y = 15;
            Console.SetCursorPosition(x, y);
            for (int i = 7; i < 9; i++)
            {
                DrawCards.DrawCardOutLine(x, y);
                DrawCards.DrawCardSuitValue(PlayerTwoHand[i - 7], x, y);
                x++;
            }
        }

        public void DisplayFlope()//Отображение флопа
        {
            int x = 0;//Счет карты
            int y = 1;//Курсор(вверх вниз)//ЛСП карусель

            //Отображение карт дилера

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Карты Дилера");
            y = 2;
            Console.SetCursorPosition(x, y);

            for (int i = 0; i < 3; i++)
            {
                DrawCards.DrawCardOutLine(x, y);
                DrawCards.DrawCardSuitValue(TableCards[i], x, y);
                x++;
            }
        }
        public void DisplayTurn()//Отображение терна
        {
            int x = 3;//Счет карты
            int y = 2;//Курсор(вверх вниз)

            Console.SetCursorPosition(x, y);

            DrawCards.DrawCardOutLine(x, y);
            DrawCards.DrawCardSuitValue(TableCards[x], x, y);
        }

        public void DisplayRiver()//Отображение ривера
        {
            int x = 4;//Счет карты
            int y = 2;//Курсор(вверх вниз)

            Console.SetCursorPosition(x, y);

            DrawCards.DrawCardOutLine(x, y);
            DrawCards.DrawCardSuitValue(TableCards[x], x, y);
        }
    }


}
