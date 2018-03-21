using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardStruct : MonoBehaviour {
    
    [SerializeField]
    public ICard card;
    [SerializeField]
    public int pos;

    public Outline getOutlineComponent()
    {
        return gameObject.GetComponent<Outline>();
    }

    public void setOutlineColor(int color)
    {
        getOutlineComponent().color = color;
    }

}
