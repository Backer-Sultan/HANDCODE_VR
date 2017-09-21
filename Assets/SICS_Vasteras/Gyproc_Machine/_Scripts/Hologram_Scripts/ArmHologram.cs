/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;

namespace HandCode
{
    public class ArmHologram : Hologram
    {
        /* fields & properties */
        public Identifier ID;
        public bool move;
        public bool rotate;
        public Identifier moveDirection;
        public Identifier rotationDirection;
        public ArmHologram otherArm;
        public Transform CloseStartsFrom;

        private Machine machine;


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
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }

            if (rotate)
            {
                Vector3 axis = Vector3.zero;

                if (rotationDirection == Identifier.UP)
                {
                    axis = Vector3.forward; // +z rotation
                }
                else if (rotationDirection == Identifier.DOWN)
                {
                    axis = Vector3.back; // -z rotation
                }
                else
                {
                    Debug.LogError("Invalid direction!");
                }

                float upLimit = machine.armRig_Right.joint.limits.max;
                float downLimit = machine.armRig_Right.joint.limits.min;

                if (GetSignedRotation(transform.localEulerAngles.z) < upLimit &&
                    GetSignedRotation(transform.localEulerAngles.z) > downLimit)
                {
                    transform.Rotate(axis, rotateSpeed);
                }
                else
                {
                    ResetRotationAfter(waitTime);
                }
            }
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(SetLimitForArmClose());
        }


        // setting limit for arms open
        private void OnTriggerEnter(Collider other)
        {
            if (ID == Identifier.LEFT && moveDirection == Identifier.LEFT && other.tag == "ArmLimitLeft" ||
                ID == Identifier.RIGHT && moveDirection == Identifier.RIGHT && other.tag == "ArmLimitRight")
            {
                ResetPositionAfter(waitTime);
                otherArm.ResetPositionAfter(waitTime);
            }
        }

        // setting limit for arms close
        private IEnumerator SetLimitForArmClose()
        {
            while (true)
            {
                if (ID == Identifier.LEFT && moveDirection == Identifier.RIGHT && transform.localPosition.z > initialPosition.z)
                {
                    moveSpeed = 0f;
                    otherArm.moveSpeed = 0f;
                    yield return new WaitForSeconds(waitTime);
                    transform.localPosition = CloseStartsFrom.localPosition;
                    otherArm.transform.localPosition = otherArm.CloseStartsFrom.localPosition;
                    moveSpeed = initialMoveSpeed;
                    otherArm.moveSpeed = initialMoveSpeed;
                }
                yield return null;
            }
        }
    }
}