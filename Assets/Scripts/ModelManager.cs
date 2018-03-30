using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{

    public IGame gameModel;

    [SerializeField]
    public GameState currentstate;

    public delegate void BoardChangeAction(IGame gameModel);
    public event BoardChangeAction OnBoardChange;

    


    public enum GameState
    {
        PlayerPlay, PlayerClaim, EnemyTurn
    }

    // Use this for initialization
    void Awake()
    {
        gameModel = new Game();
        FindObjectOfType<HandRender>().InitHand();
        FindObjectOfType<BoardRenderer>().initBoard();
        FindObjectOfType<ClaimGameRenderer>().initClaims();
    }

    //Changes the current 
    public void switchState()
    {
        switch (currentstate)
        {
            case GameState.EnemyTurn:
                {
                    //Switch from enemy turn to PlayerPlay
                    currentstate = GameState.PlayerPlay;
                    FindObjectOfType<PlayerTurnManager>().enabled = true;
                    break;

                }
            case GameState.PlayerPlay:
                {
                    //Switch after player makes an action to claim/end phase
                    currentstate = GameState.PlayerClaim;
                    FindObjectOfType<PlayerTurnManager>().enabled = false;

                    break;
                }
            case GameState.PlayerClaim:
                {
                    //Switch from claim phase to enemy turn
                    currentstate = GameState.EnemyTurn;
                    break;
                }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            switchState();
        }
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
