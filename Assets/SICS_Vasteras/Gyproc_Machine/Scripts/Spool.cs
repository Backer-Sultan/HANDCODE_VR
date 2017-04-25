using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spool : MonoBehaviour
{
    // fields & properties

    public string ID; // use 'left' for the left Spool and 'right' for the right one. 


    // methods & coroutines

    private void Start()
    {
        if (ID == string.Empty)
            Debug.LogError("Spool.cs: ID can't be empty!");
        else
        {
            ID = ID.Trim().ToLower();
            if (ID != "left" && ID != "right")
                Debug.LogError("Spool.cs: Invalid ID! ID should be `left` or `right`!");
        }
    }
}
