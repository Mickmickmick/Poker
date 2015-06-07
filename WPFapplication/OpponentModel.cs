using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villain
{
    public class OpponentModel
    {
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
            flopCommitsPASSIVE = flopCommitsACTIVE.OrderByDescending(k => k).ToList();

            int first = flopCommitsPASSIVE.FindIndex(k => k == percentage);
            int last = flopCommitsPASSIVE.FindLastIndex(k => k == percentage);

            return ((first + last) / 2.0) / flopCommitsPASSIVE.Count;
        }

        public double foldChanceFLOP()
        {
            return (double) flopCommitsPASSIVE.FindAll(k => k == 0).Count / (double) flopCommitsPASSIVE.Count;
        }
    
    }
}
