using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    public enum Hand
    {
        Nothing,
        OnePair,
        TwoPairs,
        ThreeOfKind,
        Straight,
        Flush,
        FullHouse,
        FourOfKind
    }
    public struct HandValue
    {
        public int Total { get; set; }
        public int HighCard { get; set; }
    }

    class HandEvaluator : Card
    {
        private int heartsSum;
        private int diamondSum;
        private int clubSum;
        private int spadesSum;

        private Card[] cardsDealer;
        private Card[] cardsPlayer;

        private HandValue handValue;

        public HandEvaluator(Card[] sortedHand, Card[] sortedDealerCards)
        {
            heartsSum = 0;
            diamondSum = 0;
            clubSum = 0;
            spadesSum = 0;

            cardsPlayer = new Card[2];
            CardsPlayer = sortedHand;

            cardsDealer = new Card[5];
            CardsDealer = sortedDealerCards;
            
            handValue = new HandValue();
        }

        public HandValue HandValues
        {
            get { return handValue; }
            set { handValue = value; }
        }

        public Card[] CardsPlayer
        {
            get { return cardsDealer; }
            set
            {
                cardsPlayer[0] = value[0];
                cardsPlayer[1] = value[1];
            }
        }

        public Card[] CardsDealer
        {
            get { return cardsDealer; }
            set
            {
                cardsDealer[0] = value[0];
                cardsDealer[1] = value[1];
                cardsDealer[2] = value[2];
                cardsDealer[3] = value[3];
                cardsDealer[4] = value[4];
            }
        }

        public Hand EvaluateHand()
        {
            GetNumberOfSuit();
            if (FourOfKind()) { return Hand.FourOfKind; }
            else if (FullHouse()) { return Hand.FullHouse; }
            else if (Flush()) { return Hand.Flush; }
            else if (Straight()) { return Hand.Straight; }
            else if (ThreeOfKind()) { return Hand.ThreeOfKind; }
            else if (TwoPairs()) { return Hand.TwoPairs; }
            else if (OnePair()) { return Hand.OnePair; }

            handValue.HighCard = (int)cardsDealer[4].MyValue;
            return Hand.Nothing;
        }

        private void GetNumberOfSuit()
        {
            foreach (var element in CardsPlayer)
            {
                if (element.MySuit == Card.SUIT.Hearts) { heartsSum++; }
                else if (element.MySuit == Card.SUIT.Diamonds) { diamondSum++; }
                else if (element.MySuit == Card.SUIT.Clubs) { clubSum++; }
                else if (element.MySuit == Card.SUIT.Spades) { spadesSum++; }
            }

            foreach (var element in CardsDealer)
            {
                if (element.MySuit == Card.SUIT.Hearts) { heartsSum++; }
                else if (element.MySuit == Card.SUIT.Diamonds) { diamondSum++; }
                else if (element.MySuit == Card.SUIT.Clubs) { clubSum++; }
                else if (element.MySuit == Card.SUIT.Spades) { spadesSum++; }
            }
        }

        private bool FourOfKind()
        {
            //if the first 4 cardsDealer, add values of the four cardsDealer and last card is the highest
            //2 - игрок. 5 - дилер
            //0 - 012, 0 - 123, 0 - 234,
            //1 - 012, 1 - 123, 1 - 234,
            //12 - 01, 12 - 12, 12 - 23, 12 - 34
            if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue)
                
            //(cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[0].MyValue == cardsDealer[2].MyValue && cardsDealer[0].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 4;
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            else if (cardsDealer[1].MyValue == cardsDealer[2].MyValue && cardsDealer[1].MyValue == cardsDealer[3].MyValue && cardsDealer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 4;
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            return false;
        }
        private bool FullHouse()
        {
            //the First three cars and last two cardsDealer are of the same value
            // first two cardsDealer, and last three cardsDealer are of the same value
            if ((cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[0].MyValue == cardsDealer[2].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue) ||
               (cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue && cardsDealer[2].MyValue == cardsDealer[4].MyValue))
            {
                handValue.Total = (int)(cardsDealer[0].MyValue) + (int)(cardsDealer[1].MyValue) + (int)(cardsDealer[2].MyValue) +
                    (int)(cardsDealer[3].MyValue) + (int)(cardsDealer[4].MyValue);
                return true;
            }
            return false;
        }
        private bool Flush()
        {
            //if all suits are the same
            if (heartsSum == 5 || diamondSum == 5 || clubSum == 5 || spadesSum == 5)
            {
                //3# flush, the player with higher cardsDealer win

                //whoever has the last card the highest, has automatically all the cardsDealer total higher
                handValue.Total = (int)cardsDealer[4].MyValue;
                return true;
            }
            return false;
        }
        private bool Straight()
        {
            //3 5 consecutive values

            if (cardsDealer[0].MyValue + 1 == cardsDealer[1].MyValue &&
                cardsDealer[1].MyValue + 1 == cardsDealer[2].MyValue &&
                cardsDealer[2].MyValue + 1 == cardsDealer[3].MyValue &&
                cardsDealer[3].MyValue + 1 == cardsDealer[4].MyValue)
            {
                //player with the highest value of the last card wins
                handValue.Total = (int)cardsDealer[4].MyValue;
                return true;
            }
            return false;
        }
        private bool ThreeOfKind()
        {
            if ((cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[0].MyValue == cardsDealer[2].MyValue) ||
            (cardsDealer[1].MyValue == cardsDealer[2].MyValue && cardsDealer[1].MyValue == cardsDealer[3].MyValue))
            {
                handValue.Total = (int)cardsDealer[2].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            else if (cardsDealer[2].MyValue == cardsDealer[3].MyValue && cardsDealer[2].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[2].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            return false;
        }

        private bool TwoPairs()
        {
            if (cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsDealer[1].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            else if (cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsDealer[1].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[2].MyValue;
                return true;
            }
            else if (cardsDealer[1].MyValue == cardsDealer[2].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsDealer[1].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            return false;
        }

        private bool OnePair()
        {
            if (cardsDealer[0].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsDealer[0].MyValue * 2;
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            else if (cardsDealer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 2;
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            else if (cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsDealer[2].MyValue * 2;
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            else if (cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[3].MyValue * 2;
                handValue.HighCard = (int)cardsDealer[2].MyValue;
                return true;
            }
            return false;
        }
    }
}
