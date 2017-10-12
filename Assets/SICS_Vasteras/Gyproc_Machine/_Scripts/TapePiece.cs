using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class TapePiece : MonoBehaviour
    {
        public CradlePosition ID;
        public bool isMounted { get { return _isMounted; } }

        private bool _isMounted;
        private Animator animator;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            if(animator == null)
                Debug.LogError(string.Format("{0}\nTapePiece.cs: No Animator is found in the hierarchy!", Machine.GetPath(gameObject)));
        }

        public void Mount()
        {
            if (!_isMounted)
            {
                _isMounted = true;
                animator.SetBool("Mount", _isMounted);
            }
        }
        
        public void Unmount()
        {
            if(_isMounted)
            {
                _isMounted = false;
                animator.SetBool("Mount", _isMounted);
            }
        }


        public void OnTriggerEnter(Collider other)
        {
            if (other.name == "SmallTapeRoll_01")
            {
                Mount();
            }
        }
    }
}