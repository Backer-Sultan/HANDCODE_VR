/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSounds : Singelton<MachineSounds>
{
    /* fields & properties */

    [Header("Cradle Sounds")]
    public AudioClip Cradle_Moving;
    public AudioClip Cradle_Stopping;
    public AudioClip Cradle_BreakToggle;

    [Space(1f)]
    [Header("ArmRig Sounds")]
    public AudioClip ArmRig_Rotating;
    public AudioClip ArmRig_StopRotating;

    [Space(1f)]
    [Header("ArmRig Sounds")]
    public AudioClip Arm_Moving;
    public AudioClip Arm_Stopping;

    /* methods & coroutines */
}
