using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BoardSpace
{
    int xpos { get; set; }
    int ypos { get; set; }
    ICard card { get; set; }
    bool isfaceup;


    public BoardSpace(int x, int y, bool isfaceup, ICard card = null)
    {
        this.xpos = x;
        this.ypos = y;
        this.isfaceup = isfaceup;
        if (card != null)
        {
            this.card = card;
        }
    }

    public void setCard(ICard newcard)
    {
        this.card = newcard;
    }

    public ICard getCard()
    {
        return card;
    }

    public bool isFaceUp()
    {
        return isfaceup;
    }
    /// <summary>
    /// sets the given card to the given ICard, then returns the previous card.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public ICard swapCard(ICard card)
    {
        if (this.card != null)
        {

            var ret = this.card;
            this.card = card;
            Console.WriteLine(String.Format("Old: {0}       New: {1}", ret.getFullCard(), this.card.getFullCard()));
            return ret;

        }
        else
        {
            throw new ArgumentException("You are trying to swap cards on an empty space.");
        }
    }

    public void removeCard()
    {
        if (this.card != null)
        {
            //deinstance the card to this boardspace
            this.card = null;
        }
        else throw new ArgumentException("No card here");
    }
}
