/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using UnityEngine;


public class Highlighter : MonoBehaviour
{
    /* fields & properties */

    public Color highlightColor = Color.blue;

    private Renderer[] renderers;
    private Color emissionColor;



    /* methods & coroutines */

    private void OnEnable()
    {
        renderers = GetComponentsInChildren<Renderer>();
        emissionColor = Color.blue;
        StartCoroutine(HighlightRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        foreach (Renderer rend in renderers)
            rend.material.SetColor("_EmissionColor", Color.black);
    }

    private IEnumerator HighlightRoutine()
    {
        float emission;
        Color finalColor;
        while (true)
        {
            emission = Mathf.PingPong(Time.time, 1f);
            finalColor = highlightColor * Mathf.LinearToGammaSpace(emission);
            foreach (Renderer rend in renderers)
                rend.material.SetColor("_EmissionColor", finalColor);
            yield return null;
        }
    }
}
