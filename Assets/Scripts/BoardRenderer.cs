using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardRenderer : MonoBehaviour 
{

    readonly int ROWCOUNT = 5;

    [SerializeField]
    float BOARDWIDTH;
    [SerializeField]
    float X_OFFSET;
    [SerializeField]
    float Y_OFFSET;

    [SerializeField]
    float startX;

    [SerializeField]
    float startY;

    [SerializeField]
    float cardHeight;

    [SerializeField]
    GameObject background;

    GameObject[,] boardobjects;

    IBoard board;

    private void OnEnable()
    {
        FindObjectOfType<ModelManager>().OnBoardChange += UpdateBoard;
    }

    private void OnDisable()
    {
        FindObjectOfType<ModelManager>().OnBoardChange -= UpdateBoard;
    }

    // Use this for initialization
    public void initBoard () 
    {
        board = FindObjectOfType<ModelManager>().gameModel.getBoard();
        //background.transform.localScale = new Vector3(BOARDWIDTH, 0, BOARDWIDTH);
        boardobjects = new GameObject[ROWCOUNT, ROWCOUNT];
        var cardtosprite = FindObjectOfType<CardToSprite>();
        for (int y = 0; y < ROWCOUNT; y ++)
        {
            for (int x = 0; x < ROWCOUNT; x++)
            {
                var obj = new GameObject();
                obj.gameObject.name = x.ToString() + ","+ y.ToString();
                obj.transform.Rotate(new Vector3(90, 0, 0));
                obj.transform.localScale = new Vector3(2, 2, 2);
                obj.transform.position = new Vector3(startX + (x * X_OFFSET), cardHeight, startY + (y * Y_OFFSET));
                obj.transform.parent = background.transform;
                obj.AddComponent<SpriteRenderer>();
                obj.AddComponent<cakeslice.Outline>();
                var col = obj.AddComponent<BoxCollider>();
                col.size = new Vector3(1, 1f, .6f);
                col.isTrigger = true;
                obj.tag = "BoardSpace";
                var bstruct = obj.AddComponent<BoardSpaceStruct>();
                bstruct.x = x;
                bstruct.y = y;


                var card = board.GetCardAtSpace(x, y);
                if (card != null)
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                    obj.GetComponent<SpriteRenderer>().sprite = cardtosprite.getSprite(card);
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    obj.GetComponent<SpriteRenderer>().sprite = cardtosprite.getEmptySpace();
                }
                boardobjects[x, y] = obj;
            }

        }
    }

    public void UpdateBoard(IGame model)
    {
        board = model.getBoard();
        renderboard();
        
    }

    private void renderboard()
    {
        var cardtosprite = FindObjectOfType<CardToSprite>();
        for (int y = 0; y < ROWCOUNT; y++)
        {
            for (int x = 0; x < ROWCOUNT; x++)
            {
                var card = board.GetCardAtSpace(x, y);
                if (card != null)
                {
                    boardobjects[x,y].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                    if ((x + y) % 2 != 0)
                    {
                        boardobjects[x, y].GetComponent<SpriteRenderer>().sprite = cardtosprite.getFaceDown();
                    }
                    else
                    {
                        boardobjects[x, y].GetComponent<SpriteRenderer>().sprite = cardtosprite.getSprite(card);
                    }
                }
                else
                {
                    boardobjects[x, y].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    boardobjects[x,y].GetComponent<SpriteRenderer>().sprite = cardtosprite.getEmptySpace();
                }
            }
        }
    }


}
