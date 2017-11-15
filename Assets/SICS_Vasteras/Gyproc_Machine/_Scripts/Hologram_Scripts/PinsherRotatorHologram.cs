/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class PinsherRotatorHologram : Hologram
    {
        public float initRotationValue;
        public float destRotationValue;
        public ShowHologramFor showHologramFor;

        public enum ShowHologramFor
        {
            NONE,
            RAISE_PINHSER,
            LOWER_PINSHER,
        }

        private bool isPinsherInDestRotation;

        /* methods & coroutines */

        protected override void OnEnable()
        {
            base.OnEnable();
            transform.localEulerAngles = new Vector3(0f, 0f, initRotationValue);
        }

        protected override void Update()
        {
            base.Update();
            if (waitingFlag)
                return;

            if (waitingFlag)
                return;
            if (isPinsherInDestRotation)
            {
                Invoke("ResetPinsherRotation", waitTime);
                waitingFlag = true;
            }
            else
                RotateTowardsDestination();
        }

        private void ResetPinsherRotation()
        {
            transform.localEulerAngles = new Vector3(0f, 0f, initRotationValue);
            rotationSpeed = initialRotationSpeed; 
            isPinsherInDestRotation = false;
            waitingFlag = false;
        }

        private void RotateTowardsDestination()
        {
            float currentRotationValue = GetSignedRotation(transform.localEulerAngles.z);
            bool dirUp = (destRotationValue >= initRotationValue) ? true : false;

            if(dirUp)
            {
                if(currentRotationValue >= destRotationValue)
                {
                    isPinsherInDestRotation = true;
                    return;
                }
                transform.localEulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;
            }
            else
            {
                if(currentRotationValue <= destRotationValue)
                {
                    isPinsherInDestRotation = true;
                    return;
                }
                transform.localEulerAngles += Vector3.back * rotationSpeed * Time.deltaTime;
            }
        }
    }
}