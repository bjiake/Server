using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    public enum Hand
    {
        HighCard,
        OnePair,
        TwoPairs,
        ThreeOfKind,
        Straight,
        Flush,
        FullHouse,
        FourOfKind,
        StraightFlush,
        FlushRoyal
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

        private HandValue handValue;

        private Card pairCard;
        private Card threeOfKindCard;
        private Card[] deck;

        public HandEvaluator(Card[] sortedHand, Card[] sortedDealerCards, Card[] sortedDeckCards)
        {
            heartsSum = 0;
            diamondSum = 0;
            clubSum = 0;
            spadesSum = 0;

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
                deckCards[0] = value[0];
                deckCards[1] = value[1];
                deckCards[2] = value[2];
                deckCards[3] = value[3];
                deckCards[4] = value[4];
                deckCards[5] = value[5];
                deckCards[6] = value[6];
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
            if (FlushRoyal()) { return Hand.FlushRoyal; }
            else if(StraightFlush()) { return Hand.StraightFlush; }
            else if (FourOfKind()) { return Hand.FourOfKind; } // Работает
            else if (FullHouse()) { return Hand.FullHouse; }//Работает
            else if (Flush()) { return Hand.Flush; }//работает
            else if (Straight()) { return Hand.Straight; }//Работает
            else if (ThreeOfKind()) { return Hand.ThreeOfKind; }//Работает
            else if (TwoPairs()) { return Hand.TwoPairs; }//Работает
            else if (OnePair()) { return Hand.OnePair; }//Работает

            else { handValue.HighCard = (int)cardsPlayer[1].MyValue; }//Сделать подсчет сильной карты
            return Hand.HighCard;
        }

        private void GetNumberOfSuit()
        {
            foreach (var element in DeckCards)
            {
                if (element.MySuit == Card.SUIT.Hearts) { heartsSum++; }
                else if (element.MySuit == Card.SUIT.Diamonds) { diamondSum++; }
                else if (element.MySuit == Card.SUIT.Clubs) { clubSum++; }
                else if (element.MySuit == Card.SUIT.Spades) { spadesSum++; }
            }
        }
        private bool FlushRoyal()
        {
            if(StraightFlush())
            {
                if((int)handValue.Total == 24 && (deck[6].MySuit == deck[5].MySuit || deck[6].MySuit == deck[4].MySuit || deck[6].MySuit == deck[3].MySuit))
                {
                    return true;
                }
            }
            return false;
        }
        private bool StraightFlush()
        {
            if (heartsSum >= 5 || spadesSum >= 5 || diamondSum >= 5 || clubSum >= 5)
            {
                int count = 0;
                int k;
                deck = new Card[7];
                for (int i = 0; i < 6; i++)
                {
                    if (deckCards[i].MyValue != deckCards[i + 1].MyValue)
                    {
                        deck[count] = deckCards[i];
                        count++;
                    }
                }

                if (count > 4)
                {
                    k = 1;
                    for (int i = 1; i < count; i++)
                    {
                        if (deck[i].MyValue - deck[i - 1].MyValue == 1 && deck[i].MySuit == deck[i - 1].MySuit)
                        {
                            k++;
                            handValue.Total = (int)deckCards[i].MyValue + 1;
                            handValue.HighCard = (int)deckCards[i].MyValue + 1;
                        }
                        else if
                        (
                         CheckForStraightAce(deck) == 24 &&
                        (int)deck[0].MyValue == 12 &&
                        (int)deck[1].MyValue == 13 &&
                        (int)deck[2].MyValue == 14 &&
                        (int)deck[3].MyValue == 15 &&
                        deck[0].MySuit == deck[1].MySuit &&
                        deck[0].MySuit == deck[2].MySuit &&
                        deck[0].MySuit == deck[3].MySuit &&
                        deck[0].MySuit == deck[4].MySuit
                        )
                        {
                            handValue.Total = 15;
                            handValue.HighCard = 15;
                            return true;
                        }
                        //else return false;
                    }
                    if (k > 4)
                    {
                        return true;
                    }

                }
            }
            return false;
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
            else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
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
            //01 - 01 
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //01 - 12
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //02 - 23
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            //02 - 34
            else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            {
                handValue.Total = (int)cardsPlayer[1].MyValue * 4;
                handValue.HighCard = (int)cardsPlayer[1].MyValue;
                return true;
            }
            return false;
        }
        private bool FullHouse()
        {
            int totalThreeOfKind;
            if(ThreeOfKind())
            {
                totalThreeOfKind = handValue.Total;
                for (int i = 6; i > 0; i--)
                {
                    if (deckCards[i].MyValue == deckCards[i - 1].MyValue && deckCards[i].MyValue != threeOfKindCard.MyValue)
                    {
                        handValue.Total = (int)deckCards[i].MyValue * 2 + totalThreeOfKind;
                        handValue.HighCard = (int)deckCards[i].MyValue * 2;
                        return true;
                    }
                }
            }
            return false;
        }
    
        private bool Flush()
        {
            //if all suits are the same
            if (heartsSum >= 5)
            {
                for (int i = 6; i>= 4; i--)
                {
                    if (deckCards[i].MySuit == Card.SUIT.Hearts)
                    {
                        handValue.Total = (int)deckCards[i].MyValue;
                        return true;
                    }
                }
            }
            else if (diamondSum >= 5)
            {
                for (int i = 6; i >= 4; i--)
                {
                    if (deckCards[i].MySuit == Card.SUIT.Diamonds)
                    {
                        handValue.Total = (int)deckCards[i].MyValue;
                        return true;
                    }
                }
            }
            else if (clubSum >= 5)
            {
                for (int i = 6; i >= 4; i--)
                {
                    if (deckCards[i].MySuit == Card.SUIT.Clubs)
                    {
                        handValue.Total = (int)deckCards[i].MyValue;
                        return true;
                    }
                }
            }
            else if (spadesSum >= 5)
            {
                for (int i = 6; i >= 4; i--)
                {
                    if (deckCards[i].MySuit == Card.SUIT.Spades)
                    {
                        handValue.Total = (int)deckCards[i].MyValue;
                        return true;
                    }
                }
            }
            return false;
        }


        private bool Straight()
        {
            int count = 0;
            int k;
            deck = new Card[7];
            for (int i = 0; i < 6; i++)
            {
                if (deckCards[i].MyValue != deckCards[i + 1].MyValue)
                {
                    deck[count] = deckCards[i];
                    count++;
                }
            }
            if (count > 4)
            {
                k = 1;
                for (int i = 1; i < count; i++)
                {
                    if (deck[i].MyValue - deck[i - 1].MyValue == 1)
                    {
                        k++;
                        handValue.Total = (int)deckCards[i].MyValue + 1;
                        handValue.HighCard = (int)deckCards[i].MyValue + 1;
                    }
                    else if
                    (
                     CheckForStraightAce(deck) == 24 &&
                    (int)deck[0].MyValue == 12 &&
                    (int)deck[1].MyValue == 13 &&
                    (int)deck[2].MyValue == 14 &&
                    (int)deck[3].MyValue == 15
                    )
                    {
                        handValue.Total = 15;
                        handValue.HighCard = 15;
                        return true;
                    }
                    //else return false;
                }
                if (k > 4)
                {
                    return true;
                }
                
            }
            return false;
            
        }
        private int CheckForStraightAce(Card[] deck)
        {
            int maxValue = 0;
            int count = 6;
            for(int i = 0; i < deck.Length; i ++)
            {
                if (deck[i]== null)
                {
                    count--;
                }
            }
            for (int i = count; i > deck.Length; i--)
            {
                if ((int)deck[i].MyValue > maxValue)
                {
                    maxValue = (int)deck[i].MyValue;
                }
            }
            return maxValue;
        }
        private bool ThreeOfKind()
        {
            for (int i = 6; i > 1; i--)
            {
                if (deckCards[i].MyValue == deckCards[i - 1].MyValue && deckCards[i].MyValue == deckCards[i - 2].MyValue)
                {
                    handValue.Total = (int)deckCards[i].MyValue * 3;
                    if (deckCards[i].MyValue != cardsPlayer[1].MyValue) { handValue.HighCard = (int)cardsPlayer[1].MyValue; }
                    else if (deckCards[i].MyValue != cardsPlayer[0].MyValue) { handValue.HighCard = (int)cardsPlayer[0].MyValue; }
                    threeOfKindCard = deckCards[i];
                    return true;
                }
            }
            return false;
            //3 одинаковые карты
            //0 - 01, 0 - 12, 0 - 23, 0 - 34
            //1 - 01, 1 - 12, 1 - 23, 1 - 34
            //01 - 0, 01 - 1, 01 - 2, 01 - 3, 01 - 4
            //0 - 01
            //if (cardsPlayer[0].MyValue == cardsDealer[0].MyValue && cardsPlayer[0].MyValue == cardsDealer[1].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[0].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[0].MyValue;
            //    return true;
            //}
            ////0 - 12
            //else if (cardsPlayer[0].MyValue == cardsDealer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[0].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[0].MyValue;
            //    return true;
            //}
            ////0 - 23
            //else if (cardsPlayer[0].MyValue == cardsDealer[2].MyValue && cardsPlayer[0].MyValue == cardsDealer[3].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[0].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[0].MyValue;
            //    return true;
            //}
            ////0 - 34
            //else if (cardsPlayer[0].MyValue == cardsDealer[3].MyValue && cardsPlayer[0].MyValue == cardsDealer[4].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[0].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[0].MyValue;
            //    return true;
            //}
            ////1 - 01
            //else if (cardsPlayer[1].MyValue == cardsDealer[0].MyValue && cardsPlayer[1].MyValue == cardsDealer[1].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////1 - 12
            //else if (cardsPlayer[1].MyValue == cardsDealer[1].MyValue && cardsPlayer[1].MyValue == cardsDealer[2].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////1 - 23
            //else if (cardsPlayer[1].MyValue == cardsDealer[2].MyValue && cardsPlayer[1].MyValue == cardsDealer[3].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////1 - 34
            //else if (cardsPlayer[1].MyValue == cardsDealer[3].MyValue && cardsPlayer[1].MyValue == cardsDealer[4].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////01 - 0
            //else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[0].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////01 - 1
            //else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[1].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////01 - 2
            //else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[2].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////01 - 3
            //else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[3].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            ////01 - 4
            //else if (cardsPlayer[0].MyValue == cardsPlayer[1].MyValue && cardsPlayer[0].MyValue == cardsDealer[4].MyValue)
            //{
            //    handValue.Total = (int)cardsDealer[1].MyValue * 3;
            //    handValue.HighCard = (int)cardsDealer[1].MyValue;
            //    return true;
            //}
            //return false;
        }

        private bool TwoPairs()
        {
            int total;
            if (OnePair())
            {
                total = handValue.Total;
                for (int i = 6; i >= 1; i--)
                {
                    if (deckCards[i].MyValue == deckCards[i - 1].MyValue && deckCards[i] != pairCard)
                    {
                        handValue.Total = (int)deckCards[i].MyValue * 2 + total;
                        return true;
                    }
                }
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
            for(int i = 6; i > 0; i--)
            {
                if (deckCards[i].MyValue == deckCards[i - 1].MyValue)
                {
                    handValue.Total = (int)deckCards[i].MyValue * 2;
                    if (deckCards[i].MyValue != cardsPlayer[1].MyValue) { handValue.HighCard = (int)cardsPlayer[1].MyValue; }
                    else if (deckCards[i].MyValue != cardsPlayer[0].MyValue) { handValue.HighCard = (int)cardsPlayer[0].MyValue; }
                    pairCard = deckCards[i];
                    return true;
                }
            }
            return false;
        }
    }
}
