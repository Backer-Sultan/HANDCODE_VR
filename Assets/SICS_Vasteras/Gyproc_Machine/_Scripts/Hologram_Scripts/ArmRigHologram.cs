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
        public GameObject staticSpoolHologram;
        public bool enableSpoolHologram;


        internal override void Start()
        {
            base.Start();
            GetInitialSpeedValues();
        }

        private void GetInitialSpeedValues()
        {
            right_arm_hologram.initialMoveSpeed = moveSpeed;
            left_arm_hologram.initialMoveSpeed = moveSpeed;
            right_arm_hologram.initialRotateSpeed = rotateSpeed;
            left_arm_hologram.initialRotateSpeed = rotateSpeed;
        }

        public void OpenArmsHologram()
        {
            right_arm_hologram.moveDirection = Identifier.RIGHT;
            left_arm_hologram.moveDirection = Identifier.LEFT;
            SetArmsMove(true);
            SetArmsActive(true);
        }

        public void CloseArmsHologram()
        {
            right_arm_hologram.moveDirection = Identifier.LEFT;
            left_arm_hologram.moveDirection = Identifier.RIGHT;
            SetArmsMove(true);
            SetArmsActive(true);
        }

        public void RaiseArmsHologram()
        {
            right_arm_hologram.rotationDirection = Identifier.UP;
            left_arm_hologram.rotationDirection = Identifier.UP;
            SetArmsRotate(true);
            SetArmsActive(true);
        }

        public void LowerArmsHologram()
        {
            right_arm_hologram.rotationDirection = Identifier.DOWN;
            left_arm_hologram.rotationDirection = Identifier.DOWN;
            SetArmsRotate(true);
            SetArmsActive(true);
        }

        public void HideHologram()
        {
            SetArmsMove(false);
            SetArmsRotate(false);
            SetArmsActive(false);
        }

        private void SetArmsMove(bool value)
        {
            right_arm_hologram.move = value;
            left_arm_hologram.move = value;
        }

        private void SetArmsRotate(bool value)
        {
            right_arm_hologram.rotate = value;
            left_arm_hologram.rotate = value;
        }

        private void SetArmsActive(bool value)
        {
            right_arm_hologram.gameObject.SetActive(value);
            left_arm_hologram.gameObject.SetActive(value);
        }

        private void SetStaticSpoolActive(bool value)
        {
            staticSpoolHologram.SetActive(value);
        }

        internal override void Update()
        {
            base.Update();

            SetStaticSpoolActive(enableSpoolHologram);

            // test code:
            if (Input.GetKeyDown(KeyCode.U))
                RaiseArmsHologram();

            if (Input.GetKeyDown(KeyCode.D))
                LowerArmsHologram();

            if (Input.GetKeyDown(KeyCode.O))
                OpenArmsHologram();

            if (Input.GetKeyDown(KeyCode.C))
                CloseArmsHologram();

            if (Input.GetKeyDown(KeyCode.S))
                enableSpoolHologram = !enableSpoolHologram;

            if (Input.GetKeyDown(KeyCode.H))
                HideHologram();
        }
    }
}
