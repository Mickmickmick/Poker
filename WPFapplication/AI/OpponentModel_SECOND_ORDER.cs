using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villain
{
    public class OpponentModel_SECOND_ORDER : IOpponentModel
    {
        Random rand = new Random();

        private double randNormDistr()
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         1.0 + 0.2 * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        /***************************************
         * General
         * *************************************/
        private double _loosenessFactor = 1.2;
        public double loosenessFactor {
            get { return _loosenessFactor * randNormDistr(); } 
            set { _loosenessFactor = value; } 
        }

        string startTime;

        // Current hand progress
        private GameMechanics.GAME_STATE current_game_state;

        List<List<List<GameMechanics.MOVE>>> moveHistory = new List<List<List<GameMechanics.MOVE>>>();
        List<List<GameMechanics.MOVE>> currentHandHistory = new List<List<GameMechanics.MOVE>>();
        List<GameMechanics.MOVE> currentStreetHistory = new List<GameMechanics.MOVE>();

        // Constructor
        public OpponentModel_SECOND_ORDER()
        {
            DateTime time = DateTime.Now;             // Use current time.
            string format = "yyyy-MM-dd-hh-mm-ss";   // Use this format.
            startTime = time.ToString(format);
            current_game_state = GameMechanics.GAME_STATE.preflop;
        }

        private void writeToFile(List<List<GameMechanics.MOVE>> list)
        {
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\Users\\Mick\\Documents\\Poker\\" + startTime + ".txt", true);
            file.WriteLine("HAND\n\n");
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                    file.WriteLine(list[i][j].ToString());
                file.WriteLine();
            }
            file.WriteLine("\n\n");
            file.Close();
        }

        // Players (Villains) move update in DB
        public void UpdateMove(GameMechanics.MOVE move, GameMechanics.GAME_STATE gamestate, int betTurn, bool NewRound)
        {
            if (NewRound)
            {
                if (currentHandHistory != null && currentHandHistory.Count != 0)
                {
                    writeToFile(currentHandHistory);
                    moveHistory.Add(currentHandHistory);
                    currentHandHistory.Clear();
                }
            }
            
            if (gamestate != current_game_state)
            {
                if (currentStreetHistory != null && currentStreetHistory.Count != 0)
                {
                    currentHandHistory.Add(currentStreetHistory);
                    currentStreetHistory.Clear();
                }
                current_game_state = gamestate;
            }
            currentStreetHistory.Add(move);
        }

        /// <summary>
        /// Get aggressiveness this hand
        /// </summary>
        /// <returns></returns>
        public double CurrentAggresiveness()
        {
            
            if (currentHandHistory.Count == 0)
                return 0.4;

            double handAggr = 0;
            //Loop through a particular hand
            for (int i = 0; i < currentHandHistory.Count; i++)
            {
                double streetAggr = 0;

                // Loop through a particular street
                for (int j = 0; j < currentStreetHistory.Count; j++)
                {
                    if (currentStreetHistory[j] == GameMechanics.MOVE.call)
                        streetAggr += 0.5;
                    if (currentStreetHistory[j] == GameMechanics.MOVE.bet_raise)
                        streetAggr += 1.0;
                }
                handAggr += streetAggr / currentStreetHistory.Count;
            }
            return handAggr / currentHandHistory.Count;
        }

        /// <summary>
        /// This function returns a double between 0 and 1 that determines the "aggresiveness" of the opponent
        /// A low number means the opponent mainly folds/checks, whereas a high number indicates a lot of betting/raising
        /// </summary>
        /// <returns></returns>
        public double AverageAggresiveness()
        {
            if (moveHistory == null || moveHistory.Count < 6)
                return 0.4;

            double totalAggr = 0;
            
            //Loop through hands
            for (int i = 0; i < moveHistory.Count; i++)
            {
                double handAggr = 0;

                //Loop through a particular hand
                for (int j = 0; j < moveHistory[i].Count; j++)
                {
                    double streetAggr = 0;

                    // Loop through a particular street
                    for (int k = 0; k < moveHistory[i][j].Count; k++)
                    {
                        if (moveHistory[i][j][k] == GameMechanics.MOVE.call)
                            streetAggr += 0.5;
                        if (moveHistory[i][j][k] == GameMechanics.MOVE.bet_raise)
                            streetAggr += 1;
                    }
                    handAggr += streetAggr / moveHistory[i][j].Count;
                }
                totalAggr += handAggr / moveHistory[i].Count;
            }
            return totalAggr / moveHistory.Count;
        }

        /***************************************
         * PREFLOP
         * *************************************/

        // default values
        List<int> preflopCommits = new List<int>(18) { 400, 200, 100, 60, 60, 60, 60, 40, 40, 40, 40, 20, 20, 20, 0, 0, 0, 0 };

        public int currentRange;

        public double addPreflopCommit(int amount)
        {
            preflopCommits.Add(amount);
            preflopCommits = preflopCommits.OrderByDescending(k => k).ToList();

            int first = preflopCommits.FindIndex(k => k == amount);
            int last = preflopCommits.FindLastIndex(k => k == amount);

            return ((first + last) / 2.0) / preflopCommits.Count;
        }

      /***************************************
      * FLOP
      * *************************************/
        List<double> flopCommitsACTIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage">Percentage relative to pot!</param>
        /// <returns></returns>
        public double addFlopRangeCommitACTIVE(double percentage)
        {
            flopCommitsACTIVE.Add(percentage);
            flopCommitsACTIVE = flopCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = flopCommitsACTIVE.FindIndex(k => k == percentage);
            int last = flopCommitsACTIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / flopCommitsACTIVE.Count;
        }

        List<double> flopCommitsPASSIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage">Percentage relative to pot!</param>
        /// <returns></returns>
        public double addFlopRangeCommitPASSIVE(double percentage)
        {
            flopCommitsPASSIVE.Add(percentage);
            flopCommitsPASSIVE = flopCommitsPASSIVE.OrderByDescending(k => k).ToList();

            int first = flopCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = flopCommitsPASSIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / flopCommitsPASSIVE.Count;
        }

        public double foldChanceFLOP()
        {
            return loosenessFactor * ((double)flopCommitsPASSIVE.FindAll(k => k == 0).Count / (double)flopCommitsPASSIVE.Count);
        }

     /***************************************
     * TURN
     * *************************************/
        List<double> turnCommitsACTIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage">Percentage relative to pot!</param>
        /// <returns></returns>
        public double addTurnRangeCommitACTIVE(double percentage)
        {
            turnCommitsACTIVE.Add(percentage);
            turnCommitsACTIVE = turnCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = turnCommitsACTIVE.FindIndex(k => k == percentage);
            int last = turnCommitsACTIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / turnCommitsACTIVE.Count;
        }

        List<double> turnCommitsPASSIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage">Percentage relative to pot!</param>
        /// <returns></returns>
        public double addTurnRangeCommitPASSIVE(double percentage)
        {
            turnCommitsPASSIVE.Add(percentage);
            turnCommitsPASSIVE = turnCommitsPASSIVE.OrderByDescending(k => k).ToList();

            int first = turnCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = turnCommitsPASSIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / turnCommitsPASSIVE.Count;
        }

        public double foldChanceTURN()
        {
            return loosenessFactor * ((double)turnCommitsPASSIVE.FindAll(k => k == 0).Count / (double)turnCommitsPASSIVE.Count);
        }

        /***************************************
        * RIVER
        * *************************************/
        List<double> riverCommitsACTIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage">Percentage relative to pot!</param>
        /// <returns></returns>
        public double addRiverRangeCommitACTIVE(double percentage)
        {
            riverCommitsACTIVE.Add(percentage);
            riverCommitsACTIVE = turnCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = riverCommitsACTIVE.FindIndex(k => k == percentage);
            int last = riverCommitsACTIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / riverCommitsACTIVE.Count;
        }

        List<double> riverCommitsPASSIVE = new List<double>(15) { 200, 100, 100, 60, 60, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage">Percentage relative to pot!</param>
        /// <returns></returns>
        public double addRiverRangeCommitPASSIVE(double percentage)
        {
            riverCommitsPASSIVE.Add(percentage);
            riverCommitsPASSIVE = riverCommitsPASSIVE.OrderByDescending(k => k).ToList();

            int first = riverCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = riverCommitsPASSIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / riverCommitsPASSIVE.Count;
        }

        public double foldChanceRIVER()
        {
            return loosenessFactor * ((double)riverCommitsPASSIVE.FindAll(k => k == 0).Count / (double)riverCommitsPASSIVE.Count);
        }
    
    }
}
