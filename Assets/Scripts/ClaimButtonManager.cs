using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimButtonManager : MonoBehaviour {

    [SerializeField]
    GameObject EndTurnButton;
    [SerializeField]
    GameObject FlushButton;
    [SerializeField]
    GameObject StraightButton;
    [SerializeField]
    GameObject FullHouseButton;
    [SerializeField]
    GameObject FourKindButton;

    private void OnEnable()
    {
        FindObjectOfType<ClaimManager>().onClaimSelect += EnableButtons;
        FindObjectOfType<ClaimManager>().onClaimDeselect += DisableButtons;
    }
    private void OnDisable()
    {
        FindObjectOfType<ClaimManager>().onClaimSelect -= EnableButtons;
        FindObjectOfType<ClaimManager>().onClaimDeselect -= DisableButtons;
    }

    // Use this for initialization
    void Start () {
        EndTurnButton.SetActive(false);
        FlushButton.SetActive(false);
        StraightButton.SetActive(false);
        FullHouseButton.SetActive(false);
        FourKindButton.SetActive(false);
    }

    public void disableButtons()
    {
        EndTurnButton.SetActive(false);
        FlushButton.SetActive(false);
        StraightButton.SetActive(false);
        FullHouseButton.SetActive(false);
        FourKindButton.SetActive(false);
    }

    private void EnableButtons(ClaimGameStruct button, BoardSpaceStruct[] cards)
    {
        EndTurnButton.SetActive(true);
        FlushButton.SetActive(true);
        StraightButton.SetActive(true);
        FullHouseButton.SetActive(true);
        FourKindButton.SetActive(true);
    }

    private void DisableButtons(ClaimGameStruct button, BoardSpaceStruct[] cards)
    {
        disableButtons();
    }

    public void EnableEndTurn()
    {
        EndTurnButton.SetActive(true);
    }
}
