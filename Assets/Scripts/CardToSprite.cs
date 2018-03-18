using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardToSprite : MonoBehaviour {

    public Sprite[] cards;

    public Sprite getFaceDown()
    {
        return cards[52];
    }

    public Sprite getSprite(ICard card)
    {
        Debug.Log(card.getFullCard());
        string suit = card.getSuitorColor();
        int val = card.GetCardNumValue() -2;
        int arrval = 0;

        switch (suit.ToLower())
        {
            case "hearts":
                arrval += val;
                break;
            case "diamonds":
                val += 13;
                arrval += val;
                break;
            case "clubs":
                val += 26;
                arrval += val;
                break;
            case "spades":
                val += 39;
                arrval += val;
                break;
            default:
                throw new System.Exception();
        }

        return cards[arrval];
    }
}
