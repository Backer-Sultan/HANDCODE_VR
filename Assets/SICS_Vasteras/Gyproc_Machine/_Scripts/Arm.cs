/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class Arm : MonoBehaviour
    {
        /* fields & properties */

        [Range(0f, 1f)]
        public float speed = 0.1f;
        public enum ArmPosition { MIDDLE, LEFT, RIGHT };
        public ArmPosition armPos { get { return _armPos; } }
        public ArmPosition _armPos = ArmPosition.MIDDLE;

        private bool isMoving = false;
        private Vector3 direction = Vector3.zero;
        private AudioSource audioSource;



        /* methods & coroutines */

        private void Start()
        {
            audioSource = GetComponentInChildren<AudioSource>();
            if (!audioSource)
                Debug.LogError(string.Format("{0}\nArm.cs: Component `AudioSource` is missing!", Machine.GetPath(gameObject)));
        }

        public void MoveLeft()
        {
            if (_armPos != ArmPosition.LEFT)
            {
                direction = Vector3.back;
                isMoving = true;
                PlaySound(MachineSounds.Instance.Arm_Moving);
            }

        }

        public void MoveRight()
        {
            if (_armPos != ArmPosition.RIGHT)
            {
                direction = Vector3.forward;
                isMoving = true;
                PlaySound(MachineSounds.Instance.Arm_Moving);
            }
        }

        public void Stop()
        {
            isMoving = false;
            PlaySound(MachineSounds.Instance.Arm_Stopping);
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }

        private void Update()
        {
            if (isMoving)
                transform.Translate(direction * speed * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "ArmLimitLeft")
            {
                Stop();
                _armPos = ArmPosition.LEFT;
            }
            if (other.tag == "ArmLimitRight")
            {
                Stop();
                _armPos = ArmPosition.RIGHT;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "ArmLimitLeft" || other.tag == "ArmLimitRight")
                _armPos = ArmPosition.MIDDLE;
        }
    }
}
