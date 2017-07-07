using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
   
    public bool isHighlighted { get { return _isHighlighted; } }
    public Renderer highLightedObject;
    public new Light light;

    private bool _isHighlighted = false;
    private Color defColor;
    private Color minAlphaColor;
    private Color maxAlphaColor;
    public bool lerpAphaRoutineIsRunning = false;

    internal void Start()
    {
        if (light == null)
            Debug.LogError("InteractableObject.cs: No light component is found on this object!");

        if (highLightedObject == null)
        {
            Debug.LogError("InteractableObject.cs: 'Highlighted Object is not specified!");
            return;
        }
        defColor = highLightedObject.material.color;
        minAlphaColor = new Color(defColor.r, defColor.g, defColor.b, 0.1f);
        maxAlphaColor = new Color(defColor.r, defColor.g, defColor.b, 0.9f);
    }

    public void Highlight()
    {
        if (!lerpAphaRoutineIsRunning)
        {
            StartCoroutine(LerpAlpha());
            _isHighlighted = true;
            light.enabled = true;
        }
    }

    public void Unhighlight()
    {
        print("Unhighlighted!");
        StopAllCoroutines();
        lerpAphaRoutineIsRunning = false;
        highLightedObject.material.color = defColor;
        _isHighlighted = false;
        light.enabled = false;
    }

    private IEnumerator LerpAlpha()
    {
        print("LerpAlpha Started!");
        lerpAphaRoutineIsRunning = true;
        while (true)
        {
            highLightedObject.material.color = Color.Lerp(minAlphaColor, maxAlphaColor, Mathf.PingPong(Time.time, 1));
            Color color = Color.Lerp(minAlphaColor, maxAlphaColor, Mathf.PingPong(Time.time, 1));
            yield return null;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Highlight();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Unhighlight();
    }


}
