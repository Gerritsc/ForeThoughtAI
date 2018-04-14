﻿using System;
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
        var playsToMake = model.gameModel.getAllPlayerMoves(model.gameModel.getBoard(),false);

        int rand = UnityEngine.Random.Range(0, playsToMake.Count - 1);

        var playToMake = playsToMake[rand];

        switch (playToMake.type) {
            case MoveType.ADD:
                {
                    model.gameModel.PlayCard(1, playToMake.x1, playToMake.y1, playToMake.card);
                    break;
                }
            case MoveType.SWAP:
                {
                    model.gameModel.SwapCards(1, playToMake.x1, playToMake.y1, playToMake.x2, playToMake.y2);
                    StartCoroutine(DisplaySwapped(playToMake, 2.5f));
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
        DisplayText.enabled = true;


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

    private bool CheckTerminalBoard(IGame curGame, IBoard board)
    {
        bool terminal = false;
        List<ICard> toTestDiag1 = new List<ICard>();
        List<ICard> toTestDiag2 = new List<ICard>();
        for (int i = 0; i < 5 && !terminal; i++)
        {
            List<ICard> toTestHoriz = new List<ICard>();
            List<ICard> toTestVert = new List<ICard>();
            ICard card1 = board.GetCardAtSpace(i, i);
            ICard card2 = board.GetCardAtSpace(i, 4 - i);
            if (card1 != null)
            {
                toTestDiag1.Add(card1);
            }
            if (card2 != null)
            {
                toTestDiag2.Add(card2);
            }
            for (int j = 0; j < 5 && (toTestHoriz.Count == j || toTestVert.Count == j); j++)
            {
                ICard card3 = board.GetCardAtSpace(j, i);
                ICard card4 = board.GetCardAtSpace(i, j);
                if (card3 != null)
                {
                    toTestHoriz.Add(card3);
                }
                if (card4 != null)
                {
                    toTestVert.Add(card4);
                }
            }

            if (toTestVert.Count != 5 && toTestHoriz.Count != 5)
            {
                continue;
            }

            foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
            {
                terminal = curGame.CheckGameOverClaim(toTestHoriz, t) ||
                    curGame.CheckGameOverClaim(toTestVert, t);

                if (terminal)
                {
                    return terminal;
                }
            }
        }
        foreach (HANDTYPE t in Enum.GetValues(typeof(HANDTYPE)))
        {
            terminal = curGame.CheckGameOverClaim(toTestDiag1, t) || curGame.CheckGameOverClaim(toTestDiag2, t);
        }
        return terminal;
    }
}
