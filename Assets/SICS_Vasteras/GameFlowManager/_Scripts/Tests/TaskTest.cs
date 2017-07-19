using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandCode;

public class TaskTest : MonoBehaviour
{
    public Task task;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            print(task.completionCondition.Invoke());
    }
}