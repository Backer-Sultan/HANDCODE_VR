/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/
 
using UnityEngine;

namespace HandCode
{
    public class SpoolHandle : MonoBehaviour
    {
        /* fields & properties */

        private Spool spool;



        /* methods & coroutines */

        private void Start()
        {
            spool = GetComponentInParent<Spool>();
            if (spool == null)
                Debug.LogError(string.Format("{0}\nSpoolHandle.cs: Script `Spool.cs` is missing in the parent object!", Machine.GetPath(gameObject)));
        }

        private void OnTriggerEnter(Collider other)
        {
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
}
