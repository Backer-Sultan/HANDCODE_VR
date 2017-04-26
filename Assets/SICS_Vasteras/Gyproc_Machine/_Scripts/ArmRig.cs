/*********************************************
 * Author: Backer Sultan                     *
 * Email:  backer.sultan@ri.se               *
 * Created: 25-04-2017                       *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRig : MonoBehaviour
{
    /* fields & properties */

    public string ID; // use 'left' for the left ArmRig and 'right' for the right one.
    public float rotationSpeed = 1f;
    public float armsSpeed = 0.3f;
    public float rotationMaxUp = 0.2f;
    public float rotationMaxDown = 0.1f;
    public float rotationAcceptedUp = 0.5f;
    //[HideInInspector]
    public float rotation = 0f;
    public float armsDistance = 0f;
    public float warningDistance = 0.18f;
    public float damageDistance = 0.078f;
    public float armsOffsetRange = 0.01f;
    public float armsOffset = 0f;
    public bool isArmsMid = true; // old name: armsOffsetInAcceptedRange
    //[HideInInspector]
    public Transform arm_Left, arm_Right, mainHandle;
    //[HideInInspector]
    public Hologram hologram_Arm_Left, hologram_Arm_Left_Closed, hologram_Arm_Left_Up, hologram_Arm_Right, hologram_Arm_Right_Closed,   hologram_Arm_Right_Up, hologram_Arms_Spool_Up;
    public ArmConsole armConsole;
    //[HideInInspector]
    public AudioSource audioSource;

    private float leftLimit_Arm_Left = -0.43f; // is the left range limit for the left arm
    private float rightLimit_Arm_Left = 0.40f; // is the right range limit for the left arm
    private float leftLimit_Arm_Right = -0.43f; // is the left range limit for the right arm
    private float rightLimit_Arm_Right = 0.40f; // is the right range limit for the right arm


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
        if (!arm_Left)
            arm_Left = transform.Find("MainHandle/Arm_Left");
        if (!arm_Left)
            Debug.LogError(string.Format("ArmRig.cs ({0}): Object MainHandle/Arm_Left is missing!", ID));

        if (!arm_Right)
            arm_Right = transform.Find("MainHandle/Arm_Right");
        if(!arm_Right)
            Debug.LogError(string.Format("ArmRig.cs ({0}): Object MainHandle/Arm_Right is missing!", ID));

        /* initialization for hologram objects goes here:
         * ...
         */

        // setting arms offset (basically for ArmRig_Left in position 4 processing)
        armsOffset = arm_Left.localPosition.z;
    }

    public void RotateRigUp()
    {
        if (mainHandle.localRotation.z < rotationMaxUp)
            mainHandle.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        else
        rotation = mainHandle.localRotation.z;
    }


}


