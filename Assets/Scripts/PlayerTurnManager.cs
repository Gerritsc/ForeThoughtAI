using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour {

    [SerializeField]
    HandCardStruct SelectedCard;

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
        SelectedCard = null;
        swapflag = false;
    }
	
	// Update is called once per frame
	void Update () {

        //Deselect/Cancel all actions
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelActions();
        }

        else if (Input.GetMouseButtonDown(0))
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
                    if (swapflag && SelectedSpace != null)
                    {
                        //attempting to swap with empty space
                        if (card == null)
                        {

                        }
                        //check for same space swap
                        if (SelectedSpace.x == space.x && SelectedSpace.y == space.y) {return;}

                        else
                        {
                            //Checks for valid swap
                            if (manager.gameModel.canSwap(SelectedSpace.x, SelectedSpace.y, space.x, space.y))
                            {
                                manager.gameModel.SwapCards(0, SelectedSpace.x, SelectedSpace.y, space.x, space.y);
                                manager.updateBoard();
                                SelectedSpace.setOutlineColor(0);
                                SelectedSpace = null;
                                FindObjectOfType<buttonManager>().Deselect();
                                swapflag = false;
                            }
                        }
                    }
                    //Select/Deselect Space
                    else if (card != null)
                    {
                        SelectSpaceFunctionality(space, card);
                    }

                    //Play Card on space
                    else if (SelectedCard != null)
                    {
                        manager.gameModel.PlayCard(0, space.x, space.y, SelectedCard.card);
                        SelectedCard.setOutlineColor(0);
                        SelectedCard = null;
                        manager.updateBoard();
                    }
                }
                //Select/Deselect card in hand
                else if (hit.transform.tag == "Hand")
                {
                    
                    var cardspace = hit.transform.GetComponent<HandCardStruct>();
                    SelectHandFunctionality(cardspace);
                }
            }
        }
    }

    public void setSwapFlag()
    {
        swapflag = true;
        
    }

    public void StartPeek()
    {
        StartCoroutine("peekFunctionality", 2f);
    }

    //flips a face down card, displaying its value
    IEnumerator peekFunctionality(float time)
    {
        var spaceToPeek = SelectedSpace;
        spaceToPeek.flipUp();
        yield return new WaitForSeconds(time);

        spaceToPeek.flipBackDown();

    }

    public void SelectHandFunctionality(HandCardStruct cardspace)
    {
        bool deselected = false;
        cardspace.setOutlineColor(1);

        if (SelectedCard != null)
        {
            SelectedCard.setOutlineColor(0);

            if (SelectedCard.pos == cardspace.pos)
            {
                SelectedCard = null;
                deselected = true;
            }
        }

        if (!deselected)
        {
            SelectedCard = cardspace;
            cardspace.setOutlineColor(1);
            if (SelectedSpace != null)
            {
                SelectedSpace.setOutlineColor(0);
                SelectedSpace = null;
                FindObjectOfType<buttonManager>().Deselect();
            }
        }
    }

    public void SelectSpaceFunctionality(BoardSpaceStruct space, ICard card)
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
            if (SelectedCard != null)
            {
                SelectedCard.setOutlineColor(0);
                SelectedCard = null;
            }
            onSpaceSelect(SelectedSpace, manager.gameModel);

        }
    }

    public void CancelActions()
    {
        swapflag = false;
        SelectedSpace.setOutlineColor(0);
        SelectedCard.setOutlineColor(0);
        SelectedSpace = null;
        SelectedCard = null;
        FindObjectOfType<buttonManager>().Deselect();
    }
}
