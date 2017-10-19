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
        /* fields & properties */

        public Identifier rotationDirection;
        public float upLimit; // in Eular angle
        public float downLimit; // in Eular angle



        /* methods & coroutines */

        protected override void Update()
        {
            base.Update();

            Vector3 axis = Vector3.zero;

            if(rotationDirection == Identifier.UP)
            {
                axis = Vector3.forward; // +z rotation
            }
            else if(rotationDirection == Identifier.DOWN)
            {
                axis = Vector3.back; // -z rotation
            }
            else
            {
                Debug.LogError("Invalid direction!");
            }

            if(GetSignedRotation(transform.localEulerAngles.z) <= upLimit &&
               GetSignedRotation(transform.localEulerAngles.z) >= downLimit)
            {
                transform.Rotate(axis, rotateSpeed);
            }
            else
            {
                ResetRotationAfter(waitTime);
            }
        }
    } 
}
