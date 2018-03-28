using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum MoveType { ADD = 0, SWAP, REMOVE}

public class GameMove {
	public int x1;
	public int y1;
	public int x2;
	public int y2;
	public ICard card;
	public MoveType type;

	public GameMove(GameMove other){
		this.x1 = other.x1;
		this.y1 = other.y1;
		this.x2 = other.x2;
		this.y2 = other.y2;
		this.card = other.card;
		this.type = other.type;
	}

	public GameMove(int x1, int y1){
		this.type = MoveType.REMOVE;
		this.x1 = x1;
		this.y1 = y1;
	}

	public GameMove(int x1, int y1, int x2, int y2){
		this.type = MoveType.SWAP;
		this.x1 = x1;
		this.y1 = y1;
		this.x2 = x2;
		this.y2 = y2;
	}

	public GameMove(int x1, int y1, ICard card){
		this.type = MoveType.ADD;
		this.x1 = x1;
		this.y1 = y1;
		this.card = card;
	}

	public override bool Equals(object obj){
		if (obj != null && (obj is GameMove) && ((GameMove)obj).type == this.type) {
			GameMove other = (GameMove)obj;
			switch (this.type) {
			case MoveType.ADD:
				return (this.x1 == other.x1 && this.y1 == other.y1) && this.card.Equals(other.card);
			case MoveType.SWAP:
				return this.x1 == other.x1 && this.y1 == other.y1 && this.x2 == other.x2 && this.y2 == other.y2;
			case MoveType.REMOVE:
				return this.x1 == other.x1 && this.y1 == other.y1;
			default:
				return false;
			}
		}
		return false;
	}
}

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
}