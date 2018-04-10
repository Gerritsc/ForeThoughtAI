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

    //Map keeping track of player's ability to remove from the board, holds true if they can remove
    public Dictionary<bool, bool> removalmap;

    /// <summary>
    /// represents whose turn it is, true being player1, false being player2
    /// </summary>
    bool playerOneTurn;

    public Game()
    {
        deck = new PlayingCardDeck();
        deck.ShuffleDeck();
        playerOneTurn = true;
        removalmap = new Dictionary<bool, bool>();
        removalmap.Add(true, false);
        removalmap.Add(false, false);

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
    }


    public IGame RestartGame()
    {
        return new Game();
    }
    //Jack note: this gave me migraines so I changed it to a bool
    public void switchTurn()
    {
        playerOneTurn = !playerOneTurn;
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
            if (player == 0)
            {
                this.player1hand.Remove(card);
                this.player1hand.Add(deck.DrawCard());
            }
            else
            {
                this.player2hand.Remove(card);
                this.player2hand.Add(deck.DrawCard());
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

    public void RemoveCard(bool playerOne, int x, int y)
    {
        
        var maxlen = board.GetBoardDimensions() - 1;
        //Check if the given coordinates are the board corners
        if(((x == 0 || x == maxlen) && (y == 0 || y == maxlen)) ||
           (x == (maxlen/2) && y == (maxlen/2)))
        {
            throw new ArgumentException("You cannot remove a card in a starting zone");
        }

        if (removalmap[playerOne])
        {
            throw new ArgumentException ("You can only remove one card per game");
        }

        try
        {
            board.removeCard(x, y);
            removalmap[playerOne] = true;
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

    //Jack Note: modified to support multiple players
    public List<ICard> getHand()
    {
        return playerOneTurn ? this.player1hand : this.player2hand;
    }

    public bool canSwap(int x1, int y1, int x2, int y2)
    {
        return board.canSwap(x1, y1, x2, y2);
    }

    public List<GameMove> getAllPlayerMoves(IBoard board, bool playerOne)
    {
        List<GameMove> retList = new List<GameMove>();
        //Jack note: replace if/else assignment with ternary if because ternary is bae
        List<ICard> hand = (playerOne) ? this.player1hand : this.player2hand;

        //Jack note: should get board dims once for optimization
        for (int x = 0; x < board.GetBoardDimensions(); x++)
        {
            for (int y = 0; y < board.GetBoardDimensions(); y++)
            {
                //Play Card Moves

                //if empty space
                if (board.GetCardAtSpace(x, y) == null)
                {
                    foreach (var card in hand)
                    {
                        retList.Add(new GameMove(x, y, card));
                    }
                    bool canremove;
                    removalmap.TryGetValue(playerOneTurn, out canremove);
                    if (canremove)
                    {
                        //Remove
                        var maxlen = board.GetBoardDimensions() - 1;
                        //Check if the given coordinates are the board corners
                        //Jack's note: shorter -> ((x % maxlen != 0 || y % maxlen != 0) && (x != maxlen / 2 || y != maxlen/2));
                        bool corner = ((x == 0 && y == 0) ||
                           (x == 0 && y == maxlen) ||
                           (x == maxlen && y == 0) ||
                           (x == maxlen & y == maxlen) ||
                           (x == (maxlen / 2) && y == (maxlen / 2)));

                        if (!corner)
                        {
                            retList.Add(new GameMove(x, y, false));
                        }
                    }

                    //Swap Cards
                }
            }
        }

        //Swap Card Moves

        return retList;
    }
    public bool canRemove(bool playerOne, int x, int y)
    {
        int max = board.GetBoardDimensions () - 1;
        return !(removalmap [playerOne] || ((x == 0 || x == max) && (y == 0 || y == max)));
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
                if ((x + y) % 2 == 1)
                {
                    boardString [x] [y] = "uk";
                } 
                else 
                {
                    ICard card = board.GetCardAtSpace (x, y);
                    if (card == null) 
                    {
                        boardString [x] [y] = "none";
                    } 
                    else 
                    {
                        boardString [x] [y] = card.getFullCard ();
                    }
                }
            }
        }
        return boardString;
    }

    public bool isPlayerOneTurn()
    {
        return playerOneTurn;
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
