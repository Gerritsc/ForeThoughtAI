using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelManager : MonoBehaviour
{

    public IGame gameModel;

    [SerializeField]
    Text DisplayTurn;

    [SerializeField]
    private GameState currentstate;

    public delegate void BoardChangeAction(IGame gameModel);
    public event BoardChangeAction OnBoardChange;

    public enum GameState
    {
        PlayerPlay, PlayerClaim, EnemyTurn, WinState
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

                    gameModel.switchTurn();

                    break;

                }
            case GameState.PlayerPlay:
                {
                    //Switch after player makes an action to claim/end phase
                    currentstate = GameState.PlayerClaim;
                    FindObjectOfType<PlayerTurnManager>().enabled = false;
                    FindObjectOfType<ClaimManager>().enabled = true;
                    FindObjectOfType<ClaimGameRenderer>().EnableClaims(gameModel);
                    break;
                }
            case GameState.PlayerClaim:
                {
                    //Switch from claim phase to enemy turn
                    currentstate = GameState.EnemyTurn;
                    FindObjectOfType<ClaimManager>().enabled = false;
                    FindObjectOfType<ClaimGameRenderer>().disableClaims();
                    FindObjectOfType<EnemyTurnMaker>().takeTurn();
                    gameModel.switchTurn();
                    break;
                }
        }
        changeDisplayText();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            switchState();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            var list = gameModel.getAllPlayerMoves(gameModel.getBoard(),true);
            Debug.Log(list);
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


    /// <summary>
    /// Changes display text to display Current State
    /// </summary>
    private void changeDisplayText()
    {
        switch (currentstate)
        {
            case GameState.EnemyTurn:
                {
                    DisplayTurn.text = "Enemy Turn";
                    break;
                }
            case GameState.PlayerPlay:
                {
                    DisplayTurn.text = "Player Play Phase";
                    break;
                }
            case GameState.PlayerClaim:
                {
                    DisplayTurn.text = "Claim Win/End of Turn Phase";
                    break;
                }
            case GameState.WinState:
                {
                    DisplayTurn.text = "You Win goodsir";
                    break;
                }
        }

    }

    public void GameWon()
    {
        this.currentstate = GameState.WinState;

        FindObjectOfType<ClaimManager>().enabled = false;
        FindObjectOfType<ClaimGameRenderer>().disableClaims();

        changeDisplayText();
        //gameModel = gameModel.RestartGame();
        //OnBoardChange(gameModel);

    }
}
