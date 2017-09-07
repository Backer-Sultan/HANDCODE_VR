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
            armRig.MoveArmsLeft();

        if (Input.GetKeyUp(KeyCode.W))
            armRig.MoveArmsRight();

        if(Input.GetKeyDown(KeyCode.E))
            armRig.StopArms();

        if (Input.GetKeyDown(KeyCode.R))
            armRig.OpenArms();

        if (Input.GetKeyDown(KeyCode.T))
            armRig.CloseArms();

        if (Input.GetKeyDown(KeyCode.Y))
            armRig.StopArms();

        if (Input.GetKeyDown(KeyCode.U))
            armRig.RotateUp();

        if (Input.GetKeyDown(KeyCode.I))
            armRig.RotateDown();

        if (Input.GetKeyDown(KeyCode.O))
            armRig.StopRotating();
    }
}
    