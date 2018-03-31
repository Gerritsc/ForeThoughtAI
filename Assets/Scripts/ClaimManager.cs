using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimManager : MonoBehaviour {

    ClaimGameStruct selectedButton;
    BoardSpaceStruct[] selectedCards;

    public delegate void SpaceSelectAction(ClaimGameStruct selected, BoardSpaceStruct[] cards);
    public event SpaceSelectAction onClaimSelect;
    public event SpaceSelectAction onClaimDeselect;

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
}
