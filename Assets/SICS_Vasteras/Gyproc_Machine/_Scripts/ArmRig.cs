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
    


    /* methods & coroutines */

    private void Start()
    {
        if (ID == string.Empty)
            Debug.LogError("ArmRig.cs: ID can't be empty!");
        else
        {
            ID = ID.Trim().ToLower();
            if (ID != "left" && ID != "right")
                Debug.LogError("ArmRig.cs: Invalid ID! ID should be `left` or `right`!");
        }
    }
}
