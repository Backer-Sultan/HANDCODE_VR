using System.Collections;
using UnityEngine;

namespace HandCode
{
    public class HintSystem_VG : MonoBehaviour
    {
        public bool activeState;
        public UI_Button_VG instruction, controller, controlled, explanation, power, showMe;

        private UI_Button_VG activeButton, lastClickedButton;
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
            powerAnimator = transform.Find("ProgressRadial_onHand/Button_Power").GetComponent<Animator>();
            showMeAnimator = transform.Find("ProgressRadial_onHand/Button_ShowMe").GetComponent<Animator>();

            buttonsAnimators = new Animator[] { instructionAnimator, controlledAnimtor, controllerAnimator, explanationAnimator };
            hintLine = GameObject.FindObjectOfType<BezierLaserBeam>();
            hintLine.gameObject.SetActive(false);

            audioSource = GetComponent<AudioSource>();
            gameFlowManager = FindObjectOfType<GameFlowManager>();
        }

        public void ClickRequest(UI_Button_VG button)
        {
            animator.SetBool("Active", true);

            if (button.ID == UI_Button_ID.POWER)
            {
                powerAnimator.SetTrigger("Click");
                return;
            }

            if (button.ID == UI_Button_ID.SHOW_ME)
                return;

            lastClickedButton = button;
            // specific for the old show-me button
            /*showMeButtonState = true; 
            showMeAnimator.SetBool("Active", showMeButtonState);*/

            /* Swiching between buttons */

            // no button is active
            if (activeButton == null)
            {
                ActivateButton(button);
                StartCoroutine(DeactivateCurrentHintRoutine(button, audioSource.clip.length));
            }
            // the clicked button is the same as the currently active clicked button
            else if (activeButton == button)
            {
                StopAllCoroutines();
                StartCoroutine(DeactivateCurrentHintRoutine(button, 0f));
            }
            // the clicked button is different from the currently active button
            else
            {
                StopAllCoroutines();
                StartCoroutine(SwitchButtonRoutine(button));
            }

            showMeAnimator.SetBool("Active", true);
            ShowHintFor(button);
        }
        private IEnumerator SwitchButtonRoutine(UI_Button_VG button)
        {
            yield return DeactivateCurrentHintRoutine(activeButton, 0f);
            ActivateButton(button);
            StartCoroutine(DeactivateCurrentHintRoutine(activeButton, audioSource.clip.length));
        }

        /* Hint deactivation */
        private void DeactivateCurrentHint()
        {
            if (activeHologram != null)
            {
                activeHologram.enabled = false;
                activeHologram = null;
            }
            if (activeHighligher != null)
            {
                activeHighligher.enabled = false;
                activeHighligher = null;
            }
            if (hintLine != null)
                HideHintLine();
        }
        private IEnumerator DeactivateCurrentHintRoutine(UI_Button_VG button, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.enabled = true;
            button.SetActiveAnimation(false);
            audioSource.Stop();
            currentClip = null;
            activeButton = null;
            DeactivateCurrentHint();
        }
        
        /* Button animations */
        private void ActivateButton(UI_Button_VG button)
        {
            activeButton = button;
            PlayAudioFor(activeButton);
            activeButton.SetActiveAnimation(true);
        }
        public void SetActiveAnimation(bool state)
        {
            animator.SetBool("Active", state);
        }
        public void SlideButtons()
        {
            StartCoroutine(SlideButtonsRoutine(true));
        }
        public void SlideBackButtons()
        {
            StartCoroutine(SlideButtonsRoutine(false));
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
        
        private void PlayAudioFor(UI_Button_VG button)
        {
            TaskHint taskHint = gameFlowManager.currentTask.GetComponent<TaskHint>();
            switch (button.ID)
            {
                case UI_Button_ID.INSTRUCTION:
                    if (taskHint.instructionAudio != null)
                        currentClip = taskHint.instructionAudio;
                    break;

                case UI_Button_ID.CONTROLLER:
                    if (taskHint.ControllerAudio != null)
                        currentClip = taskHint.ControllerAudio;
                    break;

                case UI_Button_ID.CONTROLLED:
                    if (taskHint.ControlledAudio != null)
                        currentClip = taskHint.ControlledAudio;
                    break;
                case UI_Button_ID.EXPLANATION:
                    if (taskHint.explanationAudio != null)
                        currentClip = taskHint.explanationAudio;
                    break;
            }
            audioSource.clip = currentClip;
            if (currentClip != null)
                audioSource.Play();
        }
        private void ShowHintFor(UI_Button_VG button)
        {
            DeactivateCurrentHint();

            TaskHint taskHint = gameFlowManager.currentTask.GetComponent<TaskHint>();

            if (taskHint == null)
            {
                print(string.Format("TaskHint for task {0} is not found!", gameFlowManager.currentTask.ID));
                return;
            }

            switch (button.ID)
            {
                case UI_Button_ID.INSTRUCTION:
                    if (taskHint.instructionHologram != null)
                        StartCoroutine(HintRoutine(taskHint.instructionHologram));
                    break;

                case UI_Button_ID.CONTROLLER:
                    if (taskHint.controllerHighlighter != null)
                        StartCoroutine(HintRoutine(taskHint.controllerHighlighter, taskHint.ControllerAudio.length));
                    ShowHintLine(taskHint.controlButton);
                    break;

                case UI_Button_ID.CONTROLLED:
                    if (taskHint.controlledHighlighter != null)
                        StartCoroutine(HintRoutine(taskHint.controlledHighlighter, taskHint.ControlledAudio.length));
                    ShowHintLine(taskHint.controlledHighlighter.transform);
                    break;
            }
        }

        /* Hint Methods and Routines used by method `ShowHintFor`,  They should NOT be used directly. */
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
            hologram.enabled = true;
            yield return new WaitForSeconds(period);
            hologram.enabled = false;
            activeHologram = null;
        }

        /* Show/Hide line rednerer */
        private void ShowHintLine(Transform destination)
        {
            Transform lineDestination = destination.Find("LineDestination");
            if (lineDestination == null)
            {
                lineDestination = destination.transform;
            }

            hintLine.destination = lineDestination;
            hintLine.gameObject.SetActive(true);

        }
        private void HideHintLine()
        {
            hintLine.destination = null;
            hintLine.gameObject.SetActive(false);
        }

        // test code (interaction from keyboard)
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                ClickRequest(power);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                ClickRequest(instruction);
            }

            if(Input.GetKeyDown(KeyCode.J))
            {
                ClickRequest(controlled);
            }

            if(Input.GetKeyDown(KeyCode.K))
            {
                ClickRequest(controller);
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                ClickRequest(explanation);
            }
        }
    }
}
