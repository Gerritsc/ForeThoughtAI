using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// Represents a single card in a deck
/// </summary>
public interface ICard
{
    /// <summary>
    /// Returns the suit or color of a card.  For playing cards, suit should be the only important aspect.
    /// </summary>
    /// <returns></returns>
    string getSuitorColor();

    /// <summary>
    /// get the numerical value of a card.  For Non-Numerical Cards, return the Name of the Card (Ie Jack or Skip)
    /// </summary>
    /// <returns></returns>
    string GetCardValue();

    /// <summary>
    /// gets the full card value in a "X of Y" format, where X is value and Y is Suit/Color
    /// </summary>
    /// <returns></returns>
    string getFullCard();

    /// <summary>
    /// Returns the numerical value of the card.  Face cards are 11-14
    /// </summary>
    /// <returns></returns>
    int GetCardNumValue();

    /// <summary>
    /// returns whether this has the same suit as the given card. 
    /// </summary>
    /// <param name="card"></param>
    /// <returns>0 if equal, -1 if not</returns>
    int compareSuitTo(ICard card);

    /// <summary>
    /// returns whether this card has a lower value than the 
    /// </summary>
    /// <param name="card"></param>
    /// <returns>-1 if this card is less than, 0 if equal, 1 if this card is greater</returns>
    int compareValueTo(ICard card);
}
