using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Our Little AI Friendo
public class EnemyTurnMaker : MonoBehaviour {

    ModelManager model;

	// Use this for initialization
	void Start () {
        model = FindObjectOfType<ModelManager>();
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
}
