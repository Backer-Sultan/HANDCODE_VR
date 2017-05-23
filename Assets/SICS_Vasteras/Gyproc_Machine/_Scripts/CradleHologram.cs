/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CradleHologram : MonoBehaviour
{
    /* fields & properties */

    [Range(0f, 1f)]
    public float speed = 0.5f;
    public Color color1, color2;
    public Cradle cradle;

    private Color lerpedColor;
    private Renderer[] rends;



    /* methods & coroutines */

    private void Start()
    {
        // initialization
        if (!cradle)
            cradle = GameObject.Find("Machine/Cradle").GetComponent<Cradle>();
        if (!cradle)
            Debug.LogError("CradleHologram: Cradle object is missing!");

        rends = GetComponentsInChildren<Renderer>();    
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        lerpedColor = Color.Lerp(color1, color2, Mathf.PingPong(Time.time * 2f, 1));
        foreach (Renderer rend in rends)
            rend.material.color = lerpedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CradleLimitRight")
            transform.localPosition = cradle.transform.localPosition;
    }

    private void OnEnable()
    {
        if (cradle)
            transform.localPosition = cradle.transform.localPosition;
    }
}
