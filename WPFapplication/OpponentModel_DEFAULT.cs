using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villain
{
    public class OpponentModel_DEFAULT
    {
        List<int> preflopCommits = new List<int>(18) { 400, 200, 100, 60, 60, 60, 60, 40, 40, 40, 40, 20, 20, 20, 0, 0, 0, 0 };

        public double addPreflopCommit(int amount)
        {
            int first = preflopCommits.FindIndex(k => k >= amount);
            int last = preflopCommits.FindLastIndex(k => k <= amount);

            return ((first + last) / 2.0) / preflopCommits.Count;
        }
    }
}
