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
			if (pressure > 3)
				pressure = 3;
		}
		else
		{
			pressure -= 2*pressureSpeed;
			if (pressure < 0)
				pressure = 0;

		}

    pos.z = distanceFromCamera+ pressure;
    pos = Camera.main.ScreenToWorldPoint(pos);
    transform.position = pos;
  }
}
