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
        private Card[] deckCards;
        private Card[] cardsPlayer;

        public bool FirstPlayer;

        private HandValue handValue;

        public HandEvaluator(Card[] sortedHand, Card[] sortedDealerCards, Card[] sortedDeckCards, bool firstPlayer)
        {
            heartsSum = 0;
            diamondSum = 0;
            clubSum = 0;
            spadesSum = 0;
            FirstPlayer = firstPlayer;

            cardsPlayer = new Card[2];
            CardsPlayer = sortedHand;

            cardsDealer = new Card[5];
            CardsDealer = sortedDealerCards;

            deckCards = new Card[7];
            DeckCards = sortedDeckCards;


            handValue = new HandValue();
        }

        public HandValue HandValues
        {
            get { return handValue; }
            set { handValue = value; }
        }
        public Card[] DeckCards
        {
            get { return deckCards; }

            set
            {
                if (FirstPlayer == true)
                {
                    deckCards[0] = value[0];
                    deckCards[1] = value[1];
                    deckCards[2] = value[2];
                    deckCards[3] = value[3];
                    deckCards[4] = value[4];
                    deckCards[5] = value[5];
                    deckCards[6] = value[6];
                }
                else if (FirstPlayer == false)
                {
                    deckCards[0] = value[0];
                    deckCards[1] = value[1];
                    deckCards[2] = value[2];
                    deckCards[3] = value[3];
                    deckCards[4] = value[4];
                    deckCards[5] = value[7];
                    deckCards[6] = value[8];
                }
            }
        }
    

        public Card[] CardsPlayer
        {
            get { return cardsPlayer; }
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
            //(cardsDealer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[0].MyValue == cardsDealer[2].MyValue && cardsDealer[0].MyValue == cardsDealer[3].MyValue)
            //0 - 012
            if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue)      
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            //0 - 123
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsPlayer[0].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            //0 - 234
            else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsPlayer[0].MyValue == cardsDealer[3].MyValue && cardsPlayer[0].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            //1 - 012
            else if(cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //1 - 123
            else if (cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //1 - 234
            else if (cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //12 - 01 
            else if (cardsPlayer[1].MyValue == cardsPlayer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsPlayer[2].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //12 - 12
            else if (cardsPlayer[1].MyValue == cardsPlayer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[2].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //12 - 23
            else if (cardsPlayer[1].MyValue == cardsPlayer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsPlayer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //12 - 34
            else if (cardsPlayer[1].MyValue == cardsPlayer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue && cardsPlayer[2].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            return false;
        }
        private bool FullHouse()
        {
            //the First three cars and last two cardsDealer are of the same value
            // first two cardsDealer, and last three cardsDealer are of the same value
            //3 одинаковых и 2 одинаковых
            //
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
            int count = 7;

            for (int i = 1; i <= 6; i++)
            {
                if (deckCards[i].MySuit == deckCards[i - 1].MySuit)
                {
                    deckCards[i].MySuit = 0;
                    count--;
                }
            }
            if (count > 4)
            {
                count = 0;
                for (int i = 1; i <= 6; i++)
                {
                    if (deckCards[i].MySuit != 0)
                    {
                        count++;
                        handValue.Total = (int)deckCards[i].MyValue;
                    }
                }
                if (count > 4)
                {
                    return true;
                    //street = true;
                }
            }
            return false;
            //if all suits are the same

            //if (heartsSum >= 5 || diamondSum >= 5 || clubSum >= 5 || spadesSum >= 5)
            //{
            //    //3# flush, the player with higher cardsDealer win

            //    //whoever has the last card the highest, has automatically all the cardsDealer total higher
            //    for(int i = 6; i >= 0; i--)
            //    {
            //        if (deckCards[i].MyValue == deckCards[i - 1].MyValue)
            //        {
            //            handValue.Total = (int)deckCards[i].MyValue;
            //            deckCards[i].mys
            //        }
            //    }
            //    //if (cardsPlayer[1].MyValue > CardsDealer[4].MyValue)
            //    //{
            //    //    handValue.Total = (int)cardsPlayer[1].MyValue;
            //    //}
            //    //else if (cardsPlayer[1].MyValue < CardsDealer[4].MyValue)
            //    //{
            //    //    handValue.Total = (int)cardsDealer[4].MyValue;
            //    //}
            //    return true;
            //}

        }
        private bool Straight()
        {
            int count = 7;
            
            for (int i = 1; i <= 6; i++)
            {
                if (deckCards[i].MyValue == deckCards[i - 1].MyValue)
                {
                    deckCards[i].MyValue = 0;
                    count--;
                }
            }
            if(count > 4)
            {
                count = 0;
                for (int i = 1; i <= 6; i++)
                {
                    if (deckCards[i].MyValue != 0 && deckCards[i].MyValue == deckCards[i - 1].MyValue + 1)
                    {
                        count++;
                        handValue.Total = (int)deckCards[i].MyValue;
                    }
                }
                if(count > 4)
                {
                    return true;
                    //street = true;
                }
            }
            return false;
            //3 5 consecutive values
            //Стрит
            //начало рука 0 конец стол
            //0 - 0123, 0 - 1234
            //Начало рука 1 конец стол
            // 1 - 0123, 1 - 1234
            //Начало стол конец рука 0
            // - 0123,

            //if (cardsDealer[0].MyValue + 1 == cardsDealer[1].MyValue &&
            //    cardsDealer[1].MyValue + 1 == cardsDealer[2].MyValue &&
            //    cardsDealer[2].MyValue + 1 == cardsDealer[3].MyValue &&
            //    cardsDealer[3].MyValue + 1 == cardsDealer[4].MyValue)
            //{
            //    //player with the highest value of the last card wins
            //    handValue.Total = (int)cardsDealer[4].MyValue;
            //    return true;
            //}

        }
        private bool ThreeOfKind()
        {
            //3 одинаковые карты
            //0 - 01, 0 - 12, 0 - 23, 0 - 34
            //1 - 01, 1 - 12, 1 - 23, 1 - 34
            //01 - 0, 01 - 1, 01 - 2, 01 - 3, 01 - 4
            //0 - 01
            if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[0].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsDealer[0].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            //0 - 12
            else if(cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsDealer[0].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            //0 - 23
            else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsPlayer[0].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsDealer[0].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            //0 - 34
            else if (cardsPlayer[0].MyValue == cardsDealer[3].MyValue && cardsPlayer[0].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[0].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[0].MyValue;
                return true;
            }
            //1 - 01
            else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //1 - 12
            else if (cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //1 - 23
            else if (cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //1 - 34
            else if (cardsPlayer[1].MyValue == cardsDealer[3].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //01 - 0
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[0].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //01 - 1
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //01 - 2
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //01 - 3
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            //01 - 4
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[1].MyValue * 3;
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }
            return false;
        }

        private bool TwoPairs()
        {
            //2 пары на столе

            //пара в руке и пара на столе
            // 01 - 01, 01 - 12, 01 - 23, 01 - 34
            // 01 - 01
            if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsDealer[0].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[1].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            // 01 - 12
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsDealer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[2].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            // 01 - 23
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            // 01 - 34
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //Пара с руки 1 и пара на столе
            //0 - 0 & - 12, 0 - 0 & - 23, 0 - 0 & - 34
            //0 - 1 & - 23, 0 - 1 & 34
            //0 - 2 & - 34

            //0 - 0 & - 12
            if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsDealer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[2].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 0 & - 23
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[2].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 0 & - 34
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 1 & - 23
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 1 & - 34
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[2].MyValue;
                return true;
            }
            //0 - 2 & - 34
            else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }

            //Пара с руки 2 и пара на столе
            //1 - 0 & - 12, 1 - 0 & - 23, 1 - 0 & - 34
            //1 - 1 & - 23, 1 - 1 & 34
            //1 - 2 & - 34
            //1 - 0 & - 12
            else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsDealer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[2].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //1 - 0 & - 23
            else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //1 - 0 & - 34
            else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[2].MyValue;
                return true;
            }
            //1 - 1 & - 23
            else if (cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //1 - 1 & - 34
            else if (cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[2].MyValue;
                return true;

            }
            //1 - 2 & - 34
            else if (cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[1].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[1].MyValue;
                return true;
            }


            //пара с руки 1 и пара с руки 2
            // 0 - 0 & 1 - 1,  0 - 0 & 1 - 2,  0 - 0 & 1 - 3,  0 - 0 & 1 - 4,
            // 0 - 1 & 1 - 2,  0 - 1 & 1 - 3, 0 - 1 & 1 - 4,
            // 0 - 2 & 1 - 3,  0 - 2 & 1 - 4,
            //  0 - 3 & 1 - 4,

            //0 - 0 & 1 - 1
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[1].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 0 & 1 - 2
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[2].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 0 & 1 - 3
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 0 & 1 - 4
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[3].MyValue;
                return true;
            }
            //0 - 1 & 1 - 2
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[2].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 1 & 1 - 3
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 1 & 1 - 4
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[3].MyValue;
                return true;
            }
            //0 - 2 & 1 - 3
            else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[3].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[4].MyValue;
                return true;
            }
            //0 - 2 & 1 - 4
            else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[3].MyValue;
                return true;
            }
            //0 - 3 & 1 - 4
            else if (cardsPlayer[0].MyValue == cardsDealer[3].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = ((int)cardsPlayer[0].MyValue * 2) + ((int)cardsDealer[4].MyValue * 2);
                handValue.HighCard = (int)cardsDealer[3].MyValue;
                return true;
            }
            return false;
        }

        private bool OnePair()
        {
            //2 одинаковые карты
            //0 - 0, 0 - 1, 0 - 2, 0 - 3, 0 - 4
            //1 - 0, 1 - 1, 1 - 2, 1 - 3, 1 - 4
            //Пары стола
            // - 01, - 12, - 23, - 34
            // - 01
            //Пары в руке
            // 01 -
            if(cardsPlayer[0].MyValue == cardsPlayer[1].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            else if(cardsDealer[0].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsDealer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // - 12
            if (cardsDealer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsDealer[2].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // - 23
            if (cardsDealer[2].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsDealer[3].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // - 34
            if (cardsDealer[3].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsDealer[4].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // 0 - 0
            else if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // 0 - 1
            else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // 0 - 2
            else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // 0 - 3
            else if (cardsPlayer[0].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // 0 - 4
            else if (cardsPlayer[0].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsPlayer[0].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            // 1 - 0
            else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            // 1 - 1
            else if (cardsPlayer[1].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            // 1 - 2
            else if (cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            // 1 - 3
            else if (cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            // 1 - 4
            else if (cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 2;
                handValue.HighCard = (int)cardsPlayer[0].MyValue;
                return true;
            }
            return false;
        }
    }
}
