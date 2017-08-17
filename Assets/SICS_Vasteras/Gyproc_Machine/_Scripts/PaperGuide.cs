/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class PaperGuide : MonoBehaviour
    {
        /* fields & properties */
        [Range(0.1f, 3f)]
        public float rotationSpeed = 1f;

        private bool isRotating = false;
        private bool clockWise;
        private float angle;



        /* methods & coroutines */

        public void RotateToRight()
        {
            clockWise = true;
            isRotating = true;
        }

        public void RotateToLeft()
        {
            clockWise = false;
            isRotating = true;
        }

        public void StopRotating()
        {
            isRotating = false;
        }

        private void FixedUpdate ()
        {
            if (isRotating)
            {
                angle = transform.localEulerAngles.y;
                angle = (angle > 180) ? angle - 360 : angle;
                if (clockWise)
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                else
                    transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);

                // setting rotation boundaries
                if (angle <= -4.3f || angle >= 4.3f)
                    isRotating = false;
            }
        }
    } 
}
