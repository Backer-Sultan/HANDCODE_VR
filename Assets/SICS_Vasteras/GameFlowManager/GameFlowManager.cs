using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class GameFlowManager : MonoBehaviour
    {
        public SortedList<TaskID, Task> tasks;
        public Machine machine;

        private Task currentTask;
        private bool taskInProgress = false;


        private void InitializeTasks()
        {
            tasks = new SortedList<TaskID, Task>();
            tasks.Add(TaskID.MOVE_CRADLE, new Task()
            {
                ID = TaskID.MOVE_CRADLE,
                controllerObject = machine.mainConsole.gameObject,
                controlledObject = machine.cradle.gameObject,
                completionCondition = machine.cradle.isTargetReached,
            });

            tasks.Add(TaskID.RAISE_ARMS, new Task()
            {
                ID = TaskID.RAISE_ARMS,
                controllerObject = machine.mainConsole.gameObject,
                controlledObject = machine.armRig_Right.gameObject,
                completionCondition = machine.armRig_Right.gameObject,
            });
        }

        public void AssignTasks()
        {
            foreach(Task t in tasks.Values)
            {
                GameObject obj = new GameObject(t.ID.ToString());
                obj.transform.parent = transform;
                Task taskAsComponent = obj.AddComponent<Task>();
                taskAsComponent.ID = t.ID;
                taskAsComponent.controllerObject = t.controllerObject;
                taskAsComponent.controlledObject = t.controlledObject;
                taskAsComponent.completionCondition = t.completionCondition;
            }
        }

        private void Start()
        {
            machine = FindObjectOfType<Machine>();
            if (machine == null)
                Debug.LogError(string.Format("{0}\nGameFlowManager.cs: No machine is found on the scene!", Machine.GetPath(gameObject)));

            InitializeTasks();

            AssignTasks();
        }  







        /*

        public void ManageSwitch()
        {
           foreach(Task tsk in tasks.Values)
            {
                if (tsk.state != TaskState.COMPLETE)
                {
                    currentTask = tsk;
                    currentTask.onInterrupted.AddListener(GetControlBack);
                    currentTask.onCompleted.AddListener(GetControlBack);
                    currentTask.StartTask();
                    taskInProgress = true;
                }
            }
        }

        private void GetControlBack()
        {
            taskInProgress = false;
        }

        private void Update()
        {
            if (!taskInProgress)
            {
                ManageSwitch();
            }
            
        }
        */
    } 
}
