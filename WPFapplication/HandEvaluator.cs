using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMechanics
{
    public class HandEvaluator
    {
        public COLLECTION_RANKS rank;
        public CardList composition;   

        // Constructor
        public HandEvaluator(CardList all_cards)
        {
            flush flush_class = new flush(all_cards);
            straight straight_class = new straight(all_cards);
            sets sets_class = new sets(all_cards);

            // Default
            composition = sets_class.best_hand;
            rank = sets_class.hand_rank;

            // Check straight flush
            if (flush_class.in_hand && straight_class.in_hand)
            {
                flush straight_flush_class = new flush(straight_class.best_hand);
                if (straight_flush_class.in_hand)
                {
                    rank = COLLECTION_RANKS.straight_flush;
                    composition = straight_flush_class.best_hand.GetRange(0, 4);
                    return;
                }
            }
            // If full house / quads
            if (sets_class.hand_rank > COLLECTION_RANKS.flush)
                return;
            // Check flush
            if (flush_class.in_hand)
            {
                composition = flush_class.best_hand.GetRange(0, 4);
                rank = COLLECTION_RANKS.flush;
                return;
            }
            // Check straight
            if (straight_class.in_hand)
            {
                composition = straight_class.best_hand.GetRange(0, 4);
                rank = COLLECTION_RANKS.straight;
                return;
            }
        }       

   
       
    }

    public class StandardInteraction
    {
        public static bool IsBetterHand(CardList hole, CardList comm_cards, CardList vil)
        {
            CardList hero_cards = hole.copy().Concat(comm_cards);
            CardList villain_cards = vil.copy().Concat(comm_cards);

            HandEvaluator hero_strength = new HandEvaluator(hero_cards);
            HandEvaluator villain_strength = new HandEvaluator(villain_cards);

            if ((int)hero_strength.rank > (int)villain_strength.rank)
                return true;
            if ((int)hero_strength.rank < (int)villain_strength.rank)
                return false;

            for (int i = 0; i < hero_strength.composition.Count; i++)
            {
                if ((int)hero_strength.composition[i].rank > (int)villain_strength.composition[i].rank)
                    return true;
                if ((int)hero_strength.composition[i].rank < (int)villain_strength.composition[i].rank)
                    return false;
            }
            // Return null if split 
            return true;
        }

        public static Player WhoWins(Player hero, Player villain, CommunityCards comm_cards)
        {
            if (hero.PotCommit != villain.PotCommit)
            {
                Player winner = hero.PotCommit > villain.PotCommit ? hero : villain;
                return winner;
            }
            
            CardList hero_cards = hero.HoleCards.Concat(comm_cards);
            CardList villain_cards = villain.HoleCards.Concat(comm_cards);

            HandEvaluator hero_strength = new HandEvaluator(hero_cards);
            HandEvaluator villain_strength = new HandEvaluator(villain_cards);

            if ((int)hero_strength.rank > (int)villain_strength.rank)
                return hero;
            if ((int)hero_strength.rank < (int)villain_strength.rank)
                return villain;

            for (int i = 0; i < hero_strength.composition.Count; i++)
            {
                if ((int)hero_strength.composition[i].rank > (int)villain_strength.composition[i].rank)
                    return hero;
                if ((int)hero_strength.composition[i].rank < (int)villain_strength.composition[i].rank)
                    return villain;
            }
            // Return null if split 
            return null;
        }
    }

    class sets
    {        
        public COLLECTION_RANKS hand_rank;
        public CardList best_hand = new CardList();

        public COLLECTION_RANKS set_picker(List<int> set_list)
        {
            // Get rank
            if (set_list[3] > 0)
                return COLLECTION_RANKS.quads;
            else if ((set_list[2] > 0 && set_list[1] > 0) || (set_list[2] == 2))
                return COLLECTION_RANKS.full_house;
            else if (set_list[2] > 0)
                return COLLECTION_RANKS.three_of_a_kind;
            else if (set_list[1] > 1)
                return COLLECTION_RANKS.two_pair;
            else if (set_list[1] > 0)
                return COLLECTION_RANKS.one_pair;
            else
                return COLLECTION_RANKS.high_card;
        }

        public sets(CardList cards)
        {
            List<int> set_list = new List<int>(4){0,0,0,0};
            
            // Group the cards by rank and put their ranks in tuples with an occurence count
            var groups = cards.Select(s => s.rank).GroupBy(g => g).OrderBy(o => o.Key).Reverse();
            List<Tuple<CARD_RANK, int>> ranks_list = new List<Tuple<CARD_RANK,int>>();
            foreach (var g in groups)
            {
                ranks_list.Add(new Tuple<CARD_RANK, int>(g.Key, g.Count()));
                set_list[g.Count() - 1]++;
            }
            hand_rank = set_picker(set_list);

            // Get hand
            List<CARD_RANK> temp_best_hand = new List<CARD_RANK>();

            int i = 4;
            while(i > 0)
            {                
                for (int j = 0; i > 0 && j < ranks_list.Count(); j++ )
                {
                    i = 5 - temp_best_hand.Count() < i ? 5 - temp_best_hand.Count() : i;
                    Tuple<CARD_RANK, int> tup = ranks_list[j];
                    if (tup.Item2 >= i)
                    {
                        temp_best_hand.AddRange(Enumerable.Repeat(tup.Item1, i));
                        ranks_list.Remove(tup);
                        j--;
                        continue;
                    }                   
                }
                i--;                
            }

            CardList cards_copy = cards.copy();
            // From ranks-list to cards-list
            foreach (CARD_RANK r in temp_best_hand)
            {  
                Card c = cards_copy.Find(f => f.rank == r);
                best_hand.Add(c);
                cards_copy.Remove(c);
            }   
        }
        
    }

    // Checks if a straight is within the range
    class straight
    {
        public bool in_hand = false;
        public CardList best_hand = new CardList();

        // Constrcutor
        public straight(CardList cards)
        {
            // First extract the ranks from the hand and remove duplicates
            List<CARD_RANK> ranks_list = cards.Select(s => s.rank).OrderBy(o => o).Reverse().Distinct().ToList();
            if (ranks_list[0] == CARD_RANK.ace)
                ranks_list.Add(CARD_RANK.ace);
            while (ranks_list.Count() >= 4)
            {
                int sequence = descending_count(ranks_list);
                if (sequence < 5)
                    ranks_list.RemoveRange(0, sequence);
                else
                {
                    in_hand = true;
                    best_hand = CardList.ToCardList(cards.Where(i => ranks_list.GetRange(0,5).Contains(i.rank)).ToList());
                    
                    break;
                }
            }
        }

        // Gets a list of descending cards and a flag that says if an ace is there or not
        // It checks how many elements it takes until the next one is not a proper
        // successor. It returns this number
        int descending_count(List<CARD_RANK> hand_ranks)
        {
            CARD_RANK current_rank = hand_ranks[0];
            int i;
            for (i = 1; i < hand_ranks.Count(); i++)
            {
                if ((int)current_rank - (int)hand_ranks[i] == 1 || (int)current_rank - (int)hand_ranks[i] == -12)
                    current_rank = hand_ranks[i];
                else
                    break;
            }
            return i;
        }
    }

    // Checks if a flush is within the range
    class flush
    {
        public bool in_hand = false;
        public CardList best_hand = new CardList();

        // Constructor
        public flush(CardList cards)
        {
            // First extract the suits from the hand and group them
            // Sort list so the most occurent suit is at index=0
            // Check if count of index=0 is >=5           
            var grouped = cards.GroupBy(s => s.suit).Select(group => new { Word = group.Key, Count = group.Count() }).OrderBy( s => s.Count).Reverse();
            if (grouped.ElementAt(0).Count >= 5)
            {
                CARD_SUIT color = grouped.ElementAt(0).Word;
                in_hand = true;
                best_hand = CardList.ToCardList(cards.FindAll(i => i.suit == color).OrderBy(o => o.rank).ToList());                    
            }            
        }
    }


    public enum COLLECTION_RANKS
    {
        high_card,
        one_pair,
        two_pair,
        three_of_a_kind,
        straight,
        flush,
        full_house,
        quads,
        straight_flush
    }

}
