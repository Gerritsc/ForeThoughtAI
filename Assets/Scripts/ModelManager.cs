using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour {

    public IGame gameModel;

    public delegate void BoardChangeAction(IGame gameModel);
    public event BoardChangeAction OnBoardChange;

    // Use this for initialization
    void Awake () {
        gameModel = new Game();
	}

    void RestartGame()
    {
        gameModel = gameModel.RestartGame();
    }

    public void updateBoard()
    {
        OnBoardChange(gameModel);
    }
}
