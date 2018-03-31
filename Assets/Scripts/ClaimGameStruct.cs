using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClaimGameStruct : MonoBehaviour
{

    //The direction to check for 5 cards.
    public enum Direction
    {
        Row,
        Column,
        Diagonal
    }

    [SerializeField]
    public Direction direction;

    public int depthnumber;


    public bool isValidWin()
    {
        return false;
    }

    public Outline getOutlineComponent()
    {
        return gameObject.GetComponent<Outline>();
    }

    public void setOutlineColor(int color)
    {
        getOutlineComponent().color = color;
    }

    //Gets all the board spaces at this row
    public BoardSpaceStruct[] GetCardSet()
    {
        var ret = new BoardSpaceStruct[5];

        switch (direction)
        {
            case Direction.Row:
                {
                    ret = (from x in FindObjectsOfType<BoardSpaceStruct>() where x.y == this.depthnumber select x).ToArray();
                    break;
                }
            case Direction.Column:
                {
                    ret = (from x in FindObjectsOfType<BoardSpaceStruct>() where x.x == this.depthnumber select x).ToArray();
                    break;
                }
            case Direction.Diagonal:
                {
                    if (depthnumber == 0)
                    {

                    }
                    else if (depthnumber == 1)
                    {

                    }
                    else
                    {
                        throw new System.Exception("Yo dawg what are you doin");
                    }
                    break;
                }
        }
        return ret;

    }
}
