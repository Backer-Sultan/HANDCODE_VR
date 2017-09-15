/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using UnityEngine.Events;

namespace HandCode
{
    public enum CradlePosition
    {
        MIDDLE,
        LEFT,
        RIGHT,
    }

    public class Cradle : MonoBehaviour
    {
        /* fields & properties */

        [Range(0f, 1f)]
        public float speed = 0.5f;
        public Transform pinsherRotator;
        public bool isBreakApplied { get { return _isBreakApplied; } }
        public bool isTargetReached { get { return _isTargetReached; } }
        public bool isPinsherLow { get { return _isPinsherLow; } }
        [HideInInspector]
        public CradlePosition cradlePos = CradlePosition.MIDDLE;
        [Header("Events")]
        public UnityEvent onTargetReached;
        public UnityEvent onTargetLeft;
        public UnityEvent onPinsherLowered;
        public UnityEvent onPinsherRaised;
        public UnityEvent onBreakEnabled;
        public UnityEvent onBreakDisabled;

        private AudioSource audioSource;
        private Animator pinsherAnimator;
        private bool _isBreakApplied = true;
        private bool _isPinsherLow = false;
        private bool _isTargetReached = false;
        private bool isMoving = false;
        private Vector3 direction = Vector3.zero;



        /* methods & coroutines */

        private void Start()
        {
            // initialization
            if (pinsherRotator == null)
                pinsherRotator = transform.Find("PinsherRotator");
            if (pinsherRotator == null)
                Debug.LogError(string.Format("{0}\nCradle.cs: Object `PinsherRotator` is missing!", Machine.GetPath(gameObject)));

            pinsherAnimator = pinsherRotator.GetComponent<Animator>();
            if (pinsherAnimator == null)
                Debug.LogError(string.Format("{0}\nCrale.cs: Component `Animator` is missing on object `PinsherRotator`!", Machine.GetPath(gameObject)));

            audioSource = GetComponentInChildren<AudioSource>();
            if (audioSource == null)
                Debug.LogError(string.Format("{0}\nCradle.cs: Component `AudioSource` is missing!", Machine.GetPath(gameObject)));
        }

        public void MoveToLeft()
        {
            if (cradlePos != CradlePosition.LEFT)
            {
                PlaySound(MachineSounds.Instance.Cradle_Moving);
                direction = Vector3.left;
                isMoving = true;
            }
        }

        public void MoveToRight()
        {
            if (cradlePos != CradlePosition.RIGHT)
            {
                PlaySound(MachineSounds.Instance.Cradle_Moving);
                direction = Vector3.right;
                isMoving = true;
            }
        }

        public void Stop()
        {
            PlaySound(MachineSounds.Instance.Cradle_Stopping);
            isMoving = false;
        }

        public void LowerPinsher()
        {
            _isPinsherLow = true;
            pinsherAnimator.SetBool("LowerPinsher", _isPinsherLow);
            onPinsherLowered.Invoke();
        }

        public void RaisePinsher()
        {
            _isPinsherLow = false;
            pinsherAnimator.SetBool("LowerPinsher", _isPinsherLow);
            onPinsherRaised.Invoke();
        }

        public void SetBreak(bool state)
        {
            PlaySound(MachineSounds.Instance.Cradle_BreakToggle);
            _isBreakApplied = state;
            if (_isBreakApplied)
                onBreakEnabled.Invoke();
            else
                onBreakDisabled.Invoke();
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                transform.Translate(direction * speed * Time.deltaTime, Space.Self);
                // stopping when reaching the middle position (aproximately (0,0,0)).
                if (Vector3.Distance(transform.localPosition, Vector3.zero) <= Vector3.kEpsilon)
                    Stop();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "CradleLimitRight")
            {
                Stop();
                cradlePos = CradlePosition.RIGHT;
                _isTargetReached = true;
                onTargetReached.Invoke();
                return;
            }
            if (other.tag == "CradleLimitLeft")
            {
                Stop();
                cradlePos = CradlePosition.LEFT;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (cradlePos == CradlePosition.RIGHT)
                _isTargetReached = false;
                onTargetLeft.Invoke();
            cradlePos = CradlePosition.MIDDLE;
        }
    } 
}