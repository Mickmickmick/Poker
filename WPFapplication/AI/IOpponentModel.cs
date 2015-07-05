using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villain
{
    interface IOpponentModel
    {
        int currentRange { get; set; }

        double loosenessFactor { get; set; }

        void UpdateMove(GameMechanics.MOVE move, GameMechanics.GAME_STATE gamestate, int betTurn, bool NewRound);

        double CurrentAggresiveness();

        double AverageAggresiveness();

        double addPreflopCommit(int amount);
        double addFlopRangeCommitACTIVE(double percentage);
        double addFlopRangeCommitPASSIVE(double percentage);
        double addTurnRangeCommitACTIVE(double percentage);
        double addTurnRangeCommitPASSIVE(double percentage);
        double addRiverRangeCommitACTIVE(double percentage);
        double addRiverRangeCommitPASSIVE(double percentage);

        double foldChanceFLOP();
        double foldChanceTURN(List<double> current_hand_style);
        double foldChanceRIVER(List<double> current_hand_style);
    }
}
