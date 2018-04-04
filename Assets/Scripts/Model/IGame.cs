using System;
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

    /// <summary>
    /// Gets the hand.
    /// </summary>
    /// <returns>The hand.</returns>
    List<ICard> getHand ();

    /// <summary>
    /// Returns whether the 2 positions are a valid swappable positions.
    /// In order to be valid, there must be a card at both positions, and they must either be both face up,
    /// or both face down
    /// </summary>
    /// <param name="x1">x pos of first space</param>
    /// <param name="y1">y pos of first space</param>
    /// <param name="x2">x pos of second space</param>
    /// <param name="y2">y pos of second space</param>
    /// <returns>true if the two positions can be swapped</returns>
    bool canSwap(int x1, int y1, int x2, int y2);

    /// <summary>
    /// Returns whether the card at that position can be removed.
    /// In order to be valid, it must not be a starting zone,
    /// and the player must not have already removed a card
    /// </summary>
    /// <param name="player">player who's turn it is</param>
    /// <param name="x">x pos of the space</param>
    /// <param name="y">y pos of the space</param>
    /// <returns>true if the two positions can be swapped</returns>
    bool canRemove(int player, int x, int y);

    /// <summary>
    /// Gets the string representation of the turn player's board.
    /// "none" means the space is empty
    /// "uk" means the space is not known
    /// Known cards are in a "X of Y" format, where X is value and Y is Suit/Color
    /// </summary>
    /// <returns>The turn player's board.</returns>
    string[][] getBoardAsString (IBoard board, bool playerOne);

    /// <summary>
    /// Gets all valid player moves for the given player.
    /// </summary>
    /// <param name="board">current board state</param>
    /// <param name="playerOne">whose turn it is</param>
    /// <returns></returns>
    List<GameMove> getAllPlayerMoves (IBoard board, bool playerOne);

    bool isPlayerOneTurn();

    /// <summary>
    /// Checks if a given column is full of cards, indicating that it can be a claimed for a win
    /// </summary>
    /// <param name="columnnumber">the row to check, 0 indexed</param>
    /// <returns>true if the column is full </returns>
    bool isFullColumn(int columnnumber);

    /// <summary>
    /// Checks if a given column is full of cards, indicating that it 
    /// </summary>
    /// <param name="rownumber"></param>
    /// <returns>true if the row is full</returns>
    bool isFullRow(int rownumber);

    /// <summary>
    /// checks if a given diagonal is full of cards.
    /// </summary>
    /// <param name="diagonal"> True if the diagonal begins in the top left, false if it starts in top right</param>
    /// <returns>true if the diagonal is full</returns>
    bool isFullDiagonal(bool StartLeft);
}
