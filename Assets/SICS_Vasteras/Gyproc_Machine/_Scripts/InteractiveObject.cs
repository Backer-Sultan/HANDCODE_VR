using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
   
    public bool isHighlighted { get { return _isHighlighted; } }

    private bool _isHighlighted = false;
    private Highlighter highlighter;
    internal void Start()
    {
        highlighter = GetComponentInChildren<Highlighter>();
        if (highlighter == null)
            Debug.LogError("InteractiveObject.cs: No Highlighter script found in the object's hierarchy!");
    }

    public void Highlight()
    {
        highlighter.enabled = true;
    }

    public void Unhighlight()
    {
        highlighter.enabled = false;
    }

    


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Highlight();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Unhighlight();
    }


}
