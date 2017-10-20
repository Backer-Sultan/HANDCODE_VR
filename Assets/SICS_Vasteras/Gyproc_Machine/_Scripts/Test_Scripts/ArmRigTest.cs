using UnityEngine;
using HandCode;

public class ArmRigTest : MonoBehaviour
{
    ArmRig armRig;

    void Start ()
    {
        armRig = GameObject.Find("ArmRig_Right").GetComponent<ArmRig>();
        if (!armRig)
            Debug.LogError("Can't find ArmRig.cs!");
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            armRig.OpenArms();

        if (Input.GetKeyDown(KeyCode.W))
            armRig.CloseArms();

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.W))
            armRig.StopArms();

        if (Input.GetKeyDown(KeyCode.E))
            armRig.RotateUp();

        if (Input.GetKeyDown(KeyCode.R))
            armRig.RotateDown();

        if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.R))
            armRig.StopRotating();

        if (Input.GetKeyDown(KeyCode.T))
            armRig.MoveArmsRight();

        if (Input.GetKeyDown(KeyCode.Y))
            armRig.MoveArmsLeft();

        if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Y))
            armRig.StopArms();

    }
}
    