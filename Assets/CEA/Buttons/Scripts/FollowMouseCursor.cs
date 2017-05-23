using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseCursor : MonoBehaviour {

  public float distanceFromCamera = 10;

	public float pressureSpeed = 0.05f;

	private float pressure;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    Vector3 pos = Input.mousePosition;
		if(Input.GetMouseButton(0))
		{
			pressure += pressureSpeed;
		}
		else if (Input.GetMouseButton(1))
		{
			pressure -= pressureSpeed;
		}

    pos.z = distanceFromCamera + pressure;
    pos = Camera.main.ScreenToWorldPoint(pos);
    transform.position = pos;
  }
}
