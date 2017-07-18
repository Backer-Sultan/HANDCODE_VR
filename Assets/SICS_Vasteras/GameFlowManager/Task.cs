using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HandCode
{
    // the order of the tasks is set here. declare task ids in the order you want them to execute.
    public enum TaskID
    {
        NONE,
        MOVE_CRADLE,
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
        public TaskID ID;
        public TaskState state { get { return _state; } }
        public GameObject controllerObject;
        public GameObject controlledObject;
        [Header("Voiceover Clips")]
        public AudioClip explanationAudio;
        public AudioClip ControllerAudio;
        public AudioClip ControlledAudio;
        public bool completionCondition;
        public List<Task> dependencies; // the list of tasks needed to conteniously be checked during performing this task.
        [Header("Evetns")]
        public UnityEvent onStarted;
        public UnityEvent onInterrupted;
        public UnityEvent onCompleted;

        private TaskState _state = TaskState.PENDING;



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

        private bool CheckSelf()
        {
            TaskState oldState = _state;
            if (completionCondition)
            {
                _state = TaskState.COMPLETE;
                // invoking `onComplete` event only when the state changes to complete.
                if (oldState != _state)
                    onCompleted.Invoke();
                return true;
            }
            return false;
        }

        private bool CheckDependencies()
        {
            // preformance note: "`for` is about twice faster than `foreach` on generic lists.
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i].CheckSelf() == false)
                {
                    dependencies[i]._state = TaskState.PENDING;
                    Interrupt();
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            if (_state == TaskState.ACTIVE)
            {
                CheckDependencies();
                CheckSelf();
            }
        }
    }
}