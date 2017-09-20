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

        internal override void Update()
        {
            base.Update();
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

            if (transform.localPosition.z <= 0f)
                ResetPositionAfter(waitTime);
        }

        private void FixedUpdate()
        {
        if (transform.localPosition == Vector3.zero)
            {
                ResetPositionAfter(waitTime);
            }
        }
    }
}