using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRender : MonoBehaviour {

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

	GameObject[] cards;

	List<ICard> hand;

    private void OnEnable()
    {
        FindObjectOfType<ModelManager>().OnBoardChange += UpdateHand;
    }

    private void OnDisable()
    {
        FindObjectOfType<ModelManager>().OnBoardChange -= UpdateHand;
    }

    public void InitHand () {
		hand = FindObjectOfType<ModelManager> ().gameModel.getHand ();
        cards = new GameObject[5];
		var cardtosprite = FindObjectOfType<CardToSprite>();
		for (int i = 0; i < 5; i++) {
            var obj = new GameObject();
            obj.gameObject.name = i.ToString();
			obj.transform.Rotate(new Vector3(90, 0, 0));
			obj.transform.localScale = new Vector3(2, 2, 2);
			obj.transform.position = new Vector3(startX + (i * X_OFFSET), cardHeight, startY);
            obj.transform.parent = this.gameObject.transform;
			obj.AddComponent<SpriteRenderer>();
			obj.AddComponent<cakeslice.Outline>();
			var col = obj.AddComponent<BoxCollider>();
			col.size = new Vector3(1, 1f, .6f);
			col.isTrigger = true;
            var hcs = obj.AddComponent<HandCardStruct>();
            hcs.card = hand[i];
            hcs.pos = i;


			obj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
			obj.GetComponent<SpriteRenderer>().sprite = cardtosprite.getSprite (this.hand [i]);
            obj.tag = "Hand";

			cards [i] = obj;
		}
	}

	void UpdateHand(IGame model) {
		hand = model.getHand ();
		renderHand ();
        for (int i = 0; i < 5; i++) {
            cards[i].GetComponent<HandCardStruct>().card = model.getHand()[i];
        }
 	}

	void renderHand () {
		var cardtosprite = FindObjectOfType<CardToSprite>();
		for (int i = 0; i < 5; i++)
		{ 
			cards[i].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
			cards[i].GetComponent<SpriteRenderer>().sprite = cardtosprite.getSprite(this.hand[i]);
		}
	}
}
