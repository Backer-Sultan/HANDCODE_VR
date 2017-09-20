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
        public Identifier ID;

        public ArmHologram right_arm_hologram;
        public ArmHologram left_arm_hologram;

        internal override void Start()
        {
            base.Start();
            AdjustArmsSpeed(); 
        }
       
        private void AdjustArmsSpeed()
        {
            right_arm_hologram.initialMoveSpeed = moveSpeed;
            left_arm_hologram.initialMoveSpeed = moveSpeed;
            right_arm_hologram.initialRotateSpeed = rotateSpeed;
            left_arm_hologram.initialRotateSpeed = rotateSpeed;
        }

        public void OpenArmsHologram()
        {
            gameObject.SetActive(true);
            right_arm_hologram.moveDirection = Identifier.RIGHT;
            left_arm_hologram.moveDirection = Identifier.LEFT;
        }

        public void CloseArmsHologram()
        {
            gameObject.SetActive(true);
            right_arm_hologram.moveDirection = Identifier.LEFT;
            left_arm_hologram.moveDirection = Identifier.RIGHT;
        }

        public void RaiseArmsHologram()
        {
            gameObject.SetActive(true);
            right_arm_hologram.rotationDirection = Identifier.UP;
            left_arm_hologram.rotationDirection = Identifier.UP;
        }

        public void LowerArmsHologram()
        {
            gameObject.SetActive(true);
            right_arm_hologram.rotationDirection = Identifier.DOWN;
            left_arm_hologram.rotationDirection = Identifier.DOWN;
        }

        public void HideHologram()
        {
            gameObject.SetActive(false);
        }
    }
}
