using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villain
{
    public class OpponentModel_DEFAULT : IOpponentModel
    {
        private double _loosenessFactor = 1.2;
        public double loosenessFactor { get { return _loosenessFactor; } set { _loosenessFactor = value; } }

        List<int> preflopCommits = OpponentModel_START_VALUES.preflopCommits;

        public double addPreflopCommit(int amount)
        {
            int first = preflopCommits.FindIndex(k => k >= amount);
            int last = preflopCommits.FindLastIndex(k => k <= amount);

            return ((first + last) / 2.0) / preflopCommits.Count;
        }

        public void UpdateMove(GameMechanics.MOVE move, GameMechanics.GAME_STATE gamestate, int betTurn, bool NewRound)
        {
            return;
        }

        public double CurrentAggresiveness()
        {
            return 0.4;
        }

        public double AverageAggresiveness()
        {
            return 0.4;
        }

        public int currentRange { get; set; }

        /**************
         * FLOP
         * ************/
        List<double> flopCommitsACTIVE = OpponentModel_START_VALUES.flopCommitsACTIVE;
        public double addFlopRangeCommitACTIVE(double percentage)
        {
            flopCommitsACTIVE.Add(percentage);
            flopCommitsACTIVE = flopCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = flopCommitsACTIVE.FindIndex(k => k == percentage);
            int last = flopCommitsACTIVE.FindLastIndex(k => k == percentage);

            flopCommitsACTIVE.Remove(percentage);

            return ((first + last) / 2.0) / flopCommitsACTIVE.Count;
        }

        List<double> flopCommitsPASSIVE = OpponentModel_START_VALUES.flopCommitsPASSIVE;
        public double addFlopRangeCommitPASSIVE(double percentage)
        {
            flopCommitsPASSIVE.Add(percentage);
            flopCommitsPASSIVE = flopCommitsPASSIVE.OrderByDescending(k => k).ToList();

            int first = flopCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = flopCommitsPASSIVE.FindLastIndex(k => k == percentage);

            flopCommitsPASSIVE.Remove(percentage);

            return ((first + last) / 2.0) / flopCommitsPASSIVE.Count;
        }

        /**************
         * TURN
         * ************/
        List<double> turnCommitsACTIVE = OpponentModel_START_VALUES.turnCommitsACTIVE;
        public double addTurnRangeCommitACTIVE(double percentage)
        {
            turnCommitsACTIVE.Add(percentage);
            turnCommitsACTIVE = turnCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = turnCommitsACTIVE.FindIndex(k => k == percentage);
            int last = turnCommitsACTIVE.FindLastIndex(k => k == percentage);

            turnCommitsACTIVE.Remove(percentage);

            return ((first + last) / 2.0) / turnCommitsACTIVE.Count;
        }

        List<double> turnCommitsPASSIVE = OpponentModel_START_VALUES.turnCommitsPASSIVE;
        public double addTurnRangeCommitPASSIVE(double percentage)
        {
            turnCommitsPASSIVE.Add(percentage);
            turnCommitsPASSIVE = turnCommitsPASSIVE.OrderByDescending(k => k).ToList();

            int first = turnCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = turnCommitsPASSIVE.FindLastIndex(k => k == percentage);

            turnCommitsPASSIVE.Remove(percentage);

            return ((first + last) / 2.0) / turnCommitsPASSIVE.Count;
        }

        /**************
         * RIVER
         * ************/
        List<double> riverCommitsACTIVE = OpponentModel_START_VALUES.riverCommitsACTIVE;
        public double addRiverRangeCommitACTIVE(double percentage)
        {
            riverCommitsACTIVE.Add(percentage);
            riverCommitsACTIVE = riverCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = riverCommitsACTIVE.FindIndex(k => k == percentage);
            int last = riverCommitsACTIVE.FindLastIndex(k => k == percentage);

            riverCommitsACTIVE.Remove(percentage);

            return ((first + last) / 2.0) / riverCommitsACTIVE.Count;
        }

        List<double> riverCommitsPASSIVE = OpponentModel_START_VALUES.riverCommitsPASSIVE;
        public double addRiverRangeCommitPASSIVE(double percentage)
        {
            riverCommitsPASSIVE.Add(percentage);
            riverCommitsPASSIVE = riverCommitsPASSIVE.OrderByDescending(k => k).ToList();

            int first = riverCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = riverCommitsPASSIVE.FindLastIndex(k => k == percentage);

            riverCommitsPASSIVE.Remove(percentage);

            return ((first + last) / 2.0) / riverCommitsPASSIVE.Count;
        }

        /**************
         * Fold chances
         * ************/
        public double foldChanceFLOP()
        {
            return loosenessFactor * ((double)flopCommitsPASSIVE.FindAll(k => k == 0).Count / (double)flopCommitsPASSIVE.Count);
        }

        public double foldChanceTURN(List<double> current_hand_style)
        {
            return loosenessFactor *
                ((((double)turnCommitsPASSIVE.FindAll(k => k == 0).Count / (double)turnCommitsPASSIVE.Count)
                + 0.5 * current_hand_style[current_hand_style.Count - 1]
                )) / 2;
        }

        public double foldChanceRIVER(List<double> current_hand_style)
        {
            return loosenessFactor *
                ((((double)riverCommitsPASSIVE.FindAll(k => k == 0).Count / (double)riverCommitsPASSIVE.Count)
                + 0.5 * current_hand_style[current_hand_style.Count - 1]
                + 0.5 * current_hand_style[current_hand_style.Count - 2]
                )) / 3;
        }
    }
}
