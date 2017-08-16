using UnityEngine;
using HandCode;

public class ProgressRadialTest : MonoBehaviour
{
    public ProgressRadial progressRadial;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            progressRadial.Add(0.1f);
        if (Input.GetKeyDown(KeyCode.X))
            progressRadial.Subtract(0.1f);
    }

}
