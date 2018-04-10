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

    public void addCard(int x, int y, ICard card)
    {
        if (x < GRIDSIZE && y < GRIDSIZE)
        {
            BoardSpace space = board[x, y];
            if (space.getCard() == null)
            {
                space.setCard(card);
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
            Console.WriteLine(String.Format("first: {0}     second: {1}", firstSpace.getCard(), secondSpace.getCard()));

            var tempcard = secondSpace.getCard();
            tempcard = firstSpace.swapCard(tempcard);
            tempcard = secondSpace.swapCard(tempcard);
            Console.WriteLine(String.Format("first: {0}     second: {1}", firstSpace.getCard(), secondSpace.getCard()));
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
        }
        catch(Exception e) {
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
        for (int y = 0;  y < GRIDSIZE; y ++)
        {
            if (board[columnnum,y].getCard() != null)
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
            Yval = GRIDSIZE -1;
        }
        for (int x = 0; x < GRIDSIZE; x ++)
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
}
