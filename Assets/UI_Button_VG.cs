using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public enum UI_Button_ID
    {
        NONE,
        INSTRUCTION,
        CONTROLLER,
        CONTROLLED,
        EXPLANATION,
        POWER,
        SHOW_ME,
    }

    public class UI_Button_VG : MonoBehaviour
    {

        public UI_Button_ID ID; 

        Animator animator;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void SetActiveAnimation(bool state)
        {
            animator.SetBool("Active", state);
        }

        public void PlaySlideAnimation()
        {
            animator.SetTrigger("Slide");
        }
    } 
}
