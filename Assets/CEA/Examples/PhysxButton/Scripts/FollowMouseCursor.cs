using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseCursor : MonoBehaviour {

  public float distanceFromCamera = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    Vector3 pos = Input.mousePosition;
    pos.z = distanceFromCamera;
    pos = Camera.main.ScreenToWorldPoint(pos);
    transform.position = pos;
  }
}
