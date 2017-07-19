/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/
 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace HandCode
{
    // the order of the tasks is set here. declare task ids in the order you want them to execute.
    public enum TaskID
    {
        NONE,
        MOVE_CRADLE_RIGHT,
        TEST,
        RAISE_ARMS,
    }

    public enum TaskState
    {
        PENDING,
        ACTIVE,
        INTERRUPTED,
        COMPLETE,
    }

    public class Task : MonoBehaviour
    {
        /* fields & properties */

        public TaskID ID;
        public TaskState state
        {
            get
            {
                CheckState();
                return _state;
            }
        }
        public GameObject controllerObject;
        public GameObject controlledObject;
        [Header("Voiceover Clips")]
        public AudioClip explanationAudio;
        public AudioClip ControllerAudio;
        public AudioClip ControlledAudio;
        public Func<bool> completionCondition;
        public List<Task> dependencies; // the list of tasks needed to conteniously be checked during performing this task.
        [SerializeField] // for test only!!!
        private TaskState _state = TaskState.PENDING;
        [Header("Evetns")]
        public UnityEvent onStarted;
        public UnityEvent onInterrupted;
        public UnityEvent onCompleted;



        /* methods & coroutines */

        public void StartTask()
        {
            _state = TaskState.ACTIVE;
            onStarted.Invoke();
        }

        private void Interrupt()
        {
            _state = TaskState.INTERRUPTED;
            onInterrupted.Invoke();
        }

        private void CheckState()
        {
            TaskState oldState = _state;
            if (completionCondition.Invoke())
            {
                _state = TaskState.COMPLETE;
                // invoking `onComplete` event only when the state changes to complete.
                if (oldState != _state)
                    onCompleted.Invoke();
            }
            else if (oldState == TaskState.COMPLETE)
                _state = TaskState.PENDING;
        }

        private void CheckDependencies()
        {
            // preformance note: "`for` is twice faster than `foreach` on generic lists.
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i].state != TaskState.COMPLETE)
                {
                    dependencies[i]._state = TaskState.PENDING;
                    Interrupt();
                }
            }
        }

        private void Update()
        {
            if (_state == TaskState.ACTIVE)
            {
                CheckDependencies();
                CheckState();
            }
        }
    }
}