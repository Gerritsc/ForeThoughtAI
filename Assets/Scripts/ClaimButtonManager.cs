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

	// Use this for initialization
	void Start () {
        EndTurnButton.SetActive(false);
        FlushButton.SetActive(false);
        StraightButton.SetActive(false);
        FullHouseButton.SetActive(false);
        FourKindButton.SetActive(false);


    }
}
