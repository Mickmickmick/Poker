using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameMechanics
{
    public class Card
    {
        public CARD_SUIT suit;
        public CARD_RANK rank;

        public Card(CARD_SUIT s, CARD_RANK r)
        {
            suit = s;
            rank = r;
        }

        public string rankToString()
        {
            if (rank == CARD_RANK.jack || rank == CARD_RANK.queen || rank == CARD_RANK.king || rank == CARD_RANK.ace)
                return rank.ToString();
            switch (rank)
            {
                case CARD_RANK.two:
                    return "2";
                case CARD_RANK.three:
                    return "3";
                case CARD_RANK.four:
                    return "4";
                case CARD_RANK.five:
                    return "5";
                case CARD_RANK.six:
                    return "6";
                case CARD_RANK.seven:
                    return "7";
                case CARD_RANK.eight:
                    return "8";
                case CARD_RANK.nine:
                    return "9";
                case CARD_RANK.ten:
                    return "10";
            }
            return "Not a valid card given";
        }

        public BitmapImage GetImageSrc()
        {
            string suffix = (rank == CARD_RANK.jack || rank == CARD_RANK.queen || rank == CARD_RANK.king) ? "2" : "";
            string uriString = "C:\\Users\\Mick\\Documents\\Visual Studio 2010\\Projects\\Poker\\GameMechanics\\Images\\Playing_Cards\\" + rankToString() + "_of_" + suit.ToString() + suffix +".png";
            Uri uri = new Uri(uriString, UriKind.Absolute);
            return new BitmapImage(uri); 
        }

        public static BitmapImage BackSrc()
        {
            string uriString = "C:\\Users\\Mick\\Documents\\Visual Studio 2010\\Projects\\Poker\\GameMechanics\\Images\\Playing_Cards\\Back.png";
            Uri uri = new Uri(uriString, UriKind.Absolute);
            return new BitmapImage(uri); 
        }
    }

    public enum CARD_SUIT
    {
        hearts,
        diamonds,
        spades,
        clubs
    };

    public enum CARD_RANK
    {
        two     = 2,
        three   = 3,
        four    = 4,
        five    = 5,
        six     = 6,
        seven   = 7,
        eight   = 8,
        nine    = 9,
        ten     = 10,
        jack    = 11,
        queen   = 12,
        king    = 13,
        ace     = 14
    };
}
