using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace GameMechanics
{
    public class Dealer
    {
        // hero is 0, villain is 1
        public List<Player> players;
        
        public int small_blind = 10;

        public int pot
        {
            get { return players[0].PotCommit + players[1].PotCommit; }
        }
        public CommunityCards c;
        public GAME_STATE game_state;
        public Player whosturn;
        public Player dealer_button;
        
        Deck d;

        public Dealer(List<Player> player_list)
        {
            game_state = GAME_STATE.prepare;

            d = new Deck();               
            c = new CommunityCards();
            players = player_list;

            // Who starts as dealer?
            Random rnd = new Random();
            dealer_button = players[rnd.Next(0, 2)];
        }

        public void DealHoleCards()
        {           
            foreach (Player p in players)
                p.HoleCards = new HoleCards(d);
        }

        public void AcceptBet(Player p, int amount)
        {           
            p.Pay(amount);
            //pot = p.PotCommit + OtherPlayer(p).PotCommit;
        }

        public void ShowDown()
        {
            WrapUp(WhoWins());
        }

        public void ChangeTurn()
        {
            whosturn = OtherPlayer(whosturn);
        }

        public void ChangeDealerButton()
        {
            dealer_button = OtherPlayer(dealer_button);
        }

        public Player OtherPlayer(Player p)
        {
            if (p.PlayerID == PLAYER.Computer)
                return players[0];
            else
                return players[1];
        }

        public bool Continue()
        {
            if (players[0].PotCommit != players[1].PotCommit)
            {
                return false;
            }
            return true;
        }

        public Player WhoWins()
        {
            return StandardInteraction.WhoWins(players[0], players[1], c);
        }

        public void WrapUp(Player winner)
        {
            if (winner != null)
                winner.Won(pot);
            else
            {
                players[0].Won(players[0].PotCommit);
                players[1].Won(players[1].PotCommit);
            }

            d = new Deck();
            foreach (Player p in players)
            {
                p.HoleCards.Clear();
                p.PotCommit = 0;
            }
           
            c.Clear();
            ChangeDealerButton();
        }

        public CommunityCards Flop()
        {
            c.Flop(d);
            return c;
        }

        public CommunityCards Turn()
        {
            c.Turn(d);
            return c;
        }

        public CommunityCards River()
        {
            c.River(d);
            return c;
        }

        public void AcceptBlinds()
        {
            // If one of the players don't have small blind worth of chips
            if (dealer_button.StackSize < small_blind || OtherPlayer(dealer_button).StackSize < small_blind)
            {
                int small_stack = dealer_button.StackSize < OtherPlayer(dealer_button).StackSize ? dealer_button.StackSize : OtherPlayer(dealer_button).StackSize;
                AcceptBet(dealer_button, small_stack);
                AcceptBet(OtherPlayer(dealer_button), small_stack);
            }
            // If big blind has >small blind and <big blind worth of chips
            else if (OtherPlayer(dealer_button).StackSize < small_blind * 2)
            {
                AcceptBet(dealer_button, small_blind);
                AcceptBet(OtherPlayer(dealer_button), OtherPlayer(dealer_button).StackSize);
            }
            // Every other case
            else
            {
                AcceptBet(dealer_button, small_blind);
                AcceptBet(OtherPlayer(dealer_button), small_blind * 2);
            }
        }
    }

    public enum GAME_STATE
    {
        prepare,
        preflop,
        flop,
        turn,
        river,
        wrapup
    }

    /// <summary>
    /// Enum for hand history
    /// </summary>
    public enum MOVE
    {
        fold,
        check,
        call,
        bet_raise
    }
}
