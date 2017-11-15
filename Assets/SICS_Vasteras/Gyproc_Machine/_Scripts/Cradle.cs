/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VirtualGrasp;

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
        [Range(1f, 30f)]
        public float pinsherRotationSpeed = 5f;
        public Transform pinsherRotator;
        public Transform pinsherModelToLock;
        public bool isBreakApplied { get { return _isBreakApplied; } }
        public bool isRightTargetReached { get { return _isRightTargetReached; } }
        public bool isMiddleTargetReached { get { return _isMiddleTargetReached; } }
        public bool isPinsherLow { get { return _isPinsherLow; } }
        //[HideInInspector]
        [Header("Tape Pieces")]
        public TapePiece tapePiece_right;
        public TapePiece tapePiece_middle;
        public TapePiece tapePiece_left;

        public CradlePosition cradlePos = CradlePosition.MIDDLE;
        [Header("Events")]
        public UnityEvent onTargetReached;
        public UnityEvent onTargetLeft;
        public UnityEvent onPinsherLowered;
        public UnityEvent onPinsherRaised;
        public UnityEvent onBreakEnabled;
        public UnityEvent onBreakDisabled;

        private AudioSource audioSource;
        private Transform pinsherModel;
        private bool _isBreakApplied = true;
        private bool _isPinsherLow = false;
        private bool _isRightTargetReached = false;
        private bool _isMiddleTargetReached = false;
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

            pinsherModel = pinsherRotator.Find("Pinsher_05");
            if (pinsherModel == null)
                Debug.LogError(string.Format("{0}\nCradle.cs: No object with name 'Pinsher_05' is found under object 'PinsherRotator'!", Machine.GetPath(gameObject)));

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
            StopAllCoroutines();
            StartCoroutine(LowerPinsherRoutine());
        }

        private IEnumerator LowerPinsherRoutine()
        {
            while(GetSignedRotation(pinsherModel.localEulerAngles.z) > -130f)
            {
                pinsherModel.localEulerAngles += Vector3.back * Time.deltaTime * pinsherRotationSpeed;
                yield return null;
            }
            _isPinsherLow = true;
            onPinsherLowered.Invoke();
        }

        public void RaisePinsher()
        {
            _isPinsherLow = false;
            StopAllCoroutines();
            StartCoroutine(RaisePinsherRoutine());
        }

        private IEnumerator RaisePinsherRoutine()
        {
            while(GetSignedRotation(pinsherModel.localEulerAngles.z) < 0f)
            {
                pinsherModel.localEulerAngles -= Vector3.back * Time.deltaTime * pinsherRotationSpeed;
                yield return null;
            }
            _isPinsherLow = false;
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
                {
                    _isMiddleTargetReached = true;
                    Stop();
                }
                else
                    _isMiddleTargetReached = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // moved to a new script to fix the trigger exit problem when rotating the pinsher.

            //if (other.tag == "CradleLimitRight")
            //{
            //    Stop();
            //    cradlePos = CradlePosition.RIGHT;
            //    _isTargetReached = true;
            //    onTargetReached.Invoke();
            //    return;
            //}
            if (other.tag == "CradleLimitLeft")
            {
                Stop();
                cradlePos = CradlePosition.LEFT;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //if (cradlePos == CradlePosition.RIGHT)
            //    _isTargetReached = false;
            //onTargetLeft.Invoke();
            //cradlePos = CradlePosition.MIDDLE;
            if(other.tag == "CradleLimitLeft")
            {
                cradlePos = CradlePosition.MIDDLE;
            }
        }

        public void MarkReachedTarget()
        {
            Stop();
            cradlePos = CradlePosition.RIGHT;
            _isRightTargetReached = true;
            onTargetReached.Invoke();
            return;
        }

        public void MarkLeftTarget()
        {
            cradlePos = CradlePosition.MIDDLE;
            _isRightTargetReached = false;
            onTargetLeft.Invoke();
        }

        

        private void Update()
        {
            if (GetSignedRotation(pinsherRotator.localEulerAngles.z) <= -130f)
            {
                _isPinsherLow = true;
            }
            else
            {
                _isPinsherLow = false;
            }

           

            // teset

            if (Input.GetKeyDown(KeyCode.Alpha8))
                LowerPinsher();

            if (Input.GetKeyDown(KeyCode.Alpha9))
                print(GetSignedRotation(pinsherRotator.localEulerAngles.z));
        }

        private float GetSignedRotation(float angle)
        {
            float signedAngle = (angle > 180f) ? angle - 360f : angle;
            return signedAngle;
        }
    } 
}