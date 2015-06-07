using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMechanics
{
    class StandardHands
    {
        public IList<HandEvaluator> standard_hands;
        
        public StandardHands()
        {
            // c2, c3, hT, dJ, sK
            HandEvaluator high_card = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.clubs, CARD_RANK.two), 
                new Card(CARD_SUIT.clubs, CARD_RANK.three),
                new Card(CARD_SUIT.hearts, CARD_RANK.ten),
                new Card(CARD_SUIT.diamonds, CARD_RANK.jack),
                new Card(CARD_SUIT.spades, CARD_RANK.king)        
            });

            // d3, c3, hT, cJ, cQ
            HandEvaluator one_pair = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.diamonds, CARD_RANK.three), 
                new Card(CARD_SUIT.clubs, CARD_RANK.three),
                new Card(CARD_SUIT.hearts, CARD_RANK.ten),
                new Card(CARD_SUIT.clubs, CARD_RANK.jack),
                new Card(CARD_SUIT.clubs, CARD_RANK.queen)        
            });


            // d3, c3, hT, cT, cJ
            HandEvaluator two_pair = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.diamonds, CARD_RANK.three), 
                new Card(CARD_SUIT.clubs, CARD_RANK.three),
                new Card(CARD_SUIT.hearts, CARD_RANK.ten),
                new Card(CARD_SUIT.clubs, CARD_RANK.ten),
                new Card(CARD_SUIT.clubs, CARD_RANK.jack)                   
            });

            // c7, dT, hT, cT, cJ
            HandEvaluator three_of_a_kind = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.clubs, CARD_RANK.seven),
                new Card(CARD_SUIT.diamonds, CARD_RANK.ten), 
                new Card(CARD_SUIT.hearts, CARD_RANK.ten),
                new Card(CARD_SUIT.clubs, CARD_RANK.ten),
                new Card(CARD_SUIT.clubs, CARD_RANK.jack)                   
            });

            // d4, s5, s6, c7, c3
            HandEvaluator straight = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.diamonds, CARD_RANK.four),
                new Card(CARD_SUIT.spades, CARD_RANK.five), 
                new Card(CARD_SUIT.spades, CARD_RANK.six),
                new Card(CARD_SUIT.clubs, CARD_RANK.seven),
                new Card(CARD_SUIT.clubs, CARD_RANK.three)                   
            });

            // d2, d8, dT, d3, dJ
            HandEvaluator flush = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.diamonds, CARD_RANK.two),
                new Card(CARD_SUIT.diamonds, CARD_RANK.eight), 
                new Card(CARD_SUIT.diamonds, CARD_RANK.ten),
                new Card(CARD_SUIT.diamonds, CARD_RANK.three),
                new Card(CARD_SUIT.diamonds, CARD_RANK.jack)                   
            });

            // d6, c6, h6, hA, sA
            HandEvaluator full_house = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.diamonds, CARD_RANK.six), 
                new Card(CARD_SUIT.clubs, CARD_RANK.six),
                new Card(CARD_SUIT.hearts, CARD_RANK.six),
                new Card(CARD_SUIT.hearts, CARD_RANK.ace),
                new Card(CARD_SUIT.spades, CARD_RANK.ace),
                new Card(CARD_SUIT.diamonds, CARD_RANK.ace),
            });

            // cK, dK, hK, sK, s2
            HandEvaluator quads = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.clubs, CARD_RANK.king),
                new Card(CARD_SUIT.diamonds, CARD_RANK.king), 
                new Card(CARD_SUIT.hearts, CARD_RANK.king),
                new Card(CARD_SUIT.spades, CARD_RANK.king),
                new Card(CARD_SUIT.spades, CARD_RANK.two)                   
            });

            // hK, hT, hJ, h9, hQ
            HandEvaluator straight_flush = new HandEvaluator(new CardList() { 
                new Card(CARD_SUIT.hearts, CARD_RANK.king),
                new Card(CARD_SUIT.hearts, CARD_RANK.ten), 
                new Card(CARD_SUIT.hearts, CARD_RANK.jack),
                new Card(CARD_SUIT.hearts, CARD_RANK.nine),
                new Card(CARD_SUIT.hearts, CARD_RANK.queen)                   
            });

            standard_hands = new List<HandEvaluator>(9) { high_card, one_pair, two_pair, three_of_a_kind, straight, flush, full_house, quads, straight_flush };
        }
    }
}
