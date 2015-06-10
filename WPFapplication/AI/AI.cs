using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameMechanics;
using System.Threading;
using System.Diagnostics;

namespace Villain
{
    public partial class AI
    {
        private int lastBet = 0;

        //Function to get random number
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(0, max);
            }
        }

        Array ranks = Enum.GetValues(typeof(CARD_RANK));
        Array suits = Enum.GetValues(typeof(CARD_SUIT));

        OpponentModel_FIRST_ORDER opponentmodel = new OpponentModel_FIRST_ORDER();

        GAME_STATE _phase;
        GAME_STATE phase
        {
            get
            {
                return _phase;
            }
            set
            {
                firstMoveOfRound = !(_phase == value);
                _phase = value;
            }
        }
        bool am_dealer;
        bool firstMoveOfRound;

        bool firstTimeFlop = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public AI()
        {
            ALL_TIERS.Add(Tier1);
            ALL_TIERS.Add(Tier2);
            ALL_TIERS.Add(Tier3);
            ALL_TIERS.Add(Tier4);
            ALL_TIERS.Add(Tier5);
            ALL_TIERS.Add(Tier6);
            ALL_TIERS.Add(Tier7);
            ALL_TIERS.Add(Tier8);
        }

        public void UpdateMove(MOVE move, GAME_STATE gamestate, int betTurn, bool newRound)
        {
            opponentmodel.UpdateMove(move, gamestate, betTurn, newRound);
        }

        /// <summary>
        /// MAIN - Big wrapper function
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        public int GetMove(Dealer Mr_Brown, int betStep)
        {
            phase = DeterminePhase(Mr_Brown);
            am_dealer = Mr_Brown.dealer_button.PlayerID == PLAYER.Computer;

            if (firstMoveOfRound && lastBet > 0)
            {
                if (phase == GAME_STATE.turn)
                    opponentmodel.addFlopRangeCommitPASSIVE(lastBet);
                if (phase == GAME_STATE.river)
                    opponentmodel.addTurnRangeCommitPASSIVE(lastBet);
                if (phase == GAME_STATE.wrapup)
                    opponentmodel.addRiverRangeCommitPASSIVE(lastBet);
            }

            int amount = 0;

            if (phase == GAME_STATE.preflop)
                amount = preflop_action(Mr_Brown);

            else
            {
                if (firstTimeFlop)
                    opponentmodel.currentRange = eval_preflop(Mr_Brown);

                amount = postflop_action(Mr_Brown);
            }

            #region amount-verification
            int opponentBet = Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit;
            int small_stack = opponentBet + Mr_Brown.players[0].StackSize < Mr_Brown.players[1].StackSize ? Mr_Brown.players[0].StackSize : Mr_Brown.players[1].StackSize;
            
            // if raised
            if (Mr_Brown.players[1].PotCommit + amount > Mr_Brown.players[0].PotCommit)
            {
                if ((Mr_Brown.players[1].PotCommit + amount) - Mr_Brown.players[0].PotCommit < 20)
                    amount = 20;
            }

            if (amount < 20 && amount > 10)
                amount = 20;

            // a call of 10 is possible preflop on the button
            if (amount <= 10 && Mr_Brown.players[1].PotCommit != 10)                
                amount = 0;

            if (amount > small_stack)
                amount = small_stack;
            #endregion

            // if AI betted
            int bet = (amount + Mr_Brown.players[1].PotCommit) -  Mr_Brown.players[0].PotCommit;
            lastBet = bet > 0 ? bet : 0;

            return amount;

        }


        private GAME_STATE DeterminePhase(Dealer Mr_Brown)
        {
            if(Mr_Brown.c.Count == 0)
                return GAME_STATE.preflop;
            if (Mr_Brown.c.Count == 3)
                return GAME_STATE.flop;
            if (Mr_Brown.c.Count == 4)
                return GAME_STATE.turn;
            else
                return GAME_STATE.river;
        }

        /// <summary>
        /// Generate random card not in list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Card GenerateCard(List<Card> list)
        {
            Card Generated = new Card((CARD_SUIT)suits.GetValue(RandomNumber(suits.Length)), (CARD_RANK)ranks.GetValue(RandomNumber(ranks.Length)));
            if(!IsThere(list, Generated))
                return Generated;
            return GenerateCard(list);
        }

        /// <summary>
        /// Returns true if such a card is in list
        /// </summary>
        /// <param name="inlist"></param>
        /// <param name="iscard"></param>
        /// <returns></returns>
        private bool IsThere(List<Card> inlist, Card iscard)
        {
            foreach (Card c in inlist)
            {
                if (c.rank == iscard.rank && c.suit == iscard.suit)
                    return true;
            }
            return false;
        }


        private bool isSuited(CardList hand)
        {
            if (hand[0].suit == hand[1].suit)
                return true;
            return false;
        }

        private void sortByRank(CardList hand)
        {
            if (hand[0].rank < hand[1].rank)
                hand.Reverse();
        }

        #region Old database system

        /// <summary>
        /// Assign an integer as quality rank of the hole cards from 1 through 9
        /// 1 being best, 9 being worst
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private int RankHoleCards(CardList hand)
        {
            sortByRank(hand);
            bool suited = isSuited(hand);

            //TOP TIER
            if (IsHandType(hand, suited, Tier1))
                return 1;
            if (IsHandType(hand, suited, Tier2))
                return 2;
            if (IsHandType(hand, suited, Tier3))
                return 3;
            if (IsHandType(hand, suited, Tier4))
                return 4;
            if (IsHandType(hand, suited, Tier5))
                return 5;
            if (IsHandType(hand, suited, Tier6))
                return 6;
            if (IsHandType(hand, suited, Tier7))
                return 7;
            //BOTTOM TIER
            if (IsHandType(hand, suited, Tier8))
                return 8;
            //REST
            return 9;

        }

        /// <summary>
        /// Helper for determining the quality of the hole cards
        /// Loops through a list of tuples and sees whether or not its there
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="isHandSuited"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private bool IsHandType(CardList hand, bool isHandSuited, List<Tuple<CARD_RANK, CARD_RANK, bool>> db)
        {
            foreach (Tuple<CARD_RANK, CARD_RANK, bool> t in db)
                if (IsHandType(hand, isHandSuited, t))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks whether or not the given hand with bool matches with another hand
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="isHandSuited"></param>
        /// <param name="db"></param>
        /// <returns></returns> 
        private bool IsHandType(CardList hand, bool isHandSuited, Tuple<CARD_RANK, CARD_RANK, bool> db)
        {
            if(hand[0].rank == db.Item1 && (db.Item2 == null || hand[1].rank == db.Item2) && isHandSuited == db.Item3)
                return true;
            return false;
        }

        #region database

        List<double> Occurrencies = new List<double>(9){ 
            28 / 1326.0,                                        // Tier1
            (28 + 30) / 1326.0,                                 // Tier2
            (28 + 30 + 34) / 1326.0,                            // Tier3
            (28 + 30 + 34 + 50) / 1326.0,                       // Tier4
            (28 + 30 + 34 + 50 + 98) / 1326.0,                  // Tier5
            (28 + 30 + 34 + 50 + 98 + 68) / 1326.0,             // Tier6
            (28 + 30 + 34 + 50 + 98 + 68 + 96) / 1326.0,        // Tier7
            (28 + 30 + 34 + 50 + 98 + 68 + 96 + 128) / 1326.0,  // Tier8
            1                                                   // Tier9
        };


        //AA, KK, AKs, QQ, AK
        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier1 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(5)
            {
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.ace, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.king, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.queen, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.jack, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.king, true)
            };

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier2 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(5)
            {
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ten, CARD_RANK.ten, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.queen, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.jack, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.queen, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.king, false)
            };

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier3 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(6)
            {
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.nine, CARD_RANK.nine, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.ten, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.jack, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.jack, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.ten, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.queen, false)
            };

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier4 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(8)
            {
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.eight, CARD_RANK.eight, false), //Pocket pair
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.ten, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.ten, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.nine, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ten, CARD_RANK.nine, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.nine, CARD_RANK.eight, true),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.jack, false),
                new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.queen, false)
            };

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier5 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(18)
            {
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.seven, CARD_RANK.seven,false), //Pocket pair
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.nine,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.eight,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.seven,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.six,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.five,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.four,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.three,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.two,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.nine,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ten, CARD_RANK.eight,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.nine, CARD_RANK.seven,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.eight, CARD_RANK.seven,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.nine, CARD_RANK.seven,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.seven, CARD_RANK.six,true),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.jack,false),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.jack,false),
				new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.ten,false)
            };

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier6 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(10)
            {								
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.six, CARD_RANK.six,false), //Pocket pair
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.five, CARD_RANK.five,false), //Pocket pair
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.nine,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.eight,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.eight, CARD_RANK.six,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.seven, CARD_RANK.five,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.five, CARD_RANK.four,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.ten,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.ten,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.ten,false)
			};

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier7 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(18)
            {								
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.four, CARD_RANK.four,false), //Pocket pair
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.three, CARD_RANK.three,false), //Pocket pair
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.two, CARD_RANK.two,false), //Pocket pair
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.eight,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.seven,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.six,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.five,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.four,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.three,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.two,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.eight,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ten, CARD_RANK.seven,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.six, CARD_RANK.four,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.five, CARD_RANK.three,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.four, CARD_RANK.three,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.nine,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ten, CARD_RANK.nine,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.nine, CARD_RANK.eight,false)
			};

        List<Tuple<CARD_RANK, CARD_RANK, bool>> Tier8 =
            new List<Tuple<CARD_RANK, CARD_RANK, bool>>(15)
            {								
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.seven,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.nine, CARD_RANK.six,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.eight, CARD_RANK.five,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.seven, CARD_RANK.four,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.four, CARD_RANK.two,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.three, CARD_RANK.two,true),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ace, CARD_RANK.nine,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.king, CARD_RANK.nine,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.queen, CARD_RANK.nine,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.jack, CARD_RANK.eight,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.ten, CARD_RANK.eight,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.eight, CARD_RANK.seven,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.seven, CARD_RANK.six,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.six, CARD_RANK.five,false),
			new Tuple<CARD_RANK,CARD_RANK,bool>(CARD_RANK.five, CARD_RANK.four,false)
			};

        List<List<Tuple<CARD_RANK, CARD_RANK, bool>>> ALL_TIERS = new List<List<Tuple<CARD_RANK, CARD_RANK, bool>>>();
        #endregion

        #endregion
    }

  
}
