/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class CradleHologram : Hologram
    {
        /* methods & coroutines */

        protected override void Update()
        {
            base.Update();
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "CradleLimitRight")
                ResetPositionAfter(waitTime);
        }


        
    }
}