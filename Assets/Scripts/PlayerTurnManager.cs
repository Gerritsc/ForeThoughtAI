using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour {

    [SerializeField]
    ICard SelectedCard;

    BoardSpaceStruct SelectedSpace;

    ModelManager manager;

    public delegate void SpaceSelectAction(BoardSpaceStruct space, IGame game);
    public event SpaceSelectAction onSpaceSelect;
    public event SpaceSelectAction onSpaceDeselect;

    bool swapflag;

	// Use this for initialization
	void Awake () {
        manager = FindObjectOfType<ModelManager>();
        SelectedSpace = null;
        SelectedCard = new PlayingCard("Hearts", 11);
        swapflag = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "BoardSpace")
                {
                    var space = hit.transform.GetComponent<BoardSpaceStruct>();
                    var card = space.getCard();
                    //Swap functionality
                    if (swapflag && card != null && SelectedSpace != null)
                    {
                        //check for same space swap
                        if (SelectedSpace.x == space.x && SelectedSpace.y == space.y) {return;}
                    }
                    //Select/Deselect Space
                    else if (card != null)
                    {
                        SelectFunctionality(space, card);
                    }

                    //Play Card on space
                    else if (SelectedCard != null)
                    {
                        manager.gameModel.PlayCard(0, space.x, space.y, SelectedCard);
                        manager.updateBoard();
                    }

                }
            }
        }
    }

    public void setSwapFlag()
    {
        swapflag = true;
    }

    public void SelectFunctionality(BoardSpaceStruct space, ICard card)
    {
        bool deselected = false;
        if (SelectedSpace != null)
        {
            SelectedSpace.setOutlineColor(0);

            //Deselect selected space
            if (SelectedSpace.x == space.x && space.y == SelectedSpace.y)
            {
                SelectedSpace = null;
                deselected = true;
                onSpaceDeselect(space, manager.gameModel);
            }
        }

        if (!deselected)
        {
            SelectedSpace = space;
            space.setOutlineColor(1);
            onSpaceSelect(SelectedSpace, manager.gameModel);

        }
    }
}
