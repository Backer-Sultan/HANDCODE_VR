/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VRTK;

namespace HandCode
{
    public class UI_Button : MonoBehaviour
    {
        /* fields & properties */

        public UnityEvent onClick;

        private Animator animator;



        /* methods & coroutines */

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                print("Error!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finger")
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right), 1f);
                animator.SetBool("Active", true);
                onClick.Invoke();
            }
        }
    }
}