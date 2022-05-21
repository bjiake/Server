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
        public Card[] playerHand;
        public Card[] playerTwoHand;
        public Card[] deckCards;

        private Card[] sortedPlayerTwoHand;
        private Card[] sortedPlayerHand;
        private Card[] sortedTableCards;

        public DealCards()
        {
            playerHand = new Card[2];
            playerTwoHand = new Card[2];
            deckCards = new Card[5];

            sortedPlayerHand = new Card[2];
            sortedPlayerTwoHand = new Card[2];
            sortedTableCards = new Card[5];
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
        public void EvaluateHands()
        {
            //create player's computer's evaluation objects (passing SORTED hand to constructor)

            HandEvaluator playerHandEvaluator = new HandEvaluator(sortedPlayerHand);
            HandEvaluator playerTwoHandEvaluator = new HandEvaluator(sortedPlayerTwoHand);

            //get the player;s and computer's hand
            Hand playerHand = playerHandEvaluator.EvaluateHand();
            Hand playerTwoHand = playerTwoHandEvaluator.EvaluateHand();

            //display each hand
            
            Console.WriteLine("\nPlayer Hand: " + playerHand);
            Console.WriteLine("\nPlayer Two Hand: " + playerTwoHand);

            //evaluate hands
            if (playerHand > playerTwoHand) { Console.WriteLine("Player №1 WINS!"); }
            else if (playerHand < playerTwoHand) { Console.WriteLine("Player №2 WINS!"); }
            else //if the hands are the same, evaluate the values
            {
                //first evaluate who has higher value of poker hand
                if (playerHandEvaluator.HandValues.Total > playerTwoHandEvaluator.HandValues.Total) { Console.WriteLine("Player №1 WINS!"); }
                else if (playerHandEvaluator.HandValues.Total < playerTwoHandEvaluator.HandValues.Total) { Console.WriteLine("Player №2 WINS!"); }

                //i# both hanve the same poker hand (for example, both have a pair of queens),
                //than the player with the next higher card wins L
                else if (playerHandEvaluator.HandValues.HighCard > playerTwoHandEvaluator.HandValues.HighCard) { Console.WriteLine("Player №1 WINS!"); }
                else if (playerHandEvaluator.HandValues.HighCard < playerTwoHandEvaluator.HandValues.HighCard) { Console.WriteLine("Player №2 WINS!"); }
                else { Console.WriteLine("No one wins!"); }
            }
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
            var QueryPlayer = from hand in playerHand
                              orderby hand.MyValue
                              select hand;
            var QueryPlayerTwo = from hand in playerTwoHand
                              orderby hand.MyValue
                              select hand;
            var QueryDealer = from hand in deckCards
                              orderby hand.MyValue
                              select hand;

            var index = 0;
            foreach (var element in QueryPlayer.ToList())
            {
                sortedPlayerHand[index] = element;
                index++;
            }

            index = 0;
            foreach (var element in QueryPlayerTwo.ToList())
            {
                sortedPlayerTwoHand[index] = element;
                index++;
            }

            index = 0;
            foreach (var element in QueryDealer.ToList())
            {
                sortedTableCards[index] = element;
                index++;
            }
        }
        public void GetHand()//Раздача карт
        {
            //5 карт дилера
            for (int i = 0; i < 5; i++)
            {
                deckCards[i] = getDeck[i];
            }
            //2 карты 1 игроку
            for (int i = 5; i < 7; i++)
            {
                playerHand[i - 5] = getDeck[i];
            }
            //
            for (int i = 7; i < 9; i++)
            {
                playerTwoHand[i - 7] = getDeck[i];
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
                DrawCards.DrawCardSuitValue(playerHand[i - 5], x, y);
                x++;
            }
        }
        public void DisplayPlayerTwoCard()
        {
            //Отображение карт игрока
            int y = 14;//Перемещение в место для карт игрока
            int x = 3;
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.SetCursorPosition(x * 12, y);
            Console.WriteLine("Карты 2 Игрока");
            y = 15;
            Console.SetCursorPosition(x, y);
            for (int i = 7; i < 9; i++)
            {
                DrawCards.DrawCardOutLine(x, y);
                DrawCards.DrawCardSuitValue(playerTwoHand[i - 7], x, y);
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
                DrawCards.DrawCardSuitValue(deckCards[i], x, y);
                x++;
            }
        }
        public void DisplayTurn()//Отображение терна
        {
            int x = 3;//Счет карты
            int y = 2;//Курсор(вверх вниз)

            Console.SetCursorPosition(x, y);

            DrawCards.DrawCardOutLine(x, y);
            DrawCards.DrawCardSuitValue(deckCards[x], x, y);
        }

        public void DisplayRiver()//Отображение ривера
        {
            int x = 4;//Счет карты
            int y = 2;//Курсор(вверх вниз)

            Console.SetCursorPosition(x, y);

            DrawCards.DrawCardOutLine(x, y);
            DrawCards.DrawCardSuitValue(deckCards[x], x, y);
        }
    }


}
