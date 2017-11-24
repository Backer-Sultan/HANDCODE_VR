/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using UnityEngine.Events;

namespace HandCode
{
    public class Spool : MonoBehaviour
    {
        /* fields & properties */

        public Identifier ID;
        [Range(0f, 10f)]
        public float speed = 1f;
        [Header("Events")]
        public UnityEvent onTargetReached;
        public UnityEvent onDamaged;
        public UnityEvent onHandled;
        public bool isTargetReached { get { return _isTargetReached; } }
        public bool isDamaged { get { return _isDamaged; } }
        public bool isHandled { get { return _isHandled; } }
        [HideInInspector]
        public bool isLeftSideHandled = false;
        [HideInInspector]
        public bool isRightSideHandled = false;
        
        internal bool _isDamaged = false;
        internal bool _isHandled = false;
        private bool isMoving = false;
        private bool _isTargetReached = false;



        /* methods & coroutines */

        private void Start()
        {
            if (ID == Identifier.NONE)
                Debug.LogError(string.Format("{0}\nSpool.cs: ID can't be empty!", Machine.GetPath(gameObject)));
        }

        public void ApplyDamage()
        {
            _isDamaged = true;
            onDamaged.Invoke();
        }

        public void Handle()
        {
            _isHandled = true;
            onHandled.Invoke();
        }

        public void MoveToTarget()
        {
            isMoving = true;
        }

        public void Stop()
        {
            isMoving = false;
        }

        private void Update()
        {
            if (isMoving && !_isDamaged)
            {
                _isTargetReached = Vector3.Distance(transform.localPosition, Vector3.zero) <= Vector3.kEpsilon;
                if (_isTargetReached)
                {
                    Stop();
                    onTargetReached.Invoke();
                }
                else
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, speed * Time.deltaTime);
            }
        }
    }
}
