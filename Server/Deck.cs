using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer   
{
    class Deck : Card
    {
        const int numOfCards = 52;//Номер всех карт
        private Card[] deck;//массив всех игральных карт

        public Deck()
        {
            deck = new Card[numOfCards];
        }
        public Card[] getDeck { get { return deck; } }

        public void SetUpDeck()
        {
            int i = 0;
            foreach (SUIT s in Enum.GetValues(typeof(SUIT)))
            {
                foreach (VALUE v in Enum.GetValues(typeof(VALUE)))
                {
                    deck[i] = new Card { MySuit = s, MyValue = v };
                    ++i;
                }
            }
            ShuffleCards();
        }

        public void ShuffleCards()
        {
            Random rand = new Random();
            Card temp;

            for (int ShuffleTimes = 0; ShuffleTimes < 2000; ++ShuffleTimes)
            {
                for (int i = 0; i < numOfCards; ++i)
                {
                    int SecondCardIndex = rand.Next(13);
                    temp = deck[i];
                    deck[i] = deck[SecondCardIndex];
                    deck[SecondCardIndex] = temp;
                }
            }
        }
    }
}
