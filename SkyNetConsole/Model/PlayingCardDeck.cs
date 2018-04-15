using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
public enum Suit { Hearts = 0, Clubs, Diamonds, Spades}
/// <summary>
/// Represents a Standard deck of playing cards
/// </summary>
[System.Serializable]
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

    private PlayingCardDeck(PlayingCardDeck deck)
    {
        this.cards = new List<ICard>();

        foreach (var card in deck.cards)
        {
            this.cards.Add(card.CopyCard());
        }

        this.decksize = this.cards.Count;

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

    public IDeck CopyDeck()
    {
        return new PlayingCardDeck(this);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("cards", this.cards, typeof(List<ICard>));
        info.AddValue("decksize", this.decksize, typeof(int));
    }

    public PlayingCardDeck(SerializationInfo info, StreamingContext context){
        this.cards = (List<ICard>) info.GetValue("cards", typeof(List<ICard>));
        this.decksize = (int) info.GetValue("decksize", typeof(int));
    }
}