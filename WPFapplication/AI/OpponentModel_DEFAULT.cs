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

        List<int> preflopCommits = new List<int>(18) { 400, 200, 100, 60, 60, 60, 60, 40, 40, 40, 40, 20, 20, 20, 0, 0, 0, 0 };

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


        /**************
         * FLOP
         * ************/
        List<double> flopCommitsACTIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };
        public double addFlopRangeCommitACTIVE(double percentage)
        {
            flopCommitsACTIVE.Add(percentage);
            flopCommitsACTIVE = flopCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = flopCommitsACTIVE.FindIndex(k => k == percentage);
            int last = flopCommitsACTIVE.FindLastIndex(k => k == percentage);

            flopCommitsACTIVE.Remove(percentage);

            return ((first + last) / 2.0) / flopCommitsACTIVE.Count;
        }

        List<double> flopCommitsPASSIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };
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
        List<double> turnCommitsACTIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };
        public double addTurnRangeCommitACTIVE(double percentage)
        {
            turnCommitsACTIVE.Add(percentage);
            turnCommitsACTIVE = turnCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = turnCommitsACTIVE.FindIndex(k => k == percentage);
            int last = turnCommitsACTIVE.FindLastIndex(k => k == percentage);

            turnCommitsACTIVE.Remove(percentage);

            return ((first + last) / 2.0) / turnCommitsACTIVE.Count;
        }

        List<double> turnCommitsPASSIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };
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
        List<double> riverCommitsACTIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };
        public double addRiverRangeCommitACTIVE(double percentage)
        {
            riverCommitsACTIVE.Add(percentage);
            riverCommitsACTIVE = riverCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = riverCommitsACTIVE.FindIndex(k => k == percentage);
            int last = riverCommitsACTIVE.FindLastIndex(k => k == percentage);

            riverCommitsACTIVE.Remove(percentage);

            return ((first + last) / 2.0) / riverCommitsACTIVE.Count;
        }

        List<double> riverCommitsPASSIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };
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

        public double foldChanceTURN()
        {
            return loosenessFactor * ((double)turnCommitsPASSIVE.FindAll(k => k == 0).Count / (double)turnCommitsPASSIVE.Count);
        }

        public double foldChanceRIVER()
        {
            return loosenessFactor * ((double)riverCommitsPASSIVE.FindAll(k => k == 0).Count / (double)riverCommitsPASSIVE.Count);
        }
    }
}
