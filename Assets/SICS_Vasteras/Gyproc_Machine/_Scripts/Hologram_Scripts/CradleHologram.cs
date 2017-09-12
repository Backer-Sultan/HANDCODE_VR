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

        internal override void Update()
        {
            base.Update();
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "CradleLimitRight")
                transform.localPosition = obj.transform.localPosition;
        }

        private void OnEnable()
        {
            if (obj != null)
                transform.localPosition = obj.transform.localPosition;
        }
    }
}