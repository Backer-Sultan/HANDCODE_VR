///*********************************************
// * Project: HANDCODE                         *
// * Author:  Backer Sultan                    *
// * Email:   backer.sultan@ri.se              *
// * *******************************************/

//using UnityEngine;
//using System.Collections;
//using VRTK;

//namespace HandCode
//{
//    public class HintSystem : MonoBehaviour
//    {
//        /* fields & properties */

//        public bool active = true;
//        public UI_Button currentPressedButton;
//        public Highlighter currentActiveHighlighter;
//        public AudioClip currentClip;
//        public AudioSource audioSource;

//        private GameFlowManager manager;
//        private BezierLaserBeam hintLine;



//        /* methods & coroutines */

//        private void Start()
//        {
//            manager = FindObjectOfType<GameFlowManager>();
//            if (manager == null)
//                Debug.LogError(string.Format("{0}\nHintSystem.cs: no `GameFlowManager` script is found in the scene!", Machine.GetPath(gameObject)));

//            audioSource = GetComponent<AudioSource>();
//            if (audioSource == null)
//                audioSource = gameObject.AddComponent<AudioSource>();

//            hintLine = GetComponentInChildren<BezierLaserBeam>();
//            if (hintLine == null)
//                Debug.LogError(string.Format("{0}\nHintSystem.cs: no `BezierLaserBeam` script is found on any of the children objects", Machine.GetPath(gameObject)));

//            HideLine();
//        }

//        public void HintController()
//        {
//            PlayVoiceOver(manager.currentTask.ControllerAudio);
//            HighlightForPeriod(manager.currentTask.controllerObject, currentClip.length);
//        }

//        public void HintControlled()
//        {
//            PlayVoiceOver(manager.currentTask.ControlledAudio);
//            HighlightForPeriod(manager.currentTask.controlledObject, currentClip.length);
//        }
        
//        public void HintInstruction()
//        {
//            PlayVoiceOver(manager.currentTask.instructionAudio);
//        }

//        public void HintExplanation()
//        {
//            PlayVoiceOver(manager.currentTask.explanationAudio);
//        }

//        private void SetHighlight(GameObject obj, bool activeState)
//        {
//            if (active && obj != null)
//            {
//                Highlighter highlighter = obj.GetComponent<Highlighter>();
//                if (highlighter != null)
//                    highlighter.enabled = activeState;
//                else
//                    Debug.LogError(string.Format("{0}\nHintSystem.cs: `Highlighter` script is missing on Object {1}!",
//                        Machine.GetPath(gameObject), obj.name));
//            }
//        }
//        private void HighlightForPeriod(GameObject obj, float period)
//        {
//            StartCoroutine(HighlightForPeriodRoutine(obj, period));
//        }
//        private IEnumerator HighlightForPeriodRoutine(GameObject obj, float period)
//        {
//            SetHighlight(obj, true);
//            currentActiveHighlighter = obj.GetComponent<Highlighter>();
//            yield return new WaitForSeconds(period);
//            SetHighlight(obj, false);
//            currentActiveHighlighter = null;
//        }

//        private void PlayVoiceOver(AudioClip clip)
//        {
//            audioSource.Stop();
//            audioSource.clip = clip;
//            audioSource.Play();
//            currentClip = clip;
//        }

//        /* LineRenderer Methods */

//        public void DrawLineToController()
//        {
//            GameObject from = GameObject.FindGameObjectWithTag("Finger");
//            GameObject to = manager.currentTask.controllerObject;
//            DrawLine(from, to);
//        }

//        public void DrawLineToControlled()
//        {
//            GameObject from = GameObject.FindGameObjectWithTag("Finger");
//            GameObject to = manager.currentTask.controlledObject;
//            DrawLine(from, to);
//        }

//        private void DrawLine(GameObject fromObj, GameObject toObj)
//        {
//            hintLine.origin = fromObj.transform;
//            hintLine.destination = toObj.transform;
//            Transform lineDestination = toObj.transform.Find("LineDestination");
//            if (lineDestination != null)
//                hintLine.destination = lineDestination;
//            hintLine.enabled = true;
//        }

//        public void HideLine()
//        {
//            hintLine.enabled = false;
//        }
//    }
   
    
//}
