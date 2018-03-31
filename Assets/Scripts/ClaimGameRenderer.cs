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

    GameObject[] HorizontalButtons;
    GameObject[] VerticalButtons;
    GameObject[] Diagonals;

    IBoard board;

    public void initClaims()
    {
        board = FindObjectOfType<ModelManager>().gameModel.getBoard();
        HorizontalButtons = new GameObject[board.GetBoardDimensions()];
        VerticalButtons = new GameObject[board.GetBoardDimensions()];
        Diagonals = new GameObject[2];
        //Horizontal 
        for (int x = 0; x < HorizontalButtons.Length; x++)
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
            col.size = new Vector3(10, 10f, 6f);
            col.isTrigger = true;
            obj.SetActive(false);
            HorizontalButtons[x] = obj;
        }

        for (int y = 0; y < VerticalButtons.Length; y++)
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
            col.size = new Vector3(10, 8f, 6f);
            col.isTrigger = true;
            obj.SetActive(false);

            VerticalButtons[y] = obj;
        }
        initDiagonals();
    }

    // Use this for initialization
    private void initDiagonals()
    {
        //Diagonals
        var diagonal = new GameObject();
        diagonal.gameObject.name = "Diagonal1";
        diagonal.tag = "ClaimGame";
        diagonal.transform.Rotate(new Vector3(90, 0, -135));
        diagonal.transform.localScale = new Vector3(.2f, .2f, .2f);
        diagonal.transform.position = new Vector3(Start_X - X_OFFSET, 1.01f, Start_Y - 1);
        diagonal.transform.parent = this.gameObject.transform;
        var drender = diagonal.AddComponent<SpriteRenderer>();
        drender.sprite = icon;
        diagonal.AddComponent<cakeslice.Outline>();
        var dcol = diagonal.AddComponent<BoxCollider>();
        dcol.size = new Vector3(10, 8f, 6f);
        dcol.isTrigger = true;
        diagonal.SetActive(false);
        Diagonals[0] = diagonal;


        diagonal = new GameObject();
        diagonal.gameObject.name = "Diagonal2";
        diagonal.tag = "ClaimGame";
        diagonal.transform.Rotate(new Vector3(90, 0, 135));
        diagonal.transform.localScale = new Vector3(.2f, .2f, .2f);
        diagonal.transform.position = new Vector3(Start_X - .5f + (X_OFFSET * (board.GetBoardDimensions())), 1.01f, Start_Y - 1);
        diagonal.transform.parent = this.gameObject.transform;
        drender = diagonal.AddComponent<SpriteRenderer>();
        drender.sprite = icon;
        diagonal.AddComponent<cakeslice.Outline>();
        dcol = diagonal.AddComponent<BoxCollider>();
        dcol.size = new Vector3(10, 8f, 6f);
        dcol.isTrigger = true;
        diagonal.SetActive(false);

        Diagonals[1] = diagonal;
    }


    public void EnableClaims(IGame model)
    {
        for (int x = 0; x < HorizontalButtons.Length; x++)
        {
            if (model.isFullColumn(x))
            {
                HorizontalButtons[x].SetActive(true);
            }
        }

        for (int y = 0; y < VerticalButtons.Length; y++)
        {
             if (model.isFullRow(y))
            {
                VerticalButtons[y].SetActive(true);
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
        foreach (var elem in HorizontalButtons)
        {
            elem.SetActive(false);
        }
        foreach (var elem in VerticalButtons)
        {
            elem.SetActive(false);
        }
        foreach (var elem in Diagonals)
        {
            elem.SetActive(false);
        }
    }
}
