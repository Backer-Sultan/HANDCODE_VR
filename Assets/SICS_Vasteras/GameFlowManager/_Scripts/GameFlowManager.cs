/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class GameFlowManager : MonoBehaviour
    {
        /* fields & properties */
        public Task currentTask { get { return _currentTask; } }

        private SortedList<TaskID, Task> tasks;
        private SortedList<TaskID, Func<bool>> completionConditions;
        private Task _currentTask;
        private Machine machine;
        private bool taskInProgress = false;



        /* methods & coroutines */

        /// <summary>
        /// The completion condition for every task is defined here.
        /// There should be an entry for every task assigned in the inspector.
        /// </summary>
        private void InitializeCompletionConditions()
        {
            completionConditions = new SortedList<TaskID, Func<bool>>();
            completionConditions.Add(TaskID.MOVE_CRADLE_RIGHT, () => machine.cradle.isTargetReached);
            completionConditions.Add(TaskID.TEST, () => false);
            completionConditions.Add(TaskID.RAISE_ARMS, () => machine.armRig_Right.mainHandle.rotation.x > 10f);
        }

        private void InitializeTasks()
        {
            InitializeCompletionConditions();

            tasks = new SortedList<TaskID, Task>();
            Task[] taskArray = FindObjectsOfType<Task>();
            foreach (Task task in taskArray)
            {
                if (completionConditions[task.ID] != null)
                    task.completionCondition = completionConditions[task.ID];
                else
                {
                    Debug.LogError(string.Format("{0}\nGameFlowManager.cs: Task initialization failed!\nCompletion condition is not set for task `{1}`!", Machine.GetPath(gameObject), task.ID.ToString()));
                    return;
                }
                tasks.Add(task.ID, task);
            }
        }

        public void ManageSwitch()
        {
            if (_currentTask != null)
            {
                _currentTask.onInterrupted.RemoveListener(GetControlBack);
                _currentTask.onCompleted.RemoveListener(GetControlBack);
            }

            foreach (Task tsk in tasks.Values)
            {
                if (tsk.state != TaskState.COMPLETE)
                {
                    _currentTask = tsk;
                    _currentTask.onInterrupted.AddListener(GetControlBack);
                    _currentTask.onCompleted.AddListener(GetControlBack);
                    _currentTask.StartTask();
                    taskInProgress = true;
                    break;
                }
            }
        }

        private void GetControlBack()
        {
            taskInProgress = false;
        }

        public int GetTasksCount()
        {
            return tasks.Count;
        }

        public float GetCompletionPercentage()
        {
            int numCompleted = 0;
            foreach (Task task in tasks.Values)
                if (task.state == TaskState.COMPLETE)
                    numCompleted++;
            return (float)numCompleted / tasks.Count;
        }

        private void Start()
        {
            machine = FindObjectOfType<Machine>();
            if (machine == null)
                Debug.LogError(string.Format("{0}\nGameFlowManager.cs: No machine is found on the scene!", Machine.GetPath(gameObject)));
            InitializeTasks();
        }

        private void Update()
        {
            if (!taskInProgress)
            {
                ManageSwitch();
            }
        }
    }
}
