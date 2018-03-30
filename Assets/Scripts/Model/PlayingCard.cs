using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayingCard : ICard
{

    private Suit suit { get; set; }
    //2-10 for normal cards, Jack through Ace are 11-14.
    private int value { get; set; }

    public PlayingCard(Suit suit, int value)
    {
        this.suit = suit;
        if (value >= 2 && value <= 14)
        {
            this.value = value;
        }
        else
        {
            throw new ArgumentException("Invalid Value.  Must be between 2 and 14");
        } 
    }

    /// <summary>
    /// Create a new Playing Card
    /// </summary>
    /// <param name="suit">Suit of the card</param>
    /// <param name="value">number value of card. Face cards are 11 through 14, minimum value of 2</param>
    public PlayingCard(string suit, int value)
    {
        switch (suit.ToLower())
        {
            case ("hearts"):
                this.suit = Suit.Hearts;
                break;
            case ("diamonds"):
                this.suit = Suit.Diamonds;
                break;
            case ("clubs"):
                this.suit = Suit.Clubs;
                break;
            case ("spades"):
                this.suit = Suit.Spades;
                break;
            default:
                throw new ArgumentException("Invalid Suit Input.  Hearts/Spades/Clubs/Diamonds Only");
        }

        if (value > 1 && value < 15)
        {
            this.value = value;
        }
        else
        {
            throw new ArgumentException("Invalid Value.  Must be between 2 and 14");
        }
    }

    public string getSuitorColor()
    {
        return this.suit.ToString();
    }

    public string GetCardValue()
    {
        if (this.value > 10)
        {
            switch (value)
            {
                case 11:
                    return "Jack";
                case 12:
                    return "Queen";
                case 13:
                    return "King";
                case 14:
                    return "Ace";
                default:
                    throw new Exception("How did this even happen");
            }
        }
        else
        {
            return this.value.ToString();
        }
    }

    public string getFullCard()
    {
        return GetCardValue() + " of " + getSuitorColor();
    }

    public int GetCardNumValue()
    {
        return this.value;
    }

    public int compareSuitTo(ICard card)
    {
        if (this.getSuitorColor() == card.getSuitorColor())
        {
            return 0;
        }
        else return -1;
    }

    public int compareValueTo(ICard card)
    {
        if (this.value > card.GetCardNumValue())
        {
            return 1;
        }
        else if (this.value == card.GetCardNumValue())
        {
            return 0;
        }
        else return -1;
    }
}
