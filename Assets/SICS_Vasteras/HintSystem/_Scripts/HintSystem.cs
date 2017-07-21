using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace HandCode
{
    public class HintSystem : MonoBehaviour
    {
        public bool active = true;

        private GameFlowManager manager;
        private AudioSource audioSource;
        private BezierLaserBeam hintLine;
        public GameObject controller;
        private void Start()
        {
            manager = FindObjectOfType<GameFlowManager>();
            if (manager == null)
                Debug.LogError(string.Format("{0}\nHintSystem.cs: no `GameFlowManager` script is found in the scene!", Machine.GetPath(gameObject)));

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            hintLine = GetComponentInChildren<BezierLaserBeam>();
            if (hintLine == null)
                Debug.LogError(string.Format("{0}\nHintSystem.cs: no `BezierLaserBeam` script is found on any of the children objects", Machine.GetPath(gameObject)));
        }

        public void SetControllerHighlight(bool activeState)
        {
            SetHighlight(activeState, manager.currentTask.controllerObject);
        }

        public void SetControlledHighlight(bool activeState)
        {
            SetHighlight(activeState, manager.currentTask.controlledObject);
        }

        private void SetHighlight(bool activeState, GameObject obj)
        {
            if (active && obj != null)
            {
                Highlighter highlighter = obj.GetComponent<Highlighter>();
                if (highlighter != null)
                    highlighter.enabled = activeState;
                else
                    Debug.LogError(string.Format("{0}\nHintSystem.cs: `Highlighter` script is missing on Object {1}!",
                        Machine.GetPath(gameObject), obj.name));
            }
        }


        public void PlayControllerVoiceOver()
        {
            PlayVoiceOver(manager.currentTask.ControllerAudio);
        }

        public void PlayControlledVoiceOver()
        {
            PlayVoiceOver(manager.currentTask.ControlledAudio);
        }

        private void PlayVoiceOver(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void DrawLineToController()
        {
            GameObject from = GameObject.FindGameObjectWithTag("Finger");
            GameObject to = manager.currentTask.controllerObject;
            DrawLine(from, to);
        }

        public void DrawLineToControlled()
        {
            GameObject from = GameObject.FindGameObjectWithTag("Finger");
            GameObject to = manager.currentTask.controlledObject;
            DrawLine(from, to);
        }

        private void DrawLine(GameObject fromObj, GameObject toObj)
        {
            hintLine.origin = fromObj.transform;
            hintLine.destination = toObj.transform;
            Transform lineDestination = toObj.transform.Find("LineDestination");
            if (lineDestination != null)
                hintLine.destination = lineDestination;
            hintLine.enabled = true;
        }

        public void HideLine()
        {
            hintLine.enabled = false;
        }


        

        // tests
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                SetControllerHighlight(true);
            if (Input.GetKeyDown(KeyCode.J))
                SetControllerHighlight(false);

            if (Input.GetKeyDown(KeyCode.N))
                SetControlledHighlight(true);
                
            if (Input.GetKeyDown(KeyCode.M))
                SetControlledHighlight(false);

            if (Input.GetKeyDown(KeyCode.V))
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left), 0.5f);
               // VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(3), 1f);
           
            }
        }

    }
    
}
