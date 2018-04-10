using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public enum Suit { Hearts = 0, Clubs, Diamonds, Spades}
/// <summary>
/// Represents a Standard deck of playing cards
/// </summary>
public class PlayingCardDeck : IDeck
{
    public int decksize { get; set; }
    public List<ICard> cards;

    /// <summary>
    /// Makes all 52 cards.  is NOT shuffled
    /// </summary>
    public PlayingCardDeck()
    {
        cards = new List<ICard>();

        for (int value = 2; value < 15; value++)
        {
            for (int suit = 0; suit < 4; suit++)
            {
                switch (suit)
                {
                    case 0:
                        cards.Add(new PlayingCard(Suit.Clubs, value));
                        break;
                    case 1:
                        cards.Add(new PlayingCard(Suit.Spades, value));
                        break;
                    case 2:
                        cards.Add(new PlayingCard(Suit.Hearts, value));
                        break;
                    case 3:
                        cards.Add(new PlayingCard(Suit.Diamonds, value));
                        break;
                }
            }
        }

        this.decksize = cards.Count;
    }

    public ICard DrawCard()
    {
        if (decksize > 0)
        {
            ICard topcard = cards.First();
            cards.Remove(cards.First());
            decksize--;

            return topcard;
        }
        else
        {
            throw new Exception("Deck has no cards in it");
        }
    }

    public int getDeckSize()
    {
        return decksize;
    }

    public IDeck RebuildDeck()
    {
        return new PlayingCardDeck();
    }


    public void ShuffleDeck()
    {

        Random r = new Random();
        for (var i = cards.Count - 1; i > 0; i--)
        {
            var temp = cards[i];
            var index = r.Next(0, i + 1);
            cards[i] = cards[index];
            cards[index] = temp;
        }
    }

    public List<ICard> getDeck()
    {
        return cards;
    }
}