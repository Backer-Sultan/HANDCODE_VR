/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HandCode
{
    public class GameFlowManager : MonoBehaviour
    {
        /* fields & properties */
        public Task currentTask { get { return _currentTask; } }
        public float completionPercentage { get { return _completionPercentage; } }
        public UnityEvent onCurrentTaskChanged;

        private SortedList<TaskID, Task> tasks;
        private SortedList<TaskID, Func<bool>> completionConditions;
        private Task _currentTask;
        private Machine machine;
        private bool taskInProgress = false;
        private float _completionPercentage;
        private bool allTasksComplete = false;



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
            completionConditions.Add(TaskID.RAISE_ARMS, () => machine.armRig_Right.isArmsUp);
            completionConditions.Add(TaskID.MOVE_SPOOL, () => machine.spool_Right.isTargetReached);
            completionConditions.Add(TaskID.LOWER_ARMS, () => machine.armRig_Right.isArmsBelowZero);
            completionConditions.Add(TaskID.TELEPORT_POS_4, () => FindObjectOfType<PlayerTracker>().isTeleportedPos4);
            completionConditions.Add(TaskID.TELEPORT_POS_2, () => FindObjectOfType<PlayerTracker>().isTeleportedPos2);
            completionConditions.Add(TaskID.HANDLE_SPOOL, () => machine.spool_Right.isHandled);
            completionConditions.Add(TaskID.RAISE_ARMS_WITH_SPOOL, () => machine.armRig_Right.isArmsUp); // is set according to the joint max limit.
            completionConditions.Add(TaskID.LOWER_ARMS_WITH_SPOOL, () => machine.armRig_Right.isArmsDown);
            completionConditions.Add(TaskID.TELEPORT_POS_3, () => FindObjectOfType<PlayerTracker>().isTeleportedPos3);
            completionConditions.Add(TaskID.LOWER_PINSHER, () => machine.cradle.isPinsherLow);
            completionConditions.Add(TaskID.DISCONNECT_BREAK, () => !machine.cradle.isBreakApplied);
            completionConditions.Add(TaskID.CUT_PAPER, () => machine.paperCut.isPaperCut);
            completionConditions.Add(TaskID.MOUNT_MIDDLE_TAPE, () => machine.cradle.tapePiece_middle.isMounted);
            completionConditions.Add(TaskID.MOUNT_RIGHT_TAPE, () => machine.cradle.tapePiece_right.isMounted);
            completionConditions.Add(TaskID.MOUNT_LEFT_TAPE, () => machine.cradle.tapePiece_left.isMounted);

        }

        private void InitializeTasks()
        {
            InitializeCompletionConditions();

            tasks = new SortedList<TaskID, Task>();
            Task[] taskArray = FindObjectsOfType<Task>();
            foreach (Task task in taskArray)
            {
                if (completionConditions.Keys.Contains(task.ID) && completionConditions[task.ID] != null)
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
            UpdateCompletionPercentage();

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
            onCurrentTaskChanged.Invoke();

            // at this point, if `taskInProgress == false` that mean all tasks are complete
            if (!taskInProgress)
            {
                allTasksComplete = true;
            }
        }

        private void GetControlBack()
        {
            taskInProgress = false;
        }

        private void UpdateCompletionPercentage()
        {
            int numCompleted = 0;
            foreach (Task task in tasks.Values)
                if (task.state == TaskState.COMPLETE)
                    numCompleted++;
            _completionPercentage = (float)numCompleted / tasks.Count;
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
            if (!taskInProgress && !allTasksComplete)
            {
                ManageSwitch();
            }
        }
        
    }
}
