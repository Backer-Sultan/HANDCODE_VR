using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HS_Button_VG : MonoBehaviour
    {
        Animator animator;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void PlayClickAnimation()
        {
            animator.SetTrigger("Click");
        }

        public void PlaySlideAnimation()
        {
            animator.SetTrigger("Slide");
        }
        public void SetAudioAnimation(bool state)
        {
            animator.SetBool("Audio", state);
        }
    } 
}
