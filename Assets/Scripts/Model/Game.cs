using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum HANDTYPE { STRAIGHT, FULLHOUSE, FLUSH, FOURKIND }
/// <summary>
/// Represents a single game of Forethought
/// </summary>
public class Game : IGame
{
    public IDeck deck;
    public IBoard board { get; set; }

	public List<ICard> player, AI;

    //Map keeping track of player's ability to remove from the board, holds true if they can remove
    public Dictionary<int, bool> removalmap;

    /// <summary>
    /// represents whose turn it is, 0 being player1, 1 being player2
    /// </summary>
    int playerturn;

    public Game()
    {
        deck = new PlayingCardDeck();
        deck.ShuffleDeck();
        playerturn = 0;
        removalmap = new Dictionary<int, bool>();
        removalmap.Add(0, true);
        removalmap.Add(1, true);

        var startingcards = new ICard[5];
        for (int i = 0; i < 5; i++)
        {
            startingcards[i] = deck.DrawCard();
        }
        board = new GameBoard(startingcards);

		player = new List<ICard> ();
		AI = new List<ICard> ();
		for (int i = 0; i < 5; i++) 
		{
			player.Add (deck.DrawCard());
			AI.Add (deck.DrawCard());
		}
    }


    public IGame RestartGame()
    {
        return new Game();
    }

    public void switchTurn()
    {
        //Switch to player2's turn
        if (playerturn == 0)
        {
            playerturn = 1;
        }
        //switch to player1's turn
        else
        {
            playerturn = 0;
        }
    }

    public void SwapCards(int player, int x1, int y1, int x2, int y2)
    {
        try
        {
            board.swapCards(x1, y1, x2, y2);
            switchTurn();
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
    }

    public void PlayCard(int player, int x, int y, ICard card)
    {
        try
        {
            this.board.addCard(x, y, card);
			if (player == 0) {
				this.player.Remove(card);
				this.player.Add(deck.DrawCard());
			} else {
				this.AI.Remove(card);
				this.AI.Add(deck.DrawCard());
			}
			
            switchTurn();
        }
        catch(Exception e)
        {

        }
    }

    public IBoard getBoard()
    {
        return this.board;
    }

    public bool CheckGameOverClaim(List<ICard> cardset, HANDTYPE wintype)
    {
        if (cardset.Count != 5)
        {
            return false;
            Console.WriteLine("This is not a valid hand to win with");
        }


        switch (wintype)
        {
            case HANDTYPE.STRAIGHT:
                {
                    sortbyValue(cardset);
                    int[] values = (from i in cardset select i.GetCardNumValue()).ToArray() ;

                    //manual check for A2345 straight
                    if (cardset.Last().GetCardNumValue() == 14 && cardset.First().GetCardNumValue() == 2)
                    {
                        return (values[1] == 2 && values[2] == 3 && values[3] == 4 && values[4] == 5);
                    }


                    break;
                }
            case HANDTYPE.FLUSH:
                {
                    string suit = cardset.First().getSuitorColor();
                    foreach(ICard i in cardset)
                    {
                        if (i.getSuitorColor() != suit)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            case HANDTYPE.FULLHOUSE:
                {
                    sortbyValue(cardset);

                    int[] values = (from i in cardset select i.GetCardNumValue()).ToArray();
                    return (values[0] == values[1] && values[3] == values[4] && (values[2] == values[1] || values[2] == values[3]));

                    break;
                }
            case HANDTYPE.FOURKIND:
                {
                    sortbyValue(cardset);
                    int[] values = (from i in cardset select i.GetCardNumValue()).ToArray();
                    return (values[1] == values[2] && values[2] == values[3]) && (values[0] == values[1] || values[4] == values[1]);
                    break;
                }

        }
        return false;
    }

    public List<ICard> sortbyValue(List<ICard> cards)
    {
        return cards.OrderBy(o => o.GetCardNumValue()).ToList();
    }

    public void RemoveCard(int player, int x, int y)
    {
        var maxlen = board.GetBoardDimensions() - 1;
        //Check if the given coordinates are the board corners
        if((x == 0  && y == 0)  || 
           (x == 0 && y == maxlen)  ||
           (x == maxlen && y == 0)  ||
           (x == maxlen & y == maxlen)  ||
           (x == (maxlen/2) && y == (maxlen/2)))
        {
            throw new ArgumentException("You cannot remove a corner card");
        }

        try
        {
            board.removeCard(x, y);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public IDeck getDeck()
    {
        return this.deck;
    }

    public List<ICard> getHand()
    {
        return this.player;
    }

    public bool canSwap(int x1, int y1, int x2, int y2)
    {
        return board.canSwap(x1, y1, x2, y2);
    }

	public string[][] getBoardAsString (IBoard board, bool playerOne){
		return null;
	}

	public List<GameMove> getAllPlayerMoves (IBoard board, bool playerOne){
		return null;
	}

	public bool isPlayerOneTurn(){
		return (playerturn == 0);
	}

    public bool isFullColumn(int columnnumber)
    {
        return board.isFullColumn(columnnumber);
    }

    public bool isFullRow(int rownumber)
    {
        return board.isFullRow(rownumber);
    }

    public bool isFullDiagonal(bool StartLeft)
    {
        return board.isFullDiagonal(StartLeft);
    }
}
