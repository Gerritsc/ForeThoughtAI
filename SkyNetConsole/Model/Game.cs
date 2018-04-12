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

    public List<ICard> player1hand, player2hand;


    public bool[][] player1KnownCards, player2KnownCards;

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

        player1hand = new List<ICard>();
        player2hand = new List<ICard>();
        for (int i = 0; i < 5; i++)
        {
            player1hand.Add(deck.DrawCard());
            player2hand.Add(deck.DrawCard());
        }

        int max = board.GetBoardDimensions();
        player1KnownCards = new bool[max][];
        player2KnownCards = new bool[max][];
        for (int x = 0; x < max; x++) {
            player1KnownCards[x] = new bool[max];
            player2KnownCards[x] = new bool[max];
            for (int y = 0; y < max; y++) {
                if (isStartingZone(x, y)) 
                {
                    player1KnownCards[x][y] = true;
                    player2KnownCards[x][y] = true;
                }
                else {
                    player1KnownCards[x][y] = false;
                    player2KnownCards[x][y] = false;
                }
            }
        }
    }

    public IGame RestartGame()
    {
        return new Game();
    }

    public void switchTurn ()
    {
        //Switch to player2's turn
        if (playerturn == 0) {
            playerturn = 1;
        }
        //switch to player1's turn
        else {
            playerturn = 0;
        }
    }

    public void SwapCards(int player, int x1, int y1, int x2, int y2)
    {
        try
        {
            board.swapCards(x1, y1, x2, y2);
            
            bool temp;
            temp = player1KnownCards[x1][y1];
            player1KnownCards[x1][y1] = player1KnownCards[x2][y2];
            player1KnownCards[x2][y2] = temp;

            temp = player2KnownCards[x1][y1];
            player2KnownCards[x1][y1] = player2KnownCards[x2][y2];
            player2KnownCards[x2][y2] = temp;

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
            if (player == 0)
            {
                this.player1hand.Remove(card);
                this.player1hand.Add(deck.DrawCard());
                this.player1KnownCards[x][y] = true;
            }
            else
            {
                this.player2hand.Remove(card);
                this.player2hand.Add(deck.DrawCard());
                this.player2KnownCards[x][y] = true;
            }
            
            switchTurn();
        }
        catch (Exception e)
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
                    int[] values = (from i in cardset select i.GetCardNumValue()).ToArray();

                    //manual check for A2345 straight
                    if (cardset.Last().GetCardNumValue() == 14 && cardset.First().GetCardNumValue() == 2)
                    {
                        return (values[1] == 2 && values[2] == 3 && values[3] == 4 && values[4] == 5);
                    }
					
                    Array.Sort (values);
                    for (int i = 1; i < values.Length; i++) 
                    {
                        if (values [i] - 1 != values [i - 1]) 
                        {
                            return false;
                        }
                    }
					
                    return true;
                }
            case HANDTYPE.FLUSH:
                {
                    string suit = cardset.First().getSuitorColor();
                    foreach (ICard i in cardset)
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
                    int[] values = (from i in cardset select i.GetCardNumValue()).ToArray();
                    Array.Sort(values);
                    return (values[0] == values[1] && values[3] == values[4] && (values[2] == values[1] || values[2] == values[3]));
                }
            case HANDTYPE.FOURKIND:
                {
                    sortbyValue(cardset);
                    int[] values = (from i in cardset select i.GetCardNumValue()).ToArray();
                    Array.Sort(values);
                    return (values[1] == values[2] && values[2] == values[3]) && (values[0] == values[1] || values[4] == values[1]);
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
        if (isStartingZone(x, y))
        {
            Console.WriteLine("You cannot remove a card in a starting zone");
        }

        if (removalmap[player])
        {
            Console.WriteLine("You can only remove one card per game");
        }

        try
        {
            board.removeCard(x, y);
            removalmap[player] = false;
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
        return this.player1hand;
    }

    public bool canSwap(int x1, int y1, int x2, int y2)
    {
        return board.canSwap(x1, y1, x2, y2);
    }

    public List<GameMove> getAllPlayerMoves (IBoard board, bool playerOne)
    {
        List<GameMove> retList = new List<GameMove> ();
        List<ICard> hand;
        if (playerOne) {
            hand = this.player1hand;
        } else {
            hand = this.player2hand;
        }

        int max = board.GetBoardDimensions ();
        for (int i = 0; i < max * max; i++) {
            int x = i % max;
            int y = i / max;
            if (board.GetCardAtSpace (x, y) == null) {
                //Play
                foreach (var card in hand) {
                    retList.Add (new GameMove (x, y, card));
                }
                continue;
            }

            //Peek
            if ((x + y) % 2 == 1) {
                retList.Add(new GameMove(x, y, true));
            }

            //Remove
            if (canRemove(playerturn, x, y)) {
                retList.Add (new GameMove (x, y, false));
            }

            //Swap
            for (int j = i + 1; j < max * max; j++) {
                int a = j % max;
                int b = j / max;

                if (((x + y) % 2 == (a + b) % 2) && board.GetCardAtSpace (a, b) != null) {
                    retList.Add(new GameMove(x, y, a, b));
                }
            }
        }

        return retList;
    }

    public bool canRemove(int player, int x, int y)
    {
        int max = board.GetBoardDimensions () - 1;
        return (removalmap[player] && !isStartingZone(x, y));
    }

    private bool isStartingZone(int x, int y) 
    {
        int max = board.GetBoardDimensions () - 1;
        return ((x == 0 || x == max) && (y == 0 || y == max)) || (x == (max / 2) && y == (max / 2));
    }


    public string[][] getBoardAsString (IBoard board, bool playerOne)
    {
        int max = board.GetBoardDimensions ();
        string[][] boardString = new string[max][];

        for (int x = 0; x < max; x++)
        {
            boardString [x] = new string[max];
            for (int y = 0; y < max; y++) 
            {
                ICard card = board.GetCardAtSpace (x, y);
                if (card == null) 
                {
                    boardString [x] [y] = "none";
                } 
                else if ((x + y) % 2 == 1)
                {
                    if (playerOne && player1KnownCards[x][y]) {
                        boardString[x][y] = card.getFullCard ();
                    }
                    else if (!playerOne && player2KnownCards[x][y]) {
                        boardString[x][y] = card.getFullCard ();
                    }
                    else {
                        boardString [x] [y] = "uk";
                    }
                } 
                else 
                {
                    boardString [x] [y] = card.getFullCard ();
                }
            }
        }
        return boardString;
    }

    public bool isPlayerOneTurn()
    {
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

    public void addPeekToKnown(int x, int y) 
    {
        if (isPlayerOneTurn()) {
            player1KnownCards[x][y] = true;
        }
        else {
            player2KnownCards[x][y] = true;
        }

    } 
}
