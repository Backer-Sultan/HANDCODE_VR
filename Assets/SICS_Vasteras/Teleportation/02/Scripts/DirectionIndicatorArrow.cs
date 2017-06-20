/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicatorArrow : MonoBehaviour
{
    public float speed = 0.2f;

    private Animator animator;

    private void Start()
    {
        // initialization
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("DirectionIndicatorArrow.cs: component 'Animator' is missing!");
    }

    private void Update()
    {
        transform.localPosition += Vector3.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "DestroyTrigger")
        {
            animator.SetTrigger("FadeOut");
            Destroy(gameObject, 1f);
        }
    }
}
