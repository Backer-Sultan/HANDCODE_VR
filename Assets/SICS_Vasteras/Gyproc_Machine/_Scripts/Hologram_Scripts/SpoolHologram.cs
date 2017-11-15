/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class SpoolHologram : Hologram
    {

        /* methods & coroutines */

        protected override void Update()
        {
            base.Update();
            if (waitingFlag)
                return;

            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            if (transform.localPosition.z >= 0f)
            {
                movementSpeed = 0f;
                Invoke("ResetPosition", waitTime);
                waitingFlag = true;
            }
        }
    }
}