/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

 using UnityEngine;

namespace HandCode
{
    public class ArmsWithSpoolHologram : Hologram
    {
        /* fields & properties */

        public ArmRigHologram armRigHologram;
        public ArmHologram armLeftHologram;
        public ArmHologram armRightHologram;
        public SpoolHologram spoolHologram;

        /* methods & coroutines */

        protected override void OnEnable()
        {
            base.OnEnable();

            SetHologramsStructure();
            spoolHologram.gameObject.SetActive(true);
            armRigHologram.gameObject.SetActive(true);
            RotateUp();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            spoolHologram.gameObject.SetActive(false);
            armRigHologram.gameObject.SetActive(false);
        }

        private void SetHologramsStructure()
        {
            spoolHologram.transform.position = Vector3.zero;
            spoolHologram.transform.SetParent(armLeftHologram.transform);
            spoolHologram.moveSpeed = 0f;
            armLeftHologram.move = false;
            armLeftHologram.rotate = false;
            armRightHologram.move = false;
            armRightHologram.rotate = false;
        }

        public void RotateDown()
        {
            armLeftHologram.rotationDirection = Identifier.DOWN;
            armLeftHologram.rotate = true;
            armRightHologram.rotationDirection = Identifier.DOWN;
            armRightHologram.rotate = true;

        }

        public void RotateUp()
        {
            armLeftHologram.rotationDirection = Identifier.UP;
            armLeftHologram.rotate = true;
            armRightHologram.rotationDirection = Identifier.UP;
            armRightHologram.rotate = true;
        }

        public void StopRotating()
        {
            armLeftHologram.rotate = false;
            armRightHologram.rotate = false;
        }
    }

}