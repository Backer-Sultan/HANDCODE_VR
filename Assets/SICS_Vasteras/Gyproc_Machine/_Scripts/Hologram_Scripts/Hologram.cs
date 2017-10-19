/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;

namespace HandCode
{
    public class Hologram : MonoBehaviour
    {
        /* fields & properties */

        [Range(0f, 1f)]
        public float moveSpeed = 0.5f;
        [Range(0f, 1f)]
        public float rotateSpeed = 0.5f;
        public float waitTime = 3f;  // time to wait before resetting the hologram to its initial position and rotation
        public Color color1 = new Color(1f, 0.5f, 0.5f, 0.2f);
        public Color color2 = new Color(1f, 0.86f, 0f, 0.2f);

        internal Vector3 initialPosition; // stored as local position
        internal Vector3 initialRotation; // stored as Eular angle
        internal float initialMoveSpeed;
        internal float initialRotateSpeed;
        protected Transform[] children;
        
        private Color lerpedColor;
        private Renderer[] rends;
        private bool resetPositionRoutineFlag;
        private bool resetRotationRoutineFlag;



        /* methods & coroutines */

        protected float GetSignedRotation(float angle)
        {
            float signedAngle = (angle > 180f) ? angle - 360f : angle;
            return signedAngle;
        }

        protected virtual void Awake()
        {
            // getting initial values for position, rotation and speed
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles;
            initialMoveSpeed = moveSpeed;
            initialRotateSpeed = rotateSpeed;

            // getting referenced to children
            rends = GetComponentsInChildren<Renderer>(true);
            children = GetComponentsInChildren<Transform>(true);
        }

        // general color lerp - make sure to call it from derived methods!
        protected virtual void Update()
        {
            lerpedColor = Color.Lerp(color1, color2, Mathf.PingPong(Time.time * 2f, 1));
            foreach (Renderer rend in rends)
                rend.material.color = lerpedColor;
        }

        protected virtual void OnEnable()
        {
            foreach (Transform t in children)
                t.gameObject.SetActive(true);
            
            ResetPosition();
            ResetRotation();
        }

        protected virtual void OnDisable()
        {
            foreach (Transform t in children)
            {
                if (t == transform)
                    continue;

                t.gameObject.SetActive(false);
            }
            StopAllCoroutines();
        }

        protected virtual void ResetPosition()
        {
            moveSpeed = initialMoveSpeed;
            transform.localPosition = initialPosition;
        }

        protected virtual void ResetRotation()
        {
            rotateSpeed = initialRotateSpeed;
            transform.localEulerAngles = initialRotation;
        }

        protected virtual void ResetPositionAfter(float seconds)
        {
            // making sure to start one instanse of the coroutine `ResetPositionRoutine`
            if (!resetPositionRoutineFlag)
                StartCoroutine(ResetPositionRoutine(seconds));
        }

        protected virtual void ResetRotationAfter(float seconds)
        {
            // making sure to start one instanse of the coroutine `ResetRotationRoutine`
            if (!resetRotationRoutineFlag)
                StartCoroutine(ResetRotationRoutine(seconds));
        }

        private IEnumerator ResetPositionRoutine(float secondeToWait)
        {
            resetPositionRoutineFlag = true;
            moveSpeed = 0f;
            yield return new WaitForSeconds(secondeToWait);
            ResetPosition();
            resetPositionRoutineFlag = false;
        }

        private IEnumerator ResetRotationRoutine(float secondsToWait)
        {
            resetRotationRoutineFlag = true;
            rotateSpeed = 0f;
            yield return new WaitForSeconds(secondsToWait);
            ResetRotation();
            resetRotationRoutineFlag = false;
        }
    }
}