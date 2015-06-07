using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMechanics
{
    public class Player
    {
        public int StackSize;
        public HoleCards HoleCards;
        public int PotCommit;
        public PLAYER PlayerID;

        public Player(PLAYER id, int begin_stack)
        {
            PotCommit = 0;
            StackSize = begin_stack;
            PlayerID = id;
        }

        public void Pay(int amount)
        {
            if (amount > StackSize)
                throw new InvalidOperationException();
            PotCommit += amount;
            StackSize -= amount;
        }

        public void Won(int amount)
        {
            StackSize += amount;
        }
    }

    public enum PLAYER
    {
        User,
        Computer
    }
}
