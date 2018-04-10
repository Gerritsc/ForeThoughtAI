using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// Represents a Deck of Cards either playing cards or not
/// </summary>
public interface IDeck
{
    /// <summary>
    /// Initializes the deck
    /// </summary>
    IDeck RebuildDeck();

    /// <summary>
    /// Randomizes the order of cards in the deck
    /// </summary>
    void ShuffleDeck();

    /// <summary>
    /// Returns the number of cards in the deck
    /// </summary>
    /// <returns></returns>
    int getDeckSize();

    /// <summary>
    /// Removes and returns the top card from the deck. 
    /// </summary>
    /// <returns></returns>
    ICard DrawCard();

}
