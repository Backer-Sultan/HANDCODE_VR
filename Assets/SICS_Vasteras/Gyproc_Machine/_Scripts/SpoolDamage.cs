/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class SpoolDamage : MonoBehaviour
    {
        /* fields & properties */

        public Renderer renderer;


        /* methods & coroutines */

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "ArmCone")
            {
                SendMessageUpwards("ApplyDamage", SendMessageOptions.RequireReceiver);
                renderer.material.color = Color.red;
            }
        }
    } 
}
