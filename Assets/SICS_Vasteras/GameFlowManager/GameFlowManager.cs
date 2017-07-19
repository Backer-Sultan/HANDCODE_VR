/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/
 
 using System.Collections.Generic;
using UnityEngine;
using System;

namespace HandCode
{
    public class GameFlowManager : MonoBehaviour
    {
        /* fields & properties */

        public SortedList<TaskID, Task> tasks;
        public SortedList<TaskID, Func<bool>> completionConditions;
        public Machine machine;
        [SerializeField]
        private Task currentTask;
        private bool taskInProgress = false;
        private bool isInitDone = false;



        /* methods & coroutines */
        
         /// <summary>
         /// The completion condition for every task is defined here.
         /// There should be an entry for every task assigned in the inspector.
         /// </summary>
        private void InitializeCompletionConditions()
        {
            completionConditions = new SortedList<TaskID, Func<bool>>();
            completionConditions.Add(TaskID.MOVE_CRADLE_RIGHT, () => { return machine.cradle.isTargetReached; });
            completionConditions.Add(TaskID.TEST, () => { return false; });
            completionConditions.Add(TaskID.RAISE_ARMS, () => { return machine.armRig_Right.mainHandle.rotation.x > 10f; });
        }

        public void ManageSwitch()
        {
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

        private void Start()
        {
            machine = FindObjectOfType<Machine>();
            if (machine == null)
                Debug.LogError(string.Format("{0}\nGameFlowManager.cs: No machine is found on the scene!", Machine.GetPath(gameObject)));
            InitializeCompletionConditions();
            InitializeTasks();
            foreach(Task t in tasks.Values)
            {
                print(t.ID);
            }
            isInitDone = true;
        }

        private void InitializeTasks()
        {
            tasks = new SortedList<TaskID, Task>();
            Task[] taskArray = FindObjectsOfType<Task>();
            foreach(Task task in taskArray)
            {
                if (completionConditions[task.ID] != null)
                    task.completionCondition = completionConditions[task.ID];
                else
                {
                    Debug.LogError(string.Format("{0}\nGameFlowManager.cs: Task initialization failed!\nNo completion condition is not set for the task`{1}`!", Machine.GetPath(gameObject), task.ID.ToString()));
                    return;
                }
                tasks.Add(task.ID, task);
            }
        }

        private void GetControlBack()
        {
            taskInProgress = false;
        }

        private void Update()
        {
            if (isInitDone && !taskInProgress)
            {
                ManageSwitch();
            }
            
        }
    } 
}
