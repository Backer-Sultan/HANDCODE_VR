using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandCode;

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

    if (Input.GetKey(KeyCode.UpArrow))
      cradle.RaisePinsher();

    if (Input.GetKey(KeyCode.DownArrow))
      cradle.LowerPinsher();
  }
}
