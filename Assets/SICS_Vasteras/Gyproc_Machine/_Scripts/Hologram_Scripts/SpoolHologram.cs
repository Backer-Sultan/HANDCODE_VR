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
            transform.Translate(Vector3.back * speed * Time.deltaTime);

            if (transform.localPosition.z <= 0f)
                ResetPosition();
        }

        private void FixedUpdate()
        {
        if (transform.localPosition == Vector3.zero)
            {
                ResetPosition();
            }
        }

        private void OnEnable()
        {
            ResetPosition();
        }
    }
}