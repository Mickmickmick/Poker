using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameMechanics;

namespace Villain
{
    /// <summary>
    /// This class takes care of all preflop action
    /// </summary>
    partial class AI
    {
        private int pfc;

        public void preflopcommit(Dealer Mr_Brown)
        {
            int amount = Mr_Brown.players[op].PotCommit;
            if (Mr_Brown.c.Count == 0)
                pfc = amount;
        }

        private int eval_preflop(Dealer Mr_Brown)
        {
            firstTimeFlop = false;
            int amount = pfc;
            double perc = opponentmodel.addPreflopCommit(amount);
            commits_ranges_current_hand.Add(perc);
            double closest = 1;
            for (int i = 0; i < Occurrencies.Count; ++i)
            {
                if (Math.Abs(Occurrencies[i] - perc) < Math.Abs(closest - perc)) closest = Occurrencies[i];
            }
            return Occurrencies.FindIndex(k => k == closest) + 1;
        }

        private int preflop_action(Dealer Mr_Brown)
        {            
            firstTimeFlop = true;
            int tier = RankHoleCards(Mr_Brown.players[tp].HoleCards);
            if (am_dealer)
                return preflop_inpos(Mr_Brown, tier);
            return preflop_outpos(Mr_Brown, tier, Mr_Brown.players[op].PotCommit == Mr_Brown.players[tp].PotCommit);            
        }

        #region Dealer button
        /// <summary>
        /// Dealer button, initiative automatically there
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <param name="tier"></param>
        /// <param name="initiative"></param>
        /// <returns></returns>
        private int preflop_inpos(Dealer Mr_Brown, int tier)
        {
            // amount to call
            int opponentBet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            // is this the first action?
            bool firstbet = Mr_Brown.players[tp].PotCommit == 10;

            // 3 bet
            if (tier <= 3)
                return opponentBet * 3 + (firstbet ? 20 : 0);
            // 2 bet
            if (tier < 7)
            {
                if (firstbet)
                    return opponentBet + 20;
                // call
                if (opponentBet <= 60)
                    return opponentBet;
                // really high bet
                else
                    return 0;
            }
            if (tier == 8 && !firstbet && opponentBet <= 40)
                return opponentBet;
            // limp
            if (opponentBet <= 20)
            {
                double chance = RandomNumber(100) * opponentmodel.loosenessFactor;
                if (chance > 65)
                    return 30;
                if (chance > 35)
                    return 10;                
            }
            return 0;
        }
        #endregion

        #region No dealer button
        /// <summary>
        /// No dealer button
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <param name="tier"></param>
        /// <returns></returns>
        private int preflop_outpos(Dealer Mr_Brown, int tier, bool initiative)
        {
            if (initiative)
                return preflop_outpos_initiative_here(Mr_Brown, tier);
            return preflop_outpos_initiative_there(Mr_Brown, tier);
        }

        /// <summary>
        /// No dealer button, opponent betted/raised (fold/call/raise)
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <param name="tier"></param>
        /// <returns></returns>
        private int preflop_outpos_initiative_there(Dealer Mr_Brown, int tier)
        {
            int opponentBet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;

 

            if (tier == 1)
                return opponentBet * 3;
            if (tier < 4)
            {
                return opponentBet;
            }
            if (tier < 9 && opponentBet <= 50)
                return opponentBet;
            int r = RandomNumber(10) >= 5 && opponentBet <= 55 ? opponentBet : 0;
            return r;
        }

        /// <summary>
        /// No dealer button, opponent limped (check/bet)
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <param name="tier"></param>
        /// <returns></returns>
        private int preflop_outpos_initiative_here(Dealer Mr_Brown, int tier)
        {

            if (tier <= 3)
                return 40;
            if (tier <= 7)
                return 20;
            return 0;
        }
        #endregion
    }
}
