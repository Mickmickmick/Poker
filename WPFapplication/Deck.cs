using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMechanics
{
    public class Deck
    {
        public CardList DeckOfCards;

        // Constructor, init deck
        public Deck()
        {
            DeckOfCards = new CardList();

            foreach (CARD_SUIT s in Enum.GetValues(typeof(CARD_SUIT)))
            {
                foreach (CARD_RANK r in Enum.GetValues(typeof(CARD_RANK)))
                {
                    DeckOfCards.Add(new Card(s, r));
                }
            }
        }

        public int DeckSize()
        {
            return DeckOfCards.Count;
        }

        Random rnd = new Random();
        // Take random card
        public Card DrawCard()
        {            
            int index = rnd.Next(DeckSize());
            Card drawnCard = DeckOfCards[index];
            DeckOfCards.RemoveAt(index);
            return drawnCard;
        }
    }

}
