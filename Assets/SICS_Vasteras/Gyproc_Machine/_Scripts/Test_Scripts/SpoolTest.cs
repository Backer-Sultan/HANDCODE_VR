using UnityEngine;
using HandCode;

public class SpoolTest : MonoBehaviour
{
    private Spool spool;

	// Use this for initialization
	void Start ()
    {
        spool = GameObject.Find("Spool_Right").GetComponent<Spool>();
        if (!spool)
            Debug.LogError("Can't find Spool.cs!");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.S))
            spool.MoveToTarget();
        if (Input.GetKeyUp(KeyCode.S))
            spool.Stop();
	}
}
