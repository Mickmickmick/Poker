using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameMechanics;
using System.Threading.Tasks;

namespace Villain
{
    partial class AI
    {
        private int postflop_action(Dealer Mr_Brown)
        {
            if (phase == GAME_STATE.flop)
                return flop_action(Mr_Brown);
            if (phase == GAME_STATE.turn)
                return turn_action(Mr_Brown);            
            return river_action(Mr_Brown);
        }

        #region FLOP

        /// <summary>
        /// Active / passive bit is about the opponents move(s) this round! 
        /// Has he betted? -> active
        /// Has he checked / not moved yet? -> passive
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int flop_action(Dealer Mr_Brown)
        {
            double potpercbet = (double)(Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit) / (double)(2 * Mr_Brown.players[tp].PotCommit);
            // Checked towards me
            if (am_dealer && Mr_Brown.players[op].PotCommit == Mr_Brown.players[tp].PotCommit)
            {
                commits_ranges_current_hand.Add(opponentmodel.addFlopRangeCommitACTIVE(0));
                return flop_action_inpos_passive(Mr_Brown);
            }
            // Betted / raised towards me
            if (am_dealer)
            {
                commits_ranges_current_hand.Add(opponentmodel.addFlopRangeCommitACTIVE(potpercbet));
                return flop_action_inpos_active(Mr_Brown);
            }
            // My move; no action yet
            if (firstMoveOfRound)
                return flop_action_outpos_passive(Mr_Brown);
            // My move; respond to bet / raise
            else
            {
                commits_ranges_current_hand.Add(opponentmodel.addFlopRangeCommitPASSIVE(potpercbet));
                return flop_action_outpos_active(Mr_Brown);
            }
        }

        /// <summary>
        /// Checked towards me
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int flop_action_inpos_passive(Dealer Mr_Brown)
        {
            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceFLOP(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;

            if (fold_or_win_chance >= 0.60)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.60) * 10);
            if (fold_or_win_chance >= 0.40)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.40) * 10);

            if (opponentmodel.CurrentAggresiveness() / opponentmodel.AverageAggresiveness() < 0.7)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.50) * 10);

            return 0;
        }

        /// <summary>
        /// Raised towards me
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int flop_action_inpos_active(Dealer Mr_Brown)
        {
            int opponent_bet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            double opponent_commit = opponent_bet / (double)Mr_Brown.pot;

            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceFLOP(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;

            if (fold_or_win_chance > 0.80 || fold_or_win_chance >= opponent_commit * 2)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.60) * 10);

            // if winchance is about equal to commit, call
            if (tuple_winchance.Item1 >= opponent_commit * 0.9)
                return opponent_bet;
            // if winchance is too low to justify the call, fold
            return 0;
        }

        /// <summary>
        /// First move UTG
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int flop_action_outpos_passive(Dealer Mr_Brown)
        {
            if (opponentmodel.foldChanceFLOP() < 0.4 & AverageWinChance(Mr_Brown) < 0.6)
                return (int)(Math.Round(Mr_Brown.virtualpot * 0.5));
            if (opponentmodel.CurrentAggresiveness() / opponentmodel.AverageAggresiveness() < 0.7)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.40) * 10);
            return 0;
        }

        /// <summary>
        /// Reaction to move UTG
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int flop_action_outpos_active(Dealer Mr_Brown)
        {
            int opponent_bet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            double opponent_commit = opponent_bet / (double)Mr_Brown.pot;

            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceFLOP(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;

            if (fold_or_win_chance > 0.70 || fold_or_win_chance >= opponent_commit * 2)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.60) * 10);

            // if winchance is about equal to commit, call
            if (tuple_winchance.Item1 >= opponent_commit * 0.5)
                return opponent_bet;
            // if winchance is too low to justify the call, fold
            return 0;
        }
        #endregion

        #region turn
        private int turn_action(Dealer Mr_Brown)
        {
            double potpercbet = (double)(Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit) / (double)(2 * Mr_Brown.players[tp].PotCommit);

            // Checked towards me
            if (am_dealer && Mr_Brown.players[op].PotCommit == Mr_Brown.players[tp].PotCommit)
            {
                commits_ranges_current_hand.Add(opponentmodel.addTurnRangeCommitACTIVE(0));
                return turn_action_inpos_passive(Mr_Brown);
            }
            // Betted / raised towards me
            if (am_dealer)
            {
                commits_ranges_current_hand.Add(opponentmodel.addTurnRangeCommitACTIVE(potpercbet));
                return turn_action_inpos_active(Mr_Brown);
            }
            // My move; no action yet
            if (firstMoveOfRound)
                return turn_action_outpos_passive(Mr_Brown);
            // My move; respond to bet / raise
            else
            {
                commits_ranges_current_hand.Add(opponentmodel.addTurnRangeCommitPASSIVE(potpercbet));
                return turn_action_outpos_active(Mr_Brown);
            }
        }

        /// <summary>
        /// Checked towards me
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int turn_action_inpos_passive(Dealer Mr_Brown)
        {
            double fold_or_win_chance = WinOrFoldChanceTURN(Mr_Brown).Item2;
            if (fold_or_win_chance >= 0.60)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.60) * 10);
            if (fold_or_win_chance >= 0.40)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.40) * 10);
            if (opponentmodel.CurrentAggresiveness() / opponentmodel.AverageAggresiveness() < 0.7)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.50) * 10);
            return 0;
        }

        /// <summary>
        /// Raised towards me
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int turn_action_inpos_active(Dealer Mr_Brown)
        {
            int opponent_bet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            double opponent_commit = opponent_bet / (double)Mr_Brown.pot;

            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceTURN(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;

            if (fold_or_win_chance > 0.70 || fold_or_win_chance >= opponent_commit * 2)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.60) * 10);

            // if winchance is about equal to commit, call
            if (tuple_winchance.Item1 >= opponent_commit * 0.5)
                return opponent_bet;
            // if winchance is too low to justify the call, fold
            return 0;
        }

        /// <summary>
        /// First move UTG
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int turn_action_outpos_passive(Dealer Mr_Brown)
        {
            if (opponentmodel.foldChanceTURN(commits_ranges_current_hand) < 0.40 & AverageWinChance(Mr_Brown) > 0.6)
                return (int)(Math.Round(Mr_Brown.virtualpot * 0.5));
            if (opponentmodel.CurrentAggresiveness() / opponentmodel.AverageAggresiveness() < 0.7)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.40) * 10);
            return 0;
        }

        /// <summary>
        /// Reaction to move UTG
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int turn_action_outpos_active(Dealer Mr_Brown)
        {
            int opponent_bet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            double opponent_commit = opponent_bet / (double)Mr_Brown.pot;

            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceTURN(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;

            if (fold_or_win_chance > 0.70 || fold_or_win_chance >= opponent_commit * 2)
                return (int)(Math.Round((Mr_Brown.pot / 10) * 0.60) * 10);

            // if winchance is about equal to commit, call
            if (tuple_winchance.Item1 >= opponent_commit * 0.5)
                return opponent_bet;
            // if winchance is too low to justify the call, fold
            return 0;
        }
        #endregion

        #region river
        private int river_action(Dealer Mr_Brown)
        {
            // Checked towards me
            if (am_dealer && Mr_Brown.players[op].PotCommit == Mr_Brown.players[tp].PotCommit)
                return river_action_inpos_passive(Mr_Brown);
            // Betted / raised towards me
            if (am_dealer)
                return river_action_inpos_active(Mr_Brown);
            // My move; no action yet
            if (firstMoveOfRound)
                return river_action_outpos_passive(Mr_Brown);
            // My move; respond to bet / raise
            else
                return river_action_outpos_active(Mr_Brown);
        }

        /// <summary>
        /// Checked towards me
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int river_action_inpos_passive(Dealer Mr_Brown)
        {
            Tuple<double, double> tuple_winchance = WinOrFoldChanceRIVER(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;
            if (fold_or_win_chance >= 0.60)
                return (int)(Math.Round((Mr_Brown.pot / 10) * 0.60) * 10);
            if (fold_or_win_chance >= 0.40)
                return (int)(Math.Round((Mr_Brown.pot / 10) * 0.40) * 10);
            if (opponentmodel.CurrentAggresiveness() / opponentmodel.AverageAggresiveness() < 0.7)
                return (int)(Math.Round((Mr_Brown.pot / 10) * 0.50) * 10);
            return 0;
        }

        /// <summary>
        /// Raised towards me
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int river_action_inpos_active(Dealer Mr_Brown)
        {
            int opponent_bet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            double opponent_commit = opponent_bet / (double)Mr_Brown.pot;

            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceRIVER(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;

            if (fold_or_win_chance > 0.70 || fold_or_win_chance >= opponent_commit * 2)
                return (int)(Math.Round((Mr_Brown.pot / 10) * 0.60) * 10);

            // if winchance is about equal to commit, call
            if (tuple_winchance.Item1 >= opponent_commit * 0.9)
                return opponent_bet;
            // if winchance is too low to justify the call, fold
            return 0;
        }

        /// <summary>
        /// First move UTG
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int river_action_outpos_passive(Dealer Mr_Brown)
        {
            if (opponentmodel.foldChanceRIVER(commits_ranges_current_hand) < 0.4 & AverageWinChance(Mr_Brown) > 0.6)
                return (int)(Math.Round(Mr_Brown.pot * 0.5));

            return 0;
        }

        /// <summary>
        /// Reaction to move UTG
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private int river_action_outpos_active(Dealer Mr_Brown)
        {
            int opponent_bet = Mr_Brown.players[op].PotCommit - Mr_Brown.players[tp].PotCommit;
            double opponent_commit = opponent_bet / (double)Mr_Brown.pot;

            // if winchance is really high, raise
            Tuple<double, double> tuple_winchance = WinOrFoldChanceRIVER(Mr_Brown);
            double fold_or_win_chance = tuple_winchance.Item2;
            if (fold_or_win_chance > 0.70 || fold_or_win_chance >= opponent_commit * 2)
                return (int)(Math.Round((Mr_Brown.virtualpot / 10) * 0.60) * 10);

            // if winchance is about equal to commit, call
            if (tuple_winchance.Item1 >= opponent_commit * 0.9)
                return opponent_bet;
            // if winchance is too low to justify the call, fold
            return 0;
        }
        #endregion

        // Return <winchance, winorfoldchance>
        private Tuple<double,double> WinOrFoldChanceFLOP(Dealer Mr_Brown)
        {
            int b = betStep <= 3 ? 1 : betStep - 2;
            double avgwc = AverageWinChance(Mr_Brown);
            return new Tuple<double,double>(avgwc, avgwc + ((1 - avgwc) * opponentmodel.foldChanceFLOP())/b);
        }

        private Tuple<double, double> WinOrFoldChanceTURN(Dealer Mr_Brown)
        {
            int b = betStep <= 3 ? 1 : betStep - 2;
            double avgwc = AverageWinChance(Mr_Brown);
            return new Tuple<double, double>(avgwc, avgwc + ((1 - avgwc) * opponentmodel.foldChanceTURN(commits_ranges_current_hand))/b);
        }

        private Tuple<double, double> WinOrFoldChanceRIVER(Dealer Mr_Brown)
        {
            int b = betStep <= 3 ? 1 : betStep - 2;
            double avgwc = AverageWinChance(Mr_Brown);
            return new Tuple<double, double>(avgwc, avgwc + ((1 - avgwc) * opponentmodel.foldChanceRIVER(commits_ranges_current_hand))/b);
        }

        private double AverageWinChance(Dealer Mr_Brown)
        {
            int b = betStep <= 3 ? 1 : betStep - 2;
            int holecardsamples = 200;
            int boardsamples = 500;
            if (phase == GAME_STATE.turn)
                boardsamples = 200;
            if (phase == GAME_STATE.river)
                boardsamples = 20;
            List<double> probabilities = new List<double>();

            // Sample 100 holecards
            Parallel.For(0, holecardsamples, (i) =>
                {
                    double prob = 0.0;

                    List<Card> hc = GenerateHoleCards(Mr_Brown, opponentmodel.currentRange);

                    // Sample 1000 boards
                    for (int j = 0; j < boardsamples; j++)
                    {
                        List<Card> board = GenerateBoard(Mr_Brown, hc);
                        prob += StandardInteraction.IsBetterHand(Mr_Brown.players[tp].HoleCards, CardList.ToCardList(board), CardList.ToCardList(hc)) ? 1 : 0;
                    }
                    probabilities.Add(prob / boardsamples);
                });

            probabilities = probabilities.OrderBy(k => k).ToList();
         //   double fraction = commits_ranges_current_hand[commits_ranges_current_hand.Count - 1] * 1.4 < 1.0 ? commits_ranges_current_hand[commits_ranges_current_hand.Count - 1] * 1.4 : 1.0;
            probabilities = probabilities.GetRange(0, (int)(Math.Round(commits_ranges_current_hand[commits_ranges_current_hand.Count - 1] * probabilities.Count)/b));

            double ret = probabilities.Average();
            return ret;
        }

        private List<Card> GenerateBoard(Dealer Mr_Brown, List<Card> GeneratedHoleCards)
        {
            List<Card> board = (Mr_Brown.c == null || Mr_Brown.c.Count == 0) ? new List<Card>() : Mr_Brown.c.copy();

            board.Add(GeneratedHoleCards[0]);
            board.Add(GeneratedHoleCards[1]);
            board.Add(Mr_Brown.players[tp].HoleCards[0]);
            board.Add(Mr_Brown.players[tp].HoleCards[1]);

            board.Reverse();

            while(board.Count <= 9)
            {
                board.Add(GenerateCard(board));
            }
            return board.GetRange(4, 5);
        }

        /// <summary>
        /// Return hole cards within a specific range
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private List<Card> GenerateHoleCards(Dealer Mr_Brown, int range)
        {
            List<Card> hc = GenerateHoleCards(Mr_Brown);

            if (range == 9)
                return hc;
            for (int i = 0; i < range - 1; i++)
            {
                for (int j = 0; j < ALL_TIERS[i].Count; j++)
                {
                    if (hc[0].rank == ALL_TIERS[i][j].Item1 && hc[1].rank == ALL_TIERS[i][j].Item2 && (hc[0].suit == hc[1].suit) == ALL_TIERS[i][j].Item3)
                        return hc;
                }
            }
            return GenerateHoleCards(Mr_Brown, range);
        }

        /// <summary>
        /// Return any generated hole cards
        /// </summary>
        /// <param name="Mr_Brown"></param>
        /// <returns></returns>
        private List<Card> GenerateHoleCards(Dealer Mr_Brown)
        {
            List<Card> current_list = (Mr_Brown.c == null || Mr_Brown.c.Count == 0) ? Mr_Brown.players[tp].HoleCards.copy() :
                Mr_Brown.players[tp].HoleCards.copy().ConcatReturn(Mr_Brown.c);


            Card c1 = GenerateCard(current_list);
            current_list.Add(c1);
            Card c2 = GenerateCard(current_list);

            List<Card> ret = new List<Card>(2) { c1, c2 };
            sortByRank(CardList.ToCardList(ret));

            return ret;
        }

    }
}
