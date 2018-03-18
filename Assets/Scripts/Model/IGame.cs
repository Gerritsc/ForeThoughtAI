﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IGame
{

    /// <summary>
    /// Creates a new Game and initializes the board and player's hand
    /// </summary>
    /// <returns></returns>
    IGame RestartGame();


    /// <summary>
    /// Checks if a set of 5 cards matches the given wintype.  returns true only if the cardset is a valid hand of that wintype
    /// false otherwise
    /// </summary>
    /// <param name="cardset"> set of 5 cards representing a hand</param>
    /// <param name="wintype">the type of win condition to check</param>
    /// <returns></returns>
    bool CheckGameOverClaim(List<ICard> cardset, HANDTYPE wintype);

    IBoard getBoard();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    void SwapCards(int player, int x1, int y1, int x2, int y2);

    /// <summary>
    /// changes turn from one player to the other, drawing cards if necessary
    /// </summary>
    void switchTurn();

    /// <summary>
    /// puts a card from the player's hand into play on the specified tile 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="card"></param>
    void PlayCard(int player, int x, int y, ICard card);

    /// <summary>
    /// Sorts a set of cards by value (Ace is treated as high in this sort)
    /// </summary>
    /// <param name="cards"></param>
    List<ICard> sortbyValue(List<ICard> cards);


    /// <summary>
    /// Removes a card at the given location, if the given player has not already removed a card this game.
    /// Cards in the corners or center can NOT be removed, and this will return an error.
    /// the removed card is not added back into the deck
    /// </summary>
    /// <param name="player">player who's turn it is</param>
    /// <param name="x">x coordinate on the board</param>
    /// <param name="y">y coordinate on the board</param>
    void RemoveCard(int player, int x, int y);

    /// <summary>
    /// Gets this game's deck
    /// </summary>
    /// <returns></returns>
    IDeck getDeck();
}
