using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class something : MonoBehaviour
{

	
	
	void Update ()
    {
        if (transform.position.y <= -2f)
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }


  
}


