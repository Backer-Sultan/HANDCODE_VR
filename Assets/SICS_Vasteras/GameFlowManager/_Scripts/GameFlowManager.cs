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

        public SortedList<TaskID, Task> tasks;
        public SortedList<TaskID, Func<bool>> completionConditions;
        [HideInInspector]
        public Task currentTask;

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
            completionConditions.Add(TaskID.OPEN_ARMS, () => machine.armRig_Right.isArmsOpen);
            completionConditions.Add(TaskID.RAISE_ARMS, () => machine.armRig_Right.isArmsRaised);
            completionConditions.Add(TaskID.MOVE_SPOOL, () => machine.spool_Right.isTargetReached);
            completionConditions.Add(TaskID.TEST, () => false);
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
            if (currentTask != null)
            {
                currentTask.onInterrupted.RemoveListener(GetControlBack);
                currentTask.onCompleted.RemoveListener(GetControlBack);
            }

            foreach (Task tsk in tasks.Values)
            {
                if (tsk.state != TaskState.COMPLETE)
                {
                    currentTask = tsk;
                    currentTask.onInterrupted.AddListener(GetControlBack);
                    currentTask.onCompleted.AddListener(GetControlBack);
                    currentTask.StartTask();
                    taskInProgress = true;
                    break;
                }
            }
        }

        private void GetControlBack()
        {
            taskInProgress = false;
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
