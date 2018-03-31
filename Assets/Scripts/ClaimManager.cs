using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClaimManager : MonoBehaviour {

    ClaimGameStruct selectedButton;
    BoardSpaceStruct[] selectedCards;

    public delegate void SpaceSelectAction(ClaimGameStruct selected, BoardSpaceStruct[] cards);
    public event SpaceSelectAction onClaimSelect;
    public event SpaceSelectAction onClaimDeselect;

    ModelManager model;

    private void OnEnable()
    {
        FindObjectOfType<ClaimButtonManager>().EnableEndTurn();
    }

    private void OnDisable()
    {
        ResetSelect();
        FindObjectOfType<ClaimButtonManager>().disableButtons();
    }

    // Use this for initialization
    void Awake()
    {
        model = FindObjectOfType<ModelManager>();
        selectedButton = null;
        selectedCards = new BoardSpaceStruct[5];
    }

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedButton = null;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "ClaimGame")
                {
                    var button = hit.transform.GetComponent<ClaimGameStruct>();
                    SelectFunctionality(button);
                }
            }
        }
	}

    private void SelectFunctionality(ClaimGameStruct button)
    {
        bool deselected = false;

        if (selectedButton != null)
        {
            setSelectColor(0);

            if (button.depthnumber == selectedButton.depthnumber && button.direction == selectedButton.direction)
            {
                ResetSelect();
                deselected = true;
                onClaimDeselect(selectedButton, selectedCards);
                
            }
        }
        
        if (!deselected)
        {
            selectedButton = button;
            selectedCards = selectedButton.GetCardSet();
            setSelectColor(2);
            onClaimSelect(selectedButton, selectedCards);
        }

    }

    private void ResetSelect()
    {
        if (selectedButton != null && selectedCards != null)
        {
            setSelectColor(0);
        }
        selectedButton = null;
        for (int i = 0; i < selectedCards.Length; i++)
        {
            selectedCards[i] = null;
        }
    }
    private void setSelectColor(int index)
    {
        selectedButton.setOutlineColor(index);
        foreach (var space in selectedCards)
        {
            if (space != null)
            {
                space.setOutlineColor(index);
            }
        }
    }

    public void CheckClaim(string type)
    {
        bool win = false;
        List<ICard> cardset = (from card in selectedCards select card.getCard()).ToList();
        switch (type)
        {
            case "FLUSH": {
                    win = model.gameModel.CheckGameOverClaim(cardset, HANDTYPE.FLUSH);
                    break;
                }
            case "STRAIGHT":
                {
                    win = model.gameModel.CheckGameOverClaim(cardset, HANDTYPE.STRAIGHT);
                    break;
                }
            case "FULLHOUSE":
                {
                    win = model.gameModel.CheckGameOverClaim(cardset, HANDTYPE.FULLHOUSE);
                    break;
                }
            case "FOURKIND":
                {
                    win = model.gameModel.CheckGameOverClaim(cardset, HANDTYPE.FOURKIND);
                    break;
                }
        }
        if (win)
        {
            model.GameWon();
        }
    }
}
