using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GameMechanics
{
    public class CardList : List<Card>
    {
        public static CardList ToCardList(List<Card> list)
        {
            CardList newCL = new CardList();
            foreach(Card c in list)
                newCL.Add(c);
            return newCL;
        }
       
        public void AddRandom(Deck d)
        {
            this.Add(d.DrawCard());
        }

        public CardList GetRange(int from, int to)
        {
            CardList newCL = new CardList();
            for (int i = from; i <= to; i++)
            {
                newCL.Add(this[i]);
            }
            return newCL;
        }

        public CardList Concat(CardList append)
        {
            CardList current = this;
            foreach (Card c in append)
                current.Add(c);
            return current;
        }

        public CardList copy()
        {
            CardList newCL = new CardList();
            foreach(Card c in this)
                newCL.Add(c);
            return newCL;
        }
    }


    public class HoleCards : CardList
    {        
        public HoleCards(Deck d)
        {
            AddRandom(d);
            AddRandom(d);
        }
    }

    public class CommunityCards : CardList
    {
        public void Flop(Deck d)
        {
            AddRandom(d);
            AddRandom(d);
            AddRandom(d);
        }

        public void Turn(Deck d)
        {
            AddRandom(d);
        }

        public void River(Deck d)
        {
            AddRandom(d);
        }
    }
}
