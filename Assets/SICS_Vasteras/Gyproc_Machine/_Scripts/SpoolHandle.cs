/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/
 
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoolHandle : MonoBehaviour
{
    private Spool spool;

    private void Start()
    {
        spool = GetComponentInParent<Spool>();
        if (!spool)
            Debug.LogError("SpoolHandle.cs: Spool.cs is missing in the parent object!");
    }
    private void OnTriggerEnter(Collider other)
    {
        //print("Spoolhandle.cs: " + other.name);
        if (other.name == "SpoolCollisionTrigger_Cone_Left" && !spool.isDamaged && !spool.isHandled)
        {                 
            other.GetComponentInParent<Arm>().speed = 0f;
            spool.transform.parent = other.gameObject.transform;
            spool.isLeftSideHandled = true;
        }
        if (other.name == "SpoolCollisionTrigger_Cone_Right" && !spool.isDamaged && !spool.isHandled)
        {
            other.GetComponentInParent<Arm>().speed = 0f;
            spool.isRightSideHandled = true;
        }
        if (spool.isLeftSideHandled && spool.isRightSideHandled)
        {
            spool.Handle();
        }
    }
}
