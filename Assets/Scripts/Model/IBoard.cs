using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBoard
{
    /// <summary>
    /// Creates and returns a new playing board, plotting the given cards as the first face up cards
    /// </summary>
    /// <param name="cards"> 5 Cards to add to the center of the board</param>
    /// <returns>An Initialized Board</returns>
    IBoard InitBoard(List<ICard> cards);

    /// <summary>
    /// Gets a card at this space, if it's empty, returns null;
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    ICard GetCardAtSpace(int x, int y);

    /// <summary>
    /// Adds given card to the board, at the specified board location with X,Y coordinates.
    /// \n Will fail if the position is outside the board, or if there is a card already on the selected space.
    /// </summary>
    /// <param name="x">X position of the card to place</param>
    /// <param name="y">Y position of the card to place</param>
    /// <param name="card">Card to add</param>
    void addCard(int x, int y, ICard card);

    /// <summary>
    /// Switches the cards at the two given coordinates on the board.  They MUST be of the same face up property, however.
    /// </summary>
    /// <param name="x1">x coordinate of first space</param>
    /// <param name="y1">y coordinate of first space</param>
    /// <param name="x2">x coordinate of second space</param>
    /// <param name="y2">y coordinate of second space</param>
    void swapCards(int x1, int y1, int x2, int y2);

    /// <summary>
    /// Removes the card at this space, throws an error if no card is at that space.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void removeCard(int x, int y);

    void PrintBoard();
    /// <summary>
    /// Returns the dimensions of this game board.  because these boards are square, only one value is needed
    /// </summary>
    /// <returns></returns>
    int GetBoardDimensions();


    /// <summary>
    /// Returns whether or not these 2 positions are swappable
    /// </summary>
    /// <param name="x1">x pos of first space</param>
    /// <param name="y1">y pos of first space</param>
    /// <param name="x2">x pos of second space</param>
    /// <param name="y2">y pos of second space</param>
    /// <returns></returns>
    bool canSwap(int x1, int y1, int x2, int y2);

    /// <summary>
    /// Checks if a given column is full of cards
    /// </summary>
    /// <param name="columnnum"></param>
    /// <returns></returns>
    bool isFullColumn(int columnnum);

    /// <summary>
    /// Checks if a given row is full of cards
    /// </summary>
    /// <param name="rownum"></param>
    /// <returns></returns>
    bool isFullRow(int rownum);

    bool isFullDiagonal(bool StartLeft);
}