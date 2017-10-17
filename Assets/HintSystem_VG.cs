using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HintSystem_VG : MonoBehaviour
    {
        public bool activeState;

        public UI_Button_VG instruction, controller, controlled, explanation, power, showMe;
        public UI_Button_VG activeButton, lastClickedButton;

        private Animator animator;
        private Animator instructionAnimator, controllerAnimator, controlledAnimtor, explanationAnimator, powerAnimator, showMeAnimator;
        private Animator[] buttonsAnimators;
        private AudioSource audioSource;
        private AudioClip currentClip;

        private GameFlowManager gameFlowManager;
        private BezierLaserBeam hintLine;

        private bool showMeButtonState;
        private Highlighter activeHighligher;
        private Hologram activeHologram;



        private void Start()
        {
            animator = transform.Find("ProgressRadial_onHand").GetComponent<Animator>();
            instructionAnimator = transform.Find("ProgressRadial_onHand/Button_Instruction/Button").GetComponent<Animator>();
            controllerAnimator = transform.Find("ProgressRadial_onHand/Button_Controller/Button").GetComponent<Animator>();
            controlledAnimtor = transform.Find("ProgressRadial_onHand/Button_Controlled/Button").GetComponent<Animator>();
            explanationAnimator = transform.Find("ProgressRadial_onHand/Button_Explanation/Button").GetComponent<Animator>();
            buttonsAnimators = new Animator[] { instructionAnimator, controlledAnimtor, controllerAnimator, explanationAnimator };
            powerAnimator = transform.Find("ProgressRadial_onHand/Button_Power").GetComponent<Animator>();
            showMeAnimator = transform.Find("ProgressRadial_onHand/Button_ShowMe").GetComponent<Animator>();

            hintLine = GameObject.FindObjectOfType<BezierLaserBeam>();
            hintLine.gameObject.SetActive(false);

            audioSource = GetComponent<AudioSource>();
            gameFlowManager = FindObjectOfType<GameFlowManager>();
        }

        public void SetActiveAnimation(bool state)
        {
            animator.SetBool("Active", state);
        }

        private IEnumerator SlideButtonsRoutine(bool state)
        {
            WaitForSeconds waitTime = new WaitForSeconds(0.1f);
            foreach (Animator a in buttonsAnimators)
            {
                if (state)
                    a.SetTrigger("Slide");
                else
                    a.SetTrigger("Unslide");
                yield return waitTime;
            }
        }

        public void SlideButtons()
        {
            //instruction.gameObject.SetActive(true);
            //controller.gameObject.SetActive(true);
            //controlled.gameObject.SetActive(true);
            //explanation.gameObject.SetActive(true);
            //showMe.gameObject.SetActive(true);
            StartCoroutine(SlideButtonsRoutine(true));
        }

        public void SlideBackButtons()
        {
            StartCoroutine(SlideButtonsRoutine(false));
        }

        public void ClickRequest(UI_Button_VG button)
        {
            print("clicked!");
            animator.SetBool("Active", true);
            // logic for power button
            if (button.ID == UI_Button_ID.POWER)
            {
                powerAnimator.SetTrigger("Click");
                return;
            }

            if (button.ID == UI_Button_ID.SHOW_ME)
                return;

            lastClickedButton = button;
            showMeButtonState = true;
            showMeAnimator.SetBool("Active", showMeButtonState);

            // Swiching between buttons

            // no button is active
            if (activeButton == null)
            {
                ActivateButton(button);
                StartCoroutine(DeactivateAudioAnimationRoutine(button, audioSource.clip.length));
            }
            // the clicked button is the same as the currently active clicked button
            else if (activeButton == button)
            {
                StopAllCoroutines();
                DeactivateCurrentHint();
                StartCoroutine(DeactivateAudioAnimationRoutine(button, 0f));
            }
            // the clicked button is different from the currently active button
            else
            {
                StopAllCoroutines();
                DeactivateCurrentHint();
                StartCoroutine(SwitchButtonRoutine(button));
            }

            showMeAnimator.SetBool("Active", true);
            ShowHintFor(button);
        }

        private IEnumerator SwitchButtonRoutine(UI_Button_VG button)
        {
            yield return DeactivateAudioAnimationRoutine(activeButton, 0f);
            ActivateButton(button);
            StartCoroutine(DeactivateAudioAnimationRoutine(activeButton, audioSource.clip.length));
        }

        private void ActivateButton(UI_Button_VG button)
        {
            activeButton = button;
            PlayAudioFor(activeButton);
            activeButton.SetActiveAnimation(true);
        }
        private IEnumerator DeactivateAudioAnimationRoutine(UI_Button_VG button, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.enabled = true;
            button.SetActiveAnimation(false);
            audioSource.Stop();
            currentClip = null;
            activeButton = null;
            DeactivateCurrentHint(); ///////////////////should be reviewd!
        }

        private void PlayAudioFor(UI_Button_VG button)
        {
            switch (button.ID)
            {
                case UI_Button_ID.INSTRUCTION:
                    currentClip = gameFlowManager.currentTask.GetComponent<TaskHint>().instructionAudio;
                    break;

                case UI_Button_ID.CONTROLLER:
                    currentClip = gameFlowManager.currentTask.GetComponent<TaskHint>().ControllerAudio;
                    break;

                case UI_Button_ID.CONTROLLED:
                    currentClip = gameFlowManager.currentTask.GetComponent<TaskHint>().ControlledAudio;
                    break;
                case UI_Button_ID.EXPLANATION:
                    currentClip = gameFlowManager.currentTask.GetComponent<TaskHint>().explanationAudio;
                    break;
            }
            audioSource.clip = currentClip;
            audioSource.Play();
        }

        private void ShowHintFor(UI_Button_VG button)
        {
            DeactivateCurrentHint();

            TaskHint taskHint = gameFlowManager.currentTask.GetComponent<TaskHint>();
            switch (button.ID)
            {
                case UI_Button_ID.INSTRUCTION:
                    StartCoroutine(HintRoutine(taskHint.instructionHologram));
                    break;

                case UI_Button_ID.CONTROLLER:
                    StartCoroutine(HintRoutine(taskHint.controllerHighlighter, taskHint.ControllerAudio.length));
                    ShowHintLine(taskHint.controllerHighlighter.gameObject);
                    break;

                case UI_Button_ID.CONTROLLED:
                    StartCoroutine(HintRoutine(taskHint.controlledHighlighter, taskHint.ControlledAudio.length));
                    ShowHintLine(taskHint.controlledHighlighter.gameObject);
                    break;
            }
        }


        // Hint Methods and Routines used by method `ShowHintFor`, and should NOT used explicitely.
        
        private IEnumerator HintRoutine(Highlighter highlighter, float period = 5f)
        {
            DeactivateCurrentHint();

            activeHighligher = highlighter;
            highlighter.enabled = true;
            yield return new WaitForSeconds(period);
            highlighter.enabled = false;
            activeHighligher = null;
        }
        private IEnumerator HintRoutine(Hologram hologram, float period = 5f)
        {
            DeactivateCurrentHint();

            activeHologram = hologram;
            hologram.gameObject.SetActive(true);
            yield return new WaitForSeconds(period);
            hologram.gameObject.SetActive(false);
            activeHologram = null;
        }


        private void DeactivateCurrentHint()
        {
            if (activeHologram != null)
            {
                activeHologram.gameObject.SetActive(false);
                activeHologram = null;
            }
            if (activeHighligher != null)
            {
                activeHighligher.enabled = false;
                activeHighligher = null;
            }
        }

        private void ShowHintLine(GameObject obj)
        {
            Transform lineDestination = obj.transform.Find("LineDestination");
            if(lineDestination == null)
            {
                lineDestination = obj.transform;
            }

            hintLine.destination = lineDestination;
            hintLine.gameObject.SetActive(true);

        }

        private void HideHintLine()
        {
            hintLine.destination = null;
            hintLine.gameObject.SetActive(false);
        }



        /* ********************************************************
         * should have another override for the line renderer hint.
         * ********************************************************/
    }
}
