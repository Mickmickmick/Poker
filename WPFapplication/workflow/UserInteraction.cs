using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameMechanics;

namespace Cards
{
    static class UserInteraction
    {

        public static int CheckRaise(Dealer Mr_Brown, GUI.Table table)
        {
            // GUI stuff
            return 0;
        }
    
        public static int FoldCallRaise(Dealer Mr_Brown, GUI.Table table)
        {

            return Mr_Brown.players[1].PotCommit - Mr_Brown.players[0].PotCommit;
        }


    }
}
