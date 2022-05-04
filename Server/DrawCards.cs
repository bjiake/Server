using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    class DrawCards
    {
        public static void DrawCardOutLine(int xCoordinate, int yCoordinate)
        {
            Console.ForegroundColor = ConsoleColor.White;

            int x = xCoordinate * 12;
            int y = yCoordinate;

            Console.SetCursorPosition(x, y);
            //Контур карт
            Console.Write(" __________\n");//Верхняя часть карт

            for (int i = 0; i < 10; ++i)//Отображение остальной части карт
            {
                Console.SetCursorPosition(x, y + 1 + i);

                if (i != 9)
                {
                    Console.WriteLine("|          |");//Середина карт
                }
                else
                {
                    Console.WriteLine("|__________|");//Нижняя часть карт
                }
            }
        }
        // Отображение масти и значения карты внутри контура
        public static void DrawCardSuitValue(Card card, int xCoordinate, int yCoordinate)
        {
            string CardSuit = " ";

            int x = xCoordinate * 12;
            int y = yCoordinate;

            //По коду CP437
            //Черви и бубны красные, трефы и черви черные(2665, 2666, 2660, 2663)
            switch (card.MySuit)
            {
                //Массив с 1 элементом, значения масти по коду CP437, Вывод
                case Card.SUIT.Hearts:
                    CardSuit = "\u2665";// Hearts
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Card.SUIT.Diamonds:
                    CardSuit = "\u2666";// Diamonds
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Card.SUIT.Clubs:
                    CardSuit = "\u2660"; // Spades

                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case Card.SUIT.Spades:
                    CardSuit = "\u2663"; // Clubs
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }
            //Отображение масти и значения карт
            Console.SetCursorPosition(x + 5, y + 5);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write(CardSuit);
            Console.SetCursorPosition(x + 4, y + 7);
            Console.Write(card.MyValue);
        }
    }
}
