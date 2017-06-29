using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonSuface : MonoBehaviour
{
    private float initalPosition;
    private float maxPushPosition = 0f;
    private new Collider collider;
    private bool isFollowingFinger = false;

    private void Start()
    {
        initalPosition = transform.localPosition.z;
        collider = GetComponent<Collider>();
        if(collider == null)
            print("UI_ButtonSurface: Component 'Collider' is missing!");
        print(initalPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Background")
            isFollowingFinger = false;
        if (other.tag == "Finger")
            isFollowingFinger = true;
    }


    private void OnTriggerStay(Collider other)
    {
        if(transform.localPosition.z <= maxPushPosition&& transform.localPosition.z >= initalPosition)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.InverseTransformVector(other.transform.position).z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Finger")
            isFollowingFinger = false;
    }
}
