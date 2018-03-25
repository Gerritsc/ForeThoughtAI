using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLBrain {

	struct SkyNetMove {
		float visitCount;
		float winCount;


		SkyNetMove(float vc, float wc){
			visitCount = vc;
			winCount = wc;
		}
	};

	Dictionary<string[], List<SkyNetMove>> memoryBank;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
