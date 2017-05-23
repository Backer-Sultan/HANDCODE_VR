/*********************************************                                 *
 * Project: HANDCODE                         *
 * Author: Backer Sultan                     *
 * Email:  backer.sultan@ri.se               *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRig : MonoBehaviour
{
    /* fields & properties */

    public string ID; // use 'left' for the left ArmRig and 'right' for the right one. 
    public Transform mainHandle;
    public Arm arm_Left, arm_Right;
    public ArmConsole armConsole;
    
    private AudioSource audioSource;
    private HingeJoint joint;

    /* methods & coroutines */

    private void Start()
    {
        // initialization
        if (ID == string.Empty)
            Debug.LogError("ArmRig.cs: ID can't be empty!");
        else
        {
            ID = ID.Trim().ToLower();
            if (ID != "left" && ID != "right")
                Debug.LogError("ArmRig.cs: Invalid ID! ID should be `left` or `right`!");
        }

        if (!mainHandle)
            mainHandle = transform.Find("MainHandle");
        if (!mainHandle)
            Debug.LogError(string.Format("ArmRig.cs ({0}): object MainHandle is missing!", ID));

        if (!arm_Left)
            arm_Left = transform.Find("MainHandle/Arm_Left").GetComponent<Arm>();
        if (!arm_Left)
            Debug.LogError(string.Format("ArmRig.cs ({0}): Object MainHandle/Arm_Left is missing!", ID));

        if (!arm_Right)
            arm_Right = transform.Find("MainHandle/Arm_Right").GetComponent<Arm>();
        if (!arm_Right)
            Debug.LogError(string.Format("ArmRig.cs ({0}): Object MainHandle/Arm_Right is missing!", ID));

        //if (!armConsole)
        //    armConsole = transform.Find("MainHandle/Arm_Left/ArmConsole").GetComponent<ArmConsole>();
        //if (!armConsole)
        //    Debug.LogError(string.Format("ArmRig.cs ({0}): Object MainHandle/Arm_Left/ArmConsole is missing!", ID));

        audioSource = GetComponentInChildren<AudioSource>();
        if(!audioSource)
            Debug.LogError(string.Format("ArmRig.cs ({0}): AudioSource component is missing!", ID));

        joint = mainHandle.GetComponent<HingeJoint>();
        if (!joint)
            Debug.LogError(string.Format("ArmRig.cs({0}): Component HingeJoint is missing on Object `ArmRig/MainHandle`!", ID));
    }

    public void RotateUp()
    {
        JointMotor motor = joint.motor;
        motor.targetVelocity = 10f;
        joint.motor = motor;
        PlaySound(MachineSounds.Instance.ArmRig_Rotating);
    }

    public void RotateDown()
    {
        JointMotor motor = joint.motor;
        motor.targetVelocity = -10f;
        joint.motor = motor;
        PlaySound(MachineSounds.Instance.ArmRig_Rotating);
    }

    public void StopRotating()
    {
        JointMotor motor = joint.motor;
        motor.targetVelocity = 0f;
        joint.motor = motor;
        PlaySound(MachineSounds.Instance.ArmRig_StopRotating);
    }

    public void OpenArms()
    {
        arm_Left.MoveLeft();
        arm_Right.MoveRight();
    }

    public void CloseArms()
    {
        arm_Left.MoveRight();
        arm_Right.MoveLeft();
    }

    public void MoveArmsLeft()
    {
        arm_Left.MoveLeft();
        arm_Right.MoveLeft();
    }

    public void MoveArmsRight()
    {
        arm_Left.MoveRight();
        arm_Right.MoveRight();
    }
 
    public void StopArms()
    {
        arm_Left.Stop();
        arm_Right.Stop();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}


