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
		

        if (Input.GetKeyDown(KeyCode.Alpha4))
            armRig.OpenArms();
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
            armRig.CloseArms();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            armRig.StopArms();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            armRig.RotateUp();
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
            armRig.RotateDown();

        if (Input.GetKeyDown(KeyCode.Alpha9))
            armRig.StopRotating();
    }
}
    