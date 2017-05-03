using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRigTest : MonoBehaviour {
    ArmRig armRig;

    void Start ()
    {
        armRig = GameObject.Find("ArmRig_Right").GetComponent<ArmRig>();
        if (!armRig)
            Debug.LogError("Can't find ArmRig.cs!");
	}
	
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            armRig.MoveArmsLeft();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
            armRig.StopArms();

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            armRig.MoveArmsRight();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            armRig.StopArms();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            armRig.OpenArms();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            armRig.CloseArms();
        }


        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            armRig.RotateUp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
            armRig.RotateDown();
        if (Input.GetKeyDown(KeyCode.Alpha8))
            armRig.StopRotating();
    }
}
    