using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Highlighter))]
public class InteractiveObject : MonoBehaviour
{
   
    public bool isHighlighted { get { return _isHighlighted; } }
    
    private bool _isHighlighted = false;
    private Highlighter highlighter;


    internal void Start()
    {
        highlighter = GetComponent<Highlighter>();
    }

    public void Highlight()
    {
        highlighter.enabled = true;
        _isHighlighted = true;
    }

    public void Unhighlight()
    {
        highlighter.enabled = false;
        _isHighlighted = false;
    }
}
