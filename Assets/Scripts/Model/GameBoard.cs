using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class GameBoard : IBoard
{
	static readonly int GRIDSIZE = 5;

	/// <summary>
	/// 2d Array of boardspaces, first value indicates Y value, second value indicates X value
	/// </summary>
	BoardSpace[,] board;

	public bool[][] player1KnownCards, player2KnownCards;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="cards"> an Array of 5 cards, in order from top left to bottom right to add to the board</param>
	public GameBoard(ICard[] cards)
	{
		board = new BoardSpace[GRIDSIZE, GRIDSIZE];
		bool faceup;
		for (int y = 0; y < GRIDSIZE; y++)
		{
			for (int x = 0; x < GRIDSIZE; x++)
			{
				faceup = (x + y) % 2 == 0;
				board[x, y] = new BoardSpace(x, y, faceup);
			}
		}

		int centerpoint = GRIDSIZE / 2;

		board[0, 0].setCard(cards[0]);
		board[0, GRIDSIZE - 1].setCard(cards[1]);
		board[centerpoint, centerpoint].setCard(cards[2]);
		board[GRIDSIZE - 1, 0].setCard(cards[3]);
		board[GRIDSIZE - 1, GRIDSIZE - 1].setCard(cards[4]);


		int max = GRIDSIZE;
		player1KnownCards = new bool[max][];
		player2KnownCards = new bool[max][];
		for (int x = 0; x < max; x++)
		{
			player1KnownCards[x] = new bool[max];
			player2KnownCards[x] = new bool[max];
			for (int y = 0; y < max; y++)
			{
				if (isStartingZone(x, y))
				{
					player1KnownCards[x][y] = true;
					player2KnownCards[x][y] = true;
				}
				else
				{
					player1KnownCards[x][y] = false;
					player2KnownCards[x][y] = false;
				}
			}
		}
	}

	private GameBoard(GameBoard other)
	{
		this.board = new BoardSpace[GRIDSIZE, GRIDSIZE];
		bool faceup;
		for (int y = 0; y < GRIDSIZE; y++)
		{
			for (int x = 0; x < GRIDSIZE; x++)
			{
				faceup = (x + y) % 2 == 0;
				board[x, y] = new BoardSpace(x, y, faceup);
				if (other.board[x, y].getCard() != null)
				{
					board[x, y].setCard(other.board[x, y].getCard().CopyCard());
				}
			}
		}
		int max = GRIDSIZE;
		player1KnownCards = new bool[max][];
		player2KnownCards = new bool[max][];

		for (int x = 0; x < max; x++)
		{
			player1KnownCards[x] = new bool[max];
			player2KnownCards[x] = new bool[max];
			for (int y = 0; y < max; y++)
			{
				player1KnownCards[x][y] = other.player1KnownCards[x][y];
				player2KnownCards[x][y] = other.player2KnownCards[x][y];
			}
		}
	}

	public void PrintBoard()
	{
		for (int y = 0; y < GRIDSIZE; y++)
		{
			for (int x = 0; x < GRIDSIZE; x++)
			{
				var card = board[x, y].getCard();
				if (card != null && board[x, y].isFaceUp())
				{
					Console.Write(String.Format("{0, 20}", board[x, y].getCard().getFullCard()));
				}
				else if (card != null)
				{
					Console.Write(String.Format("{0, 20}", "Face Down Card"));
				}
				else
				{
					Console.Write(String.Format("{0, 20}", "Empty"));
				}
			}
			Console.WriteLine();
		}
	}

	public void addCard(int player, int x, int y, ICard card)
	{
		if (x < GRIDSIZE && y < GRIDSIZE)
		{
			BoardSpace space = board[x, y];
			if (space.getCard() == null)
			{
				space.setCard(card);
				if (player == 0)
				{
					this.player1KnownCards[x][y] = true;
				}
				else
				{
					this.player2KnownCards[x][y] = true;
				}
			}
			else
			{
				throw new ArgumentException("That space already has a card on it");
			}
		}
	}

	public IBoard InitBoard(List<ICard> cards)
	{
		return new GameBoard(cards.ToArray());
	}

	public void swapCards(int x1, int y1, int x2, int y2)
	{
		var firstSpace = board[x1, y1];
		var secondSpace = board[x2, y2];

		if (firstSpace.getCard() == null)
		{
			throw new ArgumentException("First space given has not card");
		}
		if (secondSpace.getCard() == null)
		{
			throw new ArgumentException("First space given has not card");
		}

		if (firstSpace.isFaceUp() == secondSpace.isFaceUp())
		{
			//Console.WriteLine(String.Format("first: {0}     second: {1}", firstSpace.getCard(), secondSpace.getCard()));

			var tempcard = secondSpace.getCard();
			tempcard = firstSpace.swapCard(tempcard);
			tempcard = secondSpace.swapCard(tempcard);

			bool temp;
			temp = player1KnownCards[x1][y1];
			player1KnownCards[x1][y1] = player1KnownCards[x2][y2];
			player1KnownCards[x2][y2] = temp;

			temp = player2KnownCards[x1][y1];
			player2KnownCards[x1][y1] = player2KnownCards[x2][y2];
			player2KnownCards[x2][y2] = temp;

			//Console.WriteLine(String.Format("first: {0}     second: {1}", firstSpace.getCard(), secondSpace.getCard()));
		}
		else
		{
			throw new ArgumentException("Both spaces must either be face up or face down");
		}
	}

	public ICard GetCardAtSpace(int x, int y)
	{
		var space = board[x, y];
		return space.getCard();
	}

	public int GetBoardDimensions()
	{
		return GRIDSIZE;
	}

	public void removeCard(int x, int y)
	{
		var space = board[x, y];

		try
		{
			space.removeCard();

			player1KnownCards[x][y] = false;
			player2KnownCards[x][y] = false;


		}
		catch (Exception e)
		{
			throw e;
		}
	}

	public bool canSwap(int x1, int y1, int x2, int y2)
	{
		var firstSpace = board[x1, y1];
		var secondSpace = board[x2, y2];

		if (firstSpace.getCard() == null)
		{
			return false;
		}
		if (secondSpace.getCard() == null)
		{
			return false;
		}
		if (firstSpace.isFaceUp() != secondSpace.isFaceUp())
		{
			return false;
		}
		return true;
	}

	public bool isFullColumn(int columnnum)
	{
		int numcards = 0;
		for (int y = 0; y < GRIDSIZE; y++)
		{
			if (board[columnnum, y].getCard() != null)
			{
				numcards++;
			}
		}

		return numcards == GRIDSIZE;
	}

	public bool isFullRow(int rownum)
	{
		int numcards = 0;
		for (int x = 0; x < GRIDSIZE; x++)
		{
			if (board[x, rownum].getCard() != null)
			{
				numcards++;
			}
		}

		return numcards == GRIDSIZE;
	}

	public bool isFullDiagonal(bool StartLeft)
	{
		int numcards = 0;
		int Yval;
		if (StartLeft)
		{
			Yval = 0;
		}
		else
		{
			Yval = GRIDSIZE - 1;
		}
		for (int x = 0; x < GRIDSIZE; x++)
		{
			if (board[x, Yval].getCard() != null)
			{
				numcards++;
			}

			if (StartLeft)
			{
				Yval++;
			}
			else
			{
				Yval--;
			}
		}
		return numcards == GRIDSIZE;
	}

	public IBoard CopyBoard()
	{
		return new GameBoard(this);
	}

	private bool isStartingZone(int x, int y)
	{
		int max = GRIDSIZE - 1;
		return ((x == 0 || x == max) && (y == 0 || y == max)) || (x == (max / 2) && y == (max / 2));
	}

	public string getBoardAsString(bool playerOne)
	{
		int max = this.GetBoardDimensions();
		string boardString = "";

		for (int x = 0; x < max; x++)
		{
			for (int y = 0; y < max; y++)
			{
				ICard card = this.GetCardAtSpace(x, y);
				if (card == null)
				{
					boardString += "none";
				}
				else if ((x + y) % 2 == 1)
				{
					if (playerOne && player1KnownCards[x][y])
					{
						boardString += card.getFullCard();
					}
					else if (!playerOne && player2KnownCards[x][y])
					{
						boardString += card.getFullCard();
					}
					else
					{
						boardString += "uk";
					}
				}
				else
				{
					boardString += card.getFullCard();
				}
			}
		}
		return boardString;
	}

	public void Peek(int player, int x, int y)
	{
		if (player == 0)
		{
			player1KnownCards[x][y] = true;
		}
		else
		{
			player2KnownCards[x][y] = true;
		}
	}
}
