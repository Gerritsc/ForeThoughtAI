using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimGameRenderer : MonoBehaviour
{

    [SerializeField]
    float X_OFFSET;
    [SerializeField]
    float Y_OFFSET;
    [SerializeField]
    float Start_X;
    [SerializeField]
    float Start_Y;

    [SerializeField]
    Sprite icon;

    /// <summary>
    /// Buttons that stretch horizontally across the game board. STORES BUTTONS FOR COLUMNS 
    /// </summary>
    GameObject[] ColumnButtons;
    /// <summary>
    /// Buttons that stretch vertically down the side of the board.  STORES BUTTONS FOR ROWS
    /// </summary>
    GameObject[] RowButtons;
    GameObject[] Diagonals;

    IBoard board;

    public void initClaims()
    {
        board = FindObjectOfType<ModelManager>().gameModel.getBoard();
        ColumnButtons = new GameObject[board.GetBoardDimensions()];
        RowButtons = new GameObject[board.GetBoardDimensions()];
        Diagonals = new GameObject[2];
        //Horizontal 
        for (int x = 0; x < ColumnButtons.Length; x++)
        {
            var obj = new GameObject();
            obj.gameObject.name = "Claim Horizontal: " + x.ToString();
            obj.tag = "ClaimGame";
            obj.transform.Rotate(new Vector3(90, 0, 180));
            obj.transform.localScale = new Vector3(.2f, .2f, .2f);
            obj.transform.position = new Vector3(Start_X + (x * X_OFFSET), 1.01f, Start_Y - .5f);
            obj.transform.parent = this.gameObject.transform;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = icon;
            obj.AddComponent<cakeslice.Outline>();
            var col = obj.AddComponent<BoxCollider>();
            var spacestruct = obj.AddComponent<ClaimGameStruct>();
            spacestruct.depthnumber = x;
            spacestruct.direction = ClaimGameStruct.Direction.Column;
            col.size = new Vector3(10, 10f, 6f);
            col.isTrigger = true;
            obj.SetActive(false);
            ColumnButtons[x] = obj;
        }

        for (int y = 0; y < RowButtons.Length; y++)
        {
            var obj = new GameObject();
            obj.gameObject.name = "Claim Vertical: " + y.ToString();
            obj.tag = "ClaimGame";
            obj.transform.Rotate(new Vector3(90, 0, -90));
            obj.transform.localScale = new Vector3(.2f, .2f, .2f);
            obj.transform.position = new Vector3(Start_X - X_OFFSET, 1.01f, Start_Y + ((y + 1) * Y_OFFSET));
            obj.transform.parent = this.gameObject.transform;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = icon;
            obj.AddComponent<cakeslice.Outline>();
            var col = obj.AddComponent<BoxCollider>();

            var spacestruct = obj.AddComponent<ClaimGameStruct>();
            spacestruct.depthnumber = y;
            spacestruct.direction = ClaimGameStruct.Direction.Row;

            col.size = new Vector3(10, 8f, 6f);
            col.isTrigger = true;
            obj.SetActive(false);

            RowButtons[y] = obj;
        }
        initDiagonals();
    }

    // Use this for initialization
    private void initDiagonals()
    {
        //Diagonals
        var obj  = new GameObject();
        obj.gameObject.name = "Diagonal1";
        obj.tag = "ClaimGame";
        obj.transform.Rotate(new Vector3(90, 0, -135));
        obj.transform.localScale = new Vector3(.2f, .2f, .2f);
        obj.transform.position = new Vector3(Start_X - X_OFFSET, 1.01f, Start_Y - 1);
        obj.transform.parent = this.gameObject.transform;
        var drender = obj.AddComponent<SpriteRenderer>();
        drender.sprite = icon;
        obj.AddComponent<cakeslice.Outline>();
        var dcol = obj.AddComponent<BoxCollider>();

        var spacestruct = obj.AddComponent<ClaimGameStruct>();
        spacestruct.depthnumber = 0;
        spacestruct.direction = ClaimGameStruct.Direction.Diagonal;

        dcol.size = new Vector3(10, 8f, 6f);
        dcol.isTrigger = true;
        obj.SetActive(false);
        Diagonals[0] = obj;


        obj = new GameObject();
        obj.gameObject.name = "Diagonal2";
        obj.tag = "ClaimGame";
        obj.transform.Rotate(new Vector3(90, 0, 135));
        obj.transform.localScale = new Vector3(.2f, .2f, .2f);
        obj.transform.position = new Vector3(Start_X - .5f + (X_OFFSET * (board.GetBoardDimensions())), 1.01f, Start_Y - 1);
        obj.transform.parent = this.gameObject.transform;
        drender = obj.AddComponent<SpriteRenderer>();
        drender.sprite = icon;
        obj.AddComponent<cakeslice.Outline>();
        dcol = obj.AddComponent<BoxCollider>();

        spacestruct = obj.AddComponent<ClaimGameStruct>();
        spacestruct.depthnumber = 1;
        spacestruct.direction = ClaimGameStruct.Direction.Diagonal;


        dcol.size = new Vector3(10, 8f, 6f);
        dcol.isTrigger = true;
        obj.SetActive(false);

        Diagonals[1] = obj;
    }


    public void EnableClaims(IGame model)
    {
        for (int x = 0; x < ColumnButtons.Length; x++)
        {
            if (model.isFullColumn(x))
            {
                ColumnButtons[x].SetActive(true);
            }
        }

        for (int y = 0; y < RowButtons.Length; y++)
        {
             if (model.isFullRow(y))
            {
                RowButtons[y].SetActive(true);
            }
        }
        for (int x = 0; x < Diagonals.Length; x ++)
        {
            if (model.isFullDiagonal((x == 0)))
            {
                Diagonals[x].SetActive(true);
            }
        }
    }

    public void disableClaims()
    {
        foreach (var elem in ColumnButtons)
        {
            elem.SetActive(false);
        }
        foreach (var elem in RowButtons)
        {
            elem.SetActive(false);
        }
        foreach (var elem in Diagonals)
        {
            elem.SetActive(false);
        }
    }
}
