/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
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
        private HintSystem hintSystem;



        /* methods & coroutines */

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                print("Error!");

            hintSystem = GetComponentInParent<HintSystem>();
            if (hintSystem == null)
                print("Error!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finger")
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right), 1f);
                hintSystem.lastPressedButton = this;
                animator.SetBool("Active", true);
                onClick.Invoke();
                StartCoroutine(SetButtonsBackRoutine());
            }
        }

        private IEnumerator SetButtonsBackRoutine()
        {
            yield return new WaitForSeconds(hintSystem.audioSource.clip.length);
            animator.SetBool("Active", false);
        }
    }
}