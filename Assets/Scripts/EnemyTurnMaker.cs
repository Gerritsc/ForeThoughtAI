using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Our Little AI Friendo
public class EnemyTurnMaker : MonoBehaviour {

    ModelManager model;

    [SerializeField]
    Text DisplayText;

	// Use this for initialization
	void Start () {
        model = FindObjectOfType<ModelManager>();
        DisplayText.enabled = false;
        DisplayText.text = "The opponent has swapped 2 cards";
	}

    public void takeTurn()
    {
        var playToMake = model.gameModel.getAllPlayerMoves(model.gameModel.getBoard(),false)[0];

        switch (playToMake.type) {
            case MoveType.ADD:
                {
                    model.gameModel.PlayCard(1, playToMake.x1, playToMake.y1, playToMake.card);
                    break;
                }
            case MoveType.SWAP:
                {
                    model.gameModel.SwapCards(1, playToMake.x1, playToMake.y1, playToMake.x2, playToMake.y2);
                    StartCoroutine(DisplaySwapped(playToMake, 1.5f));
                    break;
                }
            case MoveType.REMOVE:
                {
                    model.gameModel.RemoveCard(1, playToMake.x1, playToMake.y1);
                    break;
                }
            case MoveType.PEEK:
                {
                    break;
                }
        }

        model.updateBoard();
        model.switchState();

    }

    private IEnumerator DisplaySwapped(GameMove g, float time)
    {
        DisplayText.enabled = false;


        //get spaces that were swapped
        var space1 = (from space in FindObjectsOfType<BoardSpaceStruct>() where (space.x == g.x1 && space.y == g.y1) select space).FirstOrDefault();
        var space2 = (from space in FindObjectsOfType<BoardSpaceStruct>() where (space.x == g.x2 && space.y == g.y2) select space).FirstOrDefault();

        space1.setOutlineColor(2);
        space2.setOutlineColor(2);

        yield return new WaitForSeconds(time);

        space1.setOutlineColor(0);
        space2.setOutlineColor(0);

        DisplayText.enabled = false;
    }
}
