using System;
using System.Collections;
using System.Collections.Generic;

public enum MoveType { ADD = 0, SWAP, REMOVE, PEEK }

public class GameMove
{
    public int x1;
    public int y1;
    public int x2;
    public int y2;
    public ICard card;
    public MoveType type;

    public GameMove(GameMove other)
    {
        this.x1 = other.x1;
        this.y1 = other.y1;
        this.x2 = other.x2;
        this.y2 = other.y2;
        this.card = other.card;
        this.type = other.type;
    }

    public GameMove(int x1, int y1, bool isPeek)
    {
        if (isPeek)
        {
            this.type = MoveType.PEEK;
        }
        else
        {
            this.type = MoveType.REMOVE;
        }
        this.x1 = x1;
        this.y1 = y1;
    }

    public GameMove(int x1, int y1, int x2, int y2)
    {
        this.type = MoveType.SWAP;
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }

    public GameMove(int x1, int y1, ICard card)
    {
        this.type = MoveType.ADD;
        this.x1 = x1;
        this.y1 = y1;
        this.card = card;
    }

    public override bool Equals(object obj)
    {
        if (obj != null && (obj is GameMove) && ((GameMove)obj).type == this.type)
        {
            GameMove other = (GameMove)obj;
            switch (this.type)
            {
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

    public override string ToString()
    {
        string typeStr = "";
        string typeSpecificStr = "";
        switch (this.type)
        {
            case MoveType.ADD:
                typeStr = "ADD";
                typeSpecificStr = String.Format("X-Pos: {1}\nY-Pos: {2}\n{3}", x1.ToString(), y1.ToString(), card.ToString());
                break;
            case MoveType.SWAP:
                typeStr = "SWAP";
                typeSpecificStr = String.Format("Orig X-Pos: {1}\nOrig Y-Pos: {2}\nNext X-Pos: {3}\n Next Y-Pos: {4}", x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString());
                break;
            case MoveType.REMOVE:
                typeStr = "REMOVE";
                typeSpecificStr = String.Format("X-Pos: {1}\nY-Pos: {2}", x1.ToString(), y1.ToString());
                break;
        }
        return string.Format("[GameMove]\n{1}\n{2}", typeStr, typeSpecificStr);
    }
}
