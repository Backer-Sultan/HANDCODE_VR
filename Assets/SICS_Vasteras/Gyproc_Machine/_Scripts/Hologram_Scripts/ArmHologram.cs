/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class ArmHologram : Hologram
    {
        [Range(0f, 1f)]
        public float rotationSpeed = 0.5f;
        public bool move;
        public bool rotate;
        public Identifier moveDirection;
        public Identifier rotationDirection;


        /* methods & coroutines */

        internal override void Update()
        {
            base.Update();

            if (move)
            {
                Vector3 direction = Vector3.zero;

                if (moveDirection == Identifier.RIGHT)
                {
                    direction = Vector3.forward;
                }
                else if (moveDirection == Identifier.LEFT)
                {
                    direction = Vector3.back;
                }

                transform.Translate(direction * speed * Time.deltaTime);
            }

            if (rotate)
            {
                Vector3 axis = Vector3.zero;

                if (rotationDirection == Identifier.UP)
                    axis = Vector3.forward; // +z rotation
                else if (rotationDirection == Identifier.Down)
                    axis = Vector3.back; // -z rotation

                transform.Rotate(axis, rotationSpeed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "ArmLimitLeft")
                transform.localPosition = obj.transform.localPosition;
        }

        private void OnEnable()
        {
            if (obj != null)
                transform.localPosition = obj.transform.localPosition;
        }

    }
}