using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HintSystem_VG : MonoBehaviour
    {
        public bool state;
        public UI_Button_VG instruction, controller, controlled, explanation;
        public UI_Button_VG activeButton;

        private Animator animator;
        private AudioSource audioSource;
        private AudioClip currentClip;
        private GameFlowManager gameFlowManager;



        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();
            gameFlowManager = FindObjectOfType<GameFlowManager>();
        }

        public void SetActiveAnimation(bool state)
        {
            animator.SetBool("Active", state);
        }


        public void ClickRequest(UI_Button_VG button)
        {
            // no button is active
            if (activeButton == null)
            {
                activeButton = button;
                PlayAudioFor(activeButton);
                activeButton.SetActiveAnimation(true);
                StartCoroutine(DeactivateAudioAnimationRoutine(button, audioSource.clip.length));
            }
        }

        private IEnumerator DeactivateAudioAnimationRoutine(UI_Button_VG button, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.enabled = true;
            button.SetActiveAnimation(false);
            currentClip = null;
            activeButton = null;
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


    }
}
