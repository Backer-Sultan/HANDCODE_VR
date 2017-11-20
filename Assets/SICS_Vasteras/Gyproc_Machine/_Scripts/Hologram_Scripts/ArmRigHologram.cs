/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class ArmRigHologram : Hologram
    {
        public Transform rightArmHologram;
        public Transform leftArmHologram;
        public Transform spoolHologram;
        public Transform rightArm_init_pos, rightArm_dest_pos;
        public Transform leftArm_init_pos, leftArm_dest_pos;
        public Transform init_rotation, dest_rotation; // I'm ignoring the base calass initial value as I want to manually set and manipulate it
        public ShowHologramFor showHologramFor;

        public bool isArmsInDestPosition;
        public bool isArmsInDestRotation;

        public enum ShowHologramFor
        {
            NONE,
            OPEN_CLOSE_ARMS,
            RAISE_LOWER_ARMS,
            RAISE_LOWER_ARMS_WITH_SPOOL,
        }

        private float initRotationValue;
        private float destRotationValue;

        private void Start()
        {
            initRotationValue = GetSignedRotation(init_rotation.localEulerAngles.z);
            destRotationValue = GetSignedRotation(dest_rotation.localEulerAngles.z);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (showHologramFor == ShowHologramFor.RAISE_LOWER_ARMS_WITH_SPOOL)
                spoolHologram.gameObject.SetActive(true);
            else
                spoolHologram.gameObject.SetActive(false);

            switch (showHologramFor)
            {
                case ShowHologramFor.OPEN_CLOSE_ARMS:
                    rightArmHologram.localPosition = rightArm_init_pos.localPosition;
                    leftArmHologram.localPosition = leftArm_init_pos.localPosition;
                    break;

                case ShowHologramFor.RAISE_LOWER_ARMS:
                case ShowHologramFor.RAISE_LOWER_ARMS_WITH_SPOOL:
                    leftArmHologram.localEulerAngles = rightArmHologram.localEulerAngles = init_rotation.localEulerAngles;
                    break;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            ResetArmsPosition();
        }

        protected override void Update()
        {
            base.Update();

            // setting flags
            switch (showHologramFor)
            {
                case ShowHologramFor.OPEN_CLOSE_ARMS:
                    if (waitingFlag)
                        break;

                    if (isArmsInDestPosition)
                    {
                        Invoke("ResetArmsPosition", waitTime);
                        waitingFlag = true;
                    }
                    else
                        MoveArmsTowardsDestination();

                    break;

                case ShowHologramFor.RAISE_LOWER_ARMS:
                case ShowHologramFor.RAISE_LOWER_ARMS_WITH_SPOOL:
                    if (waitingFlag)
                        break;

                    if (isArmsInDestRotation)
                    {
                        Invoke("ResetArmsRotation", waitTime);
                        waitingFlag = true;
                    }
                    else
                        RotateArmsTowardsDestination();

                    break;
            }
        }

        private void ResetArmsPosition()
        {
            rightArmHologram.localPosition = rightArm_init_pos.localPosition;
            leftArmHologram.localPosition = leftArm_init_pos.localPosition;
            movementSpeed = initialMovementSpeed;
            isArmsInDestPosition = false;
            waitingFlag = false;
        }

        private void MoveArmsTowardsDestination()
        {
            if (leftArmHologram.localPosition == leftArm_dest_pos.localPosition || rightArmHologram.localPosition == rightArm_dest_pos.localPosition)
            {
                isArmsInDestPosition = true;
                return;
            }
            leftArmHologram.localPosition = Vector3.MoveTowards(leftArmHologram.localPosition, leftArm_dest_pos.localPosition, movementSpeed * Time.deltaTime);
            rightArmHologram.localPosition = Vector3.MoveTowards(rightArmHologram.localPosition, rightArm_dest_pos.localPosition, movementSpeed * Time.deltaTime);
        }

        private void ResetArmsRotation()
        {
            leftArmHologram.localEulerAngles = init_rotation.localEulerAngles;
            rightArmHologram.localEulerAngles = init_rotation.localEulerAngles;
            rotationSpeed = initialRotationSpeed;
            isArmsInDestRotation = false;
            waitingFlag = false;
        }

        private void RotateArmsTowardsDestination()
        {
            float currentArmRotationValue = GetSignedRotation(leftArmHologram.localEulerAngles.z);
            bool dirUp = (destRotationValue > initRotationValue) ? true : false;

            if (dirUp)
            {
                if(currentArmRotationValue >= destRotationValue)
                {
                    isArmsInDestRotation = true;
                    return;
                }
                leftArmHologram.localEulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;
                rightArmHologram.localEulerAngles = leftArmHologram.localEulerAngles;
            }
            else
            {
                if(currentArmRotationValue <= destRotationValue)
                {
                    isArmsInDestRotation = true;
                    return;
                }
                leftArmHologram.localEulerAngles += Vector3.back * rotationSpeed * Time.deltaTime;
                rightArmHologram.localEulerAngles = leftArmHologram.localEulerAngles;
            }
        }
    }
}
