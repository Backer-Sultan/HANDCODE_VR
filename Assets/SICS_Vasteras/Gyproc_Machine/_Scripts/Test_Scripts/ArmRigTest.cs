using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRigTest : MonoBehaviour {
    ArmRig armRig;
	// Use this for initialization
	void Start () {
        armRig = GameObject.Find("ArmRig_Right").GetComponent<ArmRig>();
        if (!armRig)
            Debug.LogError("Can't find ArmRig.cs!");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Alpha1))
        {
            print("armRig.RotateRigUp()");
            armRig.RotateRigUp();
        }
	}
}
