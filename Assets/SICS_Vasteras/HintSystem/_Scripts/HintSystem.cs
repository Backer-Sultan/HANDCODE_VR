using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HintSystem : MonoBehaviour
    {
        public bool active = true;

        private GameFlowManager manager;

        private void Start()
        {
            manager = FindObjectOfType<GameFlowManager>();
            if (manager == null)
                Debug.LogError(string.Format("{0}\nHintSystem.cs: no `GameFlowManager` script is found in the scene!", Machine.GetPath(gameObject)));
        }

        public void SetControllerHighlight(bool activeState)
        {
            if (active && manager.currentTask.controllerObject!=null)
            {
                Highlighter highlighter = manager.currentTask.controllerObject.GetComponent<Highlighter>();
                if (highlighter != null)
                    highlighter.enabled = activeState;
                else
                    Debug.LogError(string.Format("{0}\nHintSystem.cs: `Highlighter` script is missing on Object `{1}`!",
                        Machine.GetPath(gameObject), Machine.GetPath(manager.currentTask.controllerObject)));
            }
        }

        public void SetControlledHighlight(bool activeState)
        {
            if (active && manager.currentTask.controlledObject != null)
            {
                Highlighter highlighter = manager.currentTask.controlledObject.GetComponent<Highlighter>();
                if (highlighter != null)
                    highlighter.enabled = activeState;
                else
                    Debug.LogError(string.Format("{0}\nHintSystem.cs: `Highlighter` script is missing on Object `{1}`!",
                        Machine.GetPath(gameObject), Machine.GetPath(manager.currentTask.controlledObject)));
            }
        }


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
        }

    } 
}
