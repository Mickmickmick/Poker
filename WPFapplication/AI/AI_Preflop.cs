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

        private int eval_preflop(Dealer Mr_Brown)
        {
            firstTimeFlop = false;
            int amount = Mr_Brown.players[0].PotCommit;
            double perc = opponentmodel.addPreflopCommit(amount);

            double closest = 1;
            for (int i = 0; i < Occurrencies.Count; ++i)
            {
                if (Math.Abs(Occurrencies[i] - amount) < Math.Abs(closest - amount)) closest = Occurrencies[i];
            }
            return Occurrencies.FindIndex(k => k == closest) + 1;
        }

        private int preflop_action(Dealer Mr_Brown)
        {            
            firstTimeFlop = true;
            int tier = RankHoleCards(Mr_Brown.players[1].HoleCards);
            if (am_dealer)
                return preflop_inpos(Mr_Brown, tier);
            return preflop_outpos(Mr_Brown, tier, Mr_Brown.players[0].PotCommit == Mr_Brown.players[1].PotCommit);            
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
            int opponentBet = Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit;
            // is this the first action?
            bool firstbet = Mr_Brown.players[1].PotCommit == 10;

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
            int opponentBet = Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit;

 

            if (tier == 1)
                return opponentBet * 3;
            if (tier < 4)
            {
                return opponentBet;
            }
            if (tier < 9 && opponentBet <= 50)
                return opponentBet;
            return 0;
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
