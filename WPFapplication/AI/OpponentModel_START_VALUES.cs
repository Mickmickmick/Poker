using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villain
{
    class OpponentModel_START_VALUES
    {
        public static List<int> preflopCommits = new List<int>(18) { 400, 200, 100, 60, 60, 60, 60, 40, 40, 40, 40, 20, 20, 20, 0, 0, 0, 0 };
        public static List<double> flopCommitsACTIVE = new List<double>(15) { 2.00, 1.00, 1.00, 0.60, 0.60, 0.5, 0.50, 0.50, 0.50, 0.40, 0.40, 0.20, 0.20 };
        public static List<double> flopCommitsPASSIVE = new List<double>(15) { 2.00, 1.00, 1.00, 0.60, 0.60, 0.5, 0.50, 0.50, 0.50, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        public static List<double> turnCommitsACTIVE = new List<double>(15) { 2.00, 1.00, 1.00, 0.60, 0.60, 0.5, 0.50, 0.50, 0.50, 0.40, 0.40, 0.20, 0.20 };
        public static List<double> turnCommitsPASSIVE = new List<double>(15) { 2.00, 1.00, 1.00, 0.60, 0.60, 0.5, 0.50, 50, 50, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        public static List<double> riverCommitsACTIVE = new List<double>(15) { 2.00, 1.00, 1.00, 0.60, 0.60, 0.5, 0.50, 0.50, 0.50, 0.40, 0.40, 0.20, 0.20 };
        public static List<double> riverCommitsPASSIVE = new List<double>(15) { 2.00, 1.00, 1.00, 0.60, 0.60, 0.5, 0.50, 0.50, 0.50, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
    }
}
