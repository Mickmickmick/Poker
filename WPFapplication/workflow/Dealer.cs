using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameMechanics
{
    public class Dealer
    {
        int pot = 0;
        Deck d;
        public CommunityCards c;

        // hero is 0, villain is 1
        public List<Player> players;
        
        //GUI.Table t;

        public Dealer(List<Player> player_list/*, GUI.Table table*/)
        {
            d = new Deck();
           // t = table;                        
            c = new CommunityCards();
            players = player_list;
        }

        public void DealHoleCards(GUI.Table table)
        {           
            foreach (Player p in players)
            {
                p.HoleCards = new HoleCards(d);
            }
            table.ChangeHoleCards(players[0].HoleCards[0].GetImageSrc(), players[0].HoleCards[1].GetImageSrc());                       
        }

        public void AcceptBet(Player p, int amount)
        {           
            p.Pay(amount);
            pot += amount;
        }

        public void ShowDown()
        {
            WrapUp(StandardInteraction.WhoWins(players[0], players[1], c));
        }



        /*
        public void CardHandler(Image im, BitmapImage src)
        {
            im.BeginInit();
            im.Source = src;
            im.EndInit();
        }

        
        public void HoleCardWpf()
        {
            if (players[0].HoleCards.Count > 0)
            {
                CardHandler((Image)t.FindName("Hero1"), players[0].HoleCards[0].GetImageSrc());
                CardHandler((Image)t.FindName("Hero2"), players[0].HoleCards[1].GetImageSrc());
                CardHandler((Image)t.FindName("Villain1"), players[1].HoleCards[0].GetImageSrc());
                CardHandler((Image)t.FindName("Villain2"), players[1].HoleCards[1].GetImageSrc());
            }
            else
            {
                CardHandler((Image)t.FindName("Hero1"), null);
                CardHandler((Image)t.FindName("Hero2"), null);
                CardHandler((Image)t.FindName("Villain1"), null);
                CardHandler((Image)t.FindName("Villain2"), null);
            }
        } */

        public void WrapUp(Player winner)
        {
            d = new Deck();
            foreach (Player p in players)
            {
                p.HoleCards.Clear();
                p.PotCommit = 0;
            }
            if(winner != null)
                winner.Won(pot);
            pot = 0;
            c.Clear();
            //HoleCardWpf();
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
    }

    public enum GAME_STATE
    {
        preflop,
        flop,
        turn,
        river
    }
}
