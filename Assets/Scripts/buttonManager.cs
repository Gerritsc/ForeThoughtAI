using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonManager : MonoBehaviour {



    [SerializeField]
    GameObject swapButton;
    [SerializeField]
    GameObject peekButton;
    [SerializeField]
    GameObject removeButton;

    PlayerTurnManager turnmanager;

    private void OnEnable()
    {
        FindObjectOfType<PlayerTurnManager>().onSpaceSelect += onSelect;
        FindObjectOfType<PlayerTurnManager>().onSpaceDeselect += onDeselect;

    }

    private void OnDisable()
    {
        FindObjectOfType<PlayerTurnManager>().onSpaceSelect -= onSelect;
        FindObjectOfType<PlayerTurnManager>().onSpaceDeselect += onDeselect;
    }

    private void Awake()
    {
        turnmanager = FindObjectOfType<PlayerTurnManager>();
        swapButton.SetActive(false);
        peekButton.SetActive(false);
        removeButton.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void onSelect(BoardSpaceStruct space, IGame model)
    {
        peekButton.SetActive(false);
        swapButton.SetActive(true);

		if (model.canRemove (0, space.x, space.y)) 
		{
			removeButton.SetActive (true);
		}

        if (!space.isFaceup())
        {
            peekButton.SetActive(true);
        }

    }

    public void Deselect()
    {
        swapButton.SetActive(false);
        peekButton.SetActive(false);
        removeButton.SetActive(false);
    }

    public void onDeselect(BoardSpaceStruct space, IGame model)
    {
        Deselect();
    }

    //Not working ATM
    public void SwapButtonSelected()
    {
        peekButton.SetActive(false);
        removeButton.SetActive(false);
        swapButton.GetComponent<Image>().color = new Color(242, 48,68);
    }

    public void swapButtonCancelled()
    {
        swapButton.GetComponent<Image>().color = new Color(255, 255, 255);
    }
}
