using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer
{

    class MoneySystem
    {
        public static int SmallBlind;
        public static int BigBlind;

        public static int PlayerMoney;
        public static int GeneralBet;
        public static int GeneralEnemyBet;
        
        public static int betPlayer;//То что поставил игрок я ссать

        public int ChoosePlayer(int playerMoney, int generalBet, int generalEnemyBet, int smallBlind, int bigBlind)
        {
            PlayerMoney = playerMoney;
            GeneralEnemyBet = generalEnemyBet;
            GeneralBet = generalBet;

            SmallBlind = smallBlind;
            BigBlind = bigBlind;
            
            int choosen;

            bool trueChoice = true;

            Console.WriteLine("\nВыберите свой ход: Check - 0, Call - 1, Raise - 2, Fold - 3");
            choosen = Convert.ToInt32(Console.ReadLine());

            while (trueChoice)
            {
                switch (choosen)
                {
                    case 0:
                        betPlayer = Check();
                        trueChoice = false;
                        break;
                    case 1:
                        betPlayer = Call();
                        trueChoice = false;
                        break;
                    case 2:
                        if(PlayerMoney >= BigBlind)
                        { 
                            betPlayer = Raise(); 
                            trueChoice = false; 
                        }     
                        break;
                    case 3:
                        betPlayer = Fold();
                        trueChoice = false;
                        break;
                    default:
                        Console.WriteLine("Вы ввели неверное значение выбора");
                        choosen = Convert.ToInt32(Console.ReadLine());
                        break;
                }
            }

            return betPlayer;
        }

        public static int Check()//Чек ничего не ставить, если ставки равны, если ставка противника больше, фолд
        {
            if(betPlayer < GeneralEnemyBet)
            {
                if(PlayerMoney >= GeneralEnemyBet - betPlayer)
                {
                    return betPlayer = GeneralEnemyBet - betPlayer;//Если хватает на повышение ставки
                }
                else
                {
                    return betPlayer = PlayerMoney;//Если не хватает на повышение ставки -> ставится все
                }
            }
            return GeneralBet - betPlayer;
        }

        public static int Call()//Уровнять ставку со ставкой противника
        {
            if(betPlayer < GeneralEnemyBet)
            {
                if (PlayerMoney >= GeneralEnemyBet - betPlayer)//Если хватает на повышение ставки
                {
                    return betPlayer = GeneralEnemyBet - betPlayer;
                }
                else
                {
                    return betPlayer = PlayerMoney;//Если не хватает на повышение ставки -> ставится все
                }
            }
            return GeneralBet - betPlayer;
        }

        public static int Raise()
        {
            Console.WriteLine("На сколько денег Вы хотите повысить ставку?");
            bool choice = false;
            while (!choice)
            {
                int valueRaise = Convert.ToInt32(Console.ReadLine());
                if (valueRaise <= PlayerMoney && valueRaise >= BigBlind)
                {
                    PlayerMoney -= valueRaise;
                    betPlayer += valueRaise;
                    choice = true;
                }
                else { Console.WriteLine("Вы ввели некоректную ставку"); }
            }
            return GeneralBet - betPlayer;
        }

        public static int Fold()
        {
            Program.roundContinue = false;
            return 0;
        }

    }
}
