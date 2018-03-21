using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceStruct : MonoBehaviour {


    public int x, y;


    public Outline getOutlineComponent()
    {
        return gameObject.GetComponent<Outline>();
    }

    public ICard getCard()
    {
        return FindObjectOfType<ModelManager>().gameModel.getBoard().GetCardAtSpace(x, y);
    }

    public void setOutlineColor(int color)
    {
        getOutlineComponent().color = color;
    }


    public bool isFaceup()
    {
        return (x + y) % 2 == 0;
    }

    //flips up this space's card (must be a face down card)
    public void flipUp()
    {
        var card = getCard();
        GetComponent<SpriteRenderer>().sprite = FindObjectOfType<CardToSprite>().getSprite(card);
    }

    public void flipBackDown()
    {
        GetComponent<SpriteRenderer>().sprite = FindObjectOfType<CardToSprite>().getFaceDown();
    }
}
