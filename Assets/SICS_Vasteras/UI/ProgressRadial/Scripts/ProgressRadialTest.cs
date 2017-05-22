using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressRadialTest : MonoBehaviour
{
    public ProgressRadial progressRadial;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            progressRadial.Add(0.1f);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            progressRadial.Subtract(0.1f);
    }

}
