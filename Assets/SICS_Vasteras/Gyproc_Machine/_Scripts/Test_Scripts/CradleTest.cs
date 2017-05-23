using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CradleTest : MonoBehaviour {
    Cradle cradle;
	// Use this for initialization
	void Start () {
        cradle = GameObject.Find("Cradle").GetComponent<Cradle>();
        if (!cradle)
            Debug.LogError("Can't find Cradle.cs!");

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            cradle.MoveToLeft();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cradle.MoveToRight();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cradle.Stop();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            cradle.LowerPinsher();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            cradle.RaisePinsher();
        }
    }
}
