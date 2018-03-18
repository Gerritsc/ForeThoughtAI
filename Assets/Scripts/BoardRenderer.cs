using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer : MonoBehaviour {

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

    }

    private void OnDisable()
    {
        
    }

    // Use this for initialization
    void Awake () {
        board = FindObjectOfType<ModelManager>().gameModel.getBoard();
        //background.transform.localScale = new Vector3(BOARDWIDTH, 0, BOARDWIDTH);
        boardobjects = new GameObject[ROWCOUNT, ROWCOUNT];
        var cardtosprite = FindObjectOfType<CardToSprite>();
        for (int y = 0; y < ROWCOUNT; y ++)
        {
            for (int x = 0; x < ROWCOUNT; x++)
            {
                var obj = new GameObject();
                obj.transform.Rotate(new Vector3(90, 0, 0));
                obj.transform.localScale = new Vector3(2, 2, 2);
                obj.transform.position = new Vector3(startX + (x * X_OFFSET), cardHeight, startY + (y * Y_OFFSET));
                obj.AddComponent<SpriteRenderer>();
                var card = board.GetCardAtSpace(x, y);
                if (card != null)
                {
                    obj.GetComponent<SpriteRenderer>().sprite = cardtosprite.getSprite(card);
                } 
                boardobjects[x, y] = obj;
            }

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
