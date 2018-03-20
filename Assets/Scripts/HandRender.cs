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

	void Awake () {
		hand = FindObjectOfType<ModelManager> ().gameModel.getHand ();
		var cardtosprite = FindObjectOfType<CardToSprite>();
		var obj = new GameObject();
		for (int i = 0; i < 5; i++) {
			obj.gameObject.name = i.ToString();
			obj.transform.Rotate(new Vector3(90, 0, 0));
			obj.transform.localScale = new Vector3(2, 2, 2);
			obj.transform.position = new Vector3(startX + (i * X_OFFSET), cardHeight, startY);
			obj.AddComponent<SpriteRenderer>();
			obj.AddComponent<cakeslice.Outline>();
			var col = obj.AddComponent<BoxCollider>();
			col.size = new Vector3(1, 1f, .6f);
			col.isTrigger = true;

			obj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
			obj.GetComponent<SpriteRenderer>().sprite = cardtosprite.getSprite (this.hand [i]);

			cards [i] = obj;
		}
	}

	void UpdateHand(IGame model) {
		hand = model.getHand ();
		renderHand ();
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
