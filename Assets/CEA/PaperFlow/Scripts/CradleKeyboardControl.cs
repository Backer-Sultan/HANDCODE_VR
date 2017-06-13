using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CradleKeyboardControl : MonoBehaviour {

	public Cradle cradle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.RightArrow))
			cradle.MoveToRight();

		if (Input.GetKey(KeyCode.LeftArrow))
			cradle.MoveToLeft();
	}
}
