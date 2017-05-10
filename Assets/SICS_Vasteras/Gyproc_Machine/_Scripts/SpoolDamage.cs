/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoolDamage : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ArmCone")
            SendMessageUpwards("ApplyDamage", SendMessageOptions.RequireReceiver);
    }
}
