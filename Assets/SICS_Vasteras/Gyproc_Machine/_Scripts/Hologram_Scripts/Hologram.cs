/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class Hologram : MonoBehaviour
    {
        /* fields & properties */

        [Range(0f, 1f)]
        public float movementSpeed = 0.5f;
        [Range(0f, 20f)]
        public float rotationSpeed = 0.5f;
        public float waitTime = 3f; // time to wait before resetting the hologram to its initial position and rotation
        public Color color1 = new Color(1f, 0.5f, 0.5f, 0.2f);
        public Color color2 = new Color(1f, 0.86f, 0f, 0.2f);

        internal Vector3 initialPosition; // stored as local position
        internal Vector3 initialRotation; // stored as local euler angles

        internal float initialMovementSpeed;
        internal float initialRotationSpeed;
        internal Transform[] children;

        private Color lerpedColor;
        private Renderer[] rends;



        /* methods & coroutines */

        protected float GetSignedRotation(float angle)
        {
            float signedAngle = (angle > 180f) ? angle - 360f : angle;
            return signedAngle;
        }

        protected virtual void Awake()
        {
            // gettin ginitial values for position, rotation and speed
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles;
            initialMovementSpeed = movementSpeed;
            initialRotationSpeed = rotationSpeed;

            // getting references to children
            rends = GetComponentsInChildren<Renderer>(true);
            children = GetComponentsInChildren<Transform>(true);
        }

        // general color lerp
        protected virtual void Update()
        {
            lerpedColor = Color.Lerp(color1, color2, Mathf.PingPong(Time.time * 2f, 1));
            foreach (Renderer rend in rends)
                rend.material.color = lerpedColor;
        }

        protected virtual void OnEnable()
        {
            ResetPosition();
            ResetRotation();

            foreach (Transform t in children)
                t.gameObject.SetActive(true);
        }

        protected virtual void OnDisable()
        {
            foreach (Transform t in children)
            {
                if (t == transform)
                    continue;
                t.gameObject.SetActive(false);
            }
        }

        protected virtual void ResetPosition()
        {
            transform.localPosition = initialPosition;
            movementSpeed = initialMovementSpeed;
        }

        protected virtual void ResetRotation()
        {
            transform.localEulerAngles = initialRotation;
            rotationSpeed = initialRotationSpeed;
        }
    }
}