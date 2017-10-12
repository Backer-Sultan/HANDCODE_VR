///*********************************************
// * Project: HANDCODE                         *
// * Author:  Backer Sultan                    *
// * Email:   backer.sultan@ri.se              *
// * *******************************************/

//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using VRTK;

//namespace HandCode
//{
//    public class UI_Button : MonoBehaviour
//    {
//        /* fields & properties */

//        public UnityEvent onClick;
//        public Animator animator;

//        private HintSystem hintSystem;



//        /* methods & coroutines */

//        private void Start()
//        {
//            animator = GetComponentInChildren<Animator>();
//            if (animator == null)
//                print("Error!");

//            hintSystem = GetComponentInParent<HintSystem>();
//            if (hintSystem == null)
//                print("Error!");
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if (other.tag == "Finger")
//            {
//                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right), 1f);
//                //deactivating animation on the currently active button
//                if (hintSystem.currentPressedButton != null)
//                    hintSystem.currentPressedButton.animator.SetBool("Active", false);
//                if (hintSystem.currentActiveHighlighter != null)
//                {
//                    hintSystem.currentActiveHighlighter.enabled = false;
//                    hintSystem.currentActiveHighlighter = null;
//                }
//                if (hintSystem.currentClip != null)
//                {
//                    hintSystem.audioSource.Stop();
//                    hintSystem.currentClip = null;
//                }   
//                hintSystem.currentPressedButton = this;
//                animator.SetBool("Active", true);
//                onClick.Invoke();
//                StartCoroutine(SetButtonsBackRoutine());
//            }
//        }

//        private IEnumerator SetButtonsBackRoutine()
//        {
//            yield return new WaitForSeconds(hintSystem.audioSource.clip.length);
//            animator.SetBool("Active", false);
//        }
//    }
//}