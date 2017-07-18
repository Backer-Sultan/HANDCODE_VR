using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sometests : MonoBehaviour
{
    bool shouldStop = false;
    string routineName = "Routine";
    

    public IEnumerator CallerRoutine()

    {
        print("caller start...");
        yield return StartCoroutine(routineName);
        print("caller end...");
    }




    public IEnumerator Routine()
    {
        print("Started...");
        for (int i = 0; i < 11; i++)
        {
            print(i);
            //if (i == 3)
            //    shouldStop = true;
            yield return new WaitForSeconds(0.1f);
        }
        print("Ended...");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            StartCoroutine(CallerRoutine());
        if (shouldStop)
        {
            StopCoroutine(routineName);
            shouldStop = false;
        }
    }





}
