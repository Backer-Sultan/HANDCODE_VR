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
        /* fields & properties */
        public Identifier ID;
        [Range(0f, 1f)]
        public float rotationSpeed = 0.5f;
        public bool move;
        public bool rotate;
        public Identifier moveDirection;
        public Identifier rotationDirection;
        public ArmHologram otherArm;

        private Machine machine;



        /* methods & coroutines */

        internal override void Start()
        {
            base.Start();

            machine = GameObject.FindObjectOfType<Machine>();
        }

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
                {
                    axis = Vector3.forward; // +z rotation
                }
                else if (rotationDirection == Identifier.Down)
                {
                    axis = Vector3.back; // -z rotation
                }

                float upLimit = machine.armRig_Right.joint.limits.max;
                float downLimit = machine.armRig_Right.joint.limits.min;

                if (GetSignedRotation(transform.localEulerAngles.z) < upLimit &&
                    GetSignedRotation(transform.localEulerAngles.z) > downLimit)
                {
                    transform.Rotate(axis, rotationSpeed);
                }
                else
                {
                    ResetRotation();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ID == Identifier.LEFT && other.tag == "ArmLimitLeft" ||
                ID == Identifier.RIGHT && other.tag == "ArmLimitRight")
            {
                ResetPosition();
                otherArm.ResetPosition();
            }
        }

        private void OnEnable()
        {
            ResetPosition();
            ResetRotation();
        }

        private float GetSignedRotation(float angle)
        {
            float signedAngle = (angle > 180f) ? angle - 360f : angle;
            return signedAngle;
        }
    }
}