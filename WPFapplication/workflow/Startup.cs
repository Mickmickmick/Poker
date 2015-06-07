using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Cards;

namespace GameMechanics
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            GUI.Table table = new GUI.Table();

            table.Show();
            table.tester();

            int begin_stack = 1500;
            int small_blind_size = 10;

            // init players
            List<Player> Participants = new List<Player>();           
            foreach (PLAYER p in Enum.GetValues(typeof(PLAYER)))            
                Participants.Add(new Player(p, begin_stack));

            // init dealer
            Dealer Mr_Brown = new Dealer(Participants/*, table*/);

            // Who starts as dealer?
            Random rnd = new Random();
            int dealer_button = rnd.Next(0,1);

            // play the game until someone's out of chips
            while (Mr_Brown.players[0].StackSize != 0 && Mr_Brown.players[1].StackSize != 0)
            {                
                int table_size = Participants.Count;
                dealer_button = dealer_button >= table_size - 1 ? 0 : dealer_button + 1;
                PlayHand(Mr_Brown, Participants, small_blind_size, dealer_button, table );
            }
            
        }

        private static void PlayHand(Dealer Mr_Brown, List<Player> Participants, int SmallBlindSize, int PlayerToDeal , GUI.Table table)
        {
           // Thread.Sleep(5000);

            int big_blind_player = (PlayerToDeal + 1) % 2;
            int BigBlindSize = 2*SmallBlindSize;

            /* PreFop action    */
            PreFlop(Mr_Brown, Participants, SmallBlindSize, BigBlindSize, PlayerToDeal, table);
            if (WrapUp(Mr_Brown, table))
                return;        

            /* Flop     */
            Flop(Mr_Brown, Participants, SmallBlindSize, BigBlindSize, big_blind_player, table);
            if (WrapUp(Mr_Brown, table))
                return;

            /* Turn     */
            Turn(Mr_Brown, Participants, SmallBlindSize, BigBlindSize, big_blind_player, table);
            if (WrapUp(Mr_Brown, table))
                return;

            /* River    */
            River(Mr_Brown, Participants, SmallBlindSize, BigBlindSize, big_blind_player, table);
            if (WrapUp(Mr_Brown, table))
                return;

            /* Showdown */
            Mr_Brown.ShowDown();
        }

        private static void River(Dealer Mr_Brown, List<Player> Participants, int SmallBlindSize, int BigBlindSize, int FirstPlayer, GUI.Table table)
        {
            Mr_Brown.River();
            BetRound(Mr_Brown, BigBlindSize, FirstPlayer, table);
        }

        private static void Turn(Dealer Mr_Brown, List<Player> Participants, int SmallBlindSize, int BigBlindSize, int FirstPlayer, GUI.Table table)
        {
            Mr_Brown.Turn();
            BetRound(Mr_Brown, BigBlindSize, FirstPlayer, table);
        }

        private static bool WrapUp(Dealer Mr_Brown, GUI.Table table)
        {
            // If there's different investments, higher investment wins
            if (Mr_Brown.players[0].PotCommit != Mr_Brown.players[1].PotCommit)
            {
                // Check which one is higher
                int winner = Mr_Brown.players[0].PotCommit > Mr_Brown.players[1].PotCommit ? 0 : 1;
                Mr_Brown.WrapUp(Mr_Brown.players[winner]);
                return true;
            }
            table.HideCards();
            return false;
        }

        private static void PreFlop(Dealer Mr_Brown, List<Player> Participants, int SmallBlindSize, int BigBlindSize, int PlayerToDeal, GUI.Table table)
        {
 	        Mr_Brown.AcceptBet(Participants[PlayerToDeal], SmallBlindSize);
            Mr_Brown.AcceptBet(Participants[(PlayerToDeal+1) % 2], BigBlindSize);
            table.ShowCards();           
            Mr_Brown.DealHoleCards(table);            

            BetRound(Mr_Brown, BigBlindSize, PlayerToDeal, table);
        }

        private static void Flop(Dealer Mr_Brown, List<Player> Participants, int SmallBlindSize, int BigBlindSize, int FirstPlayer, GUI.Table table)
        {
            Mr_Brown.Flop();
            BetRound(Mr_Brown, BigBlindSize, FirstPlayer, table);
        }

        private static void BetRound(Dealer Mr_Brown, int BigBlindSize, int PlayersTurn, GUI.Table table)
        {
          //  Thread.Sleep(40000);
            bool betting_finished = false;
            int step = 0;

            while (!betting_finished)
            {
                if (Mr_Brown.players[PlayersTurn].PotCommit < Mr_Brown.players[(PlayersTurn + 1) % 2].PotCommit)
                {
                    Ask_FoldCallRaise(Mr_Brown, PlayersTurn, table);
                    if (Mr_Brown.players[PlayersTurn].PotCommit <= Mr_Brown.players[(PlayersTurn + 1) % 2].PotCommit)
                        betting_finished = true;
                }
                else
                {
                    Ask_CheckRaise(Mr_Brown, PlayersTurn, table);
                    if (Mr_Brown.players[PlayersTurn].PotCommit == Mr_Brown.players[(PlayersTurn + 1) % 2].PotCommit && step > 0)
                        betting_finished = true;
                }

                step++;
                PlayersTurn = (PlayersTurn + 1) % 2;
            }
        }

        private static void Ask_CheckRaise(Dealer Mr_Brown, int PlayersTurn, GUI.Table table)
        {
            int AddToPot = 0;
            // If player = user
            if (PlayersTurn == 0)
                AddToPot = UserInteraction.CheckRaise(Mr_Brown, table);

            Mr_Brown.AcceptBet(Mr_Brown.players[PlayersTurn], AddToPot);
        }

        private static void Ask_FoldCallRaise(Dealer Mr_Brown, int PlayersTurn, GUI.Table table)
        {
            int AddToPot = 0;
            // If player = user
            if (PlayersTurn == 0)
                AddToPot = UserInteraction.FoldCallRaise(Mr_Brown, table);
            else
                AddToPot = Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit;

            Mr_Brown.AcceptBet(Mr_Brown.players[PlayersTurn], AddToPot);
        }        
    }
}
