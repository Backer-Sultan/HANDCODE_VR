using UnityEngine;
using HandCode;

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
