using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonSuface : MonoBehaviour
{
    public float speed = 0.1f;

    private float initalPosition;
    private float maxPushPosition = 0f;
    private new Collider collider;
    private  Rigidbody rigid;
    private bool isFollowingFinger = false;

    private void Start()
    {
        initalPosition = transform.localPosition.z;
        collider = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
        if (collider == null)
            print("UI_ButtonSurface: Component 'Collider' is missing!");
    }

    // reaching max push
    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter!");
        if (other.name == "Background")
            rigid.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("OnCollisionEnter!"); 
        StopAllCoroutines();
    }
    private void OnCollisionExit(Collision collision)
    {
        print("OnCollisionExit!");
        if(collider.gameObject.tag == "Finger")
        {
            rigid.isKinematic = false;
            StartCoroutine(MoveSurfaceBackRoutine());
        }
    }

    private IEnumerator MoveSurfaceBackRoutine()
    {
        while(transform.localPosition.z > initalPosition)
        {
            transform.Translate(Vector3.back * Time.deltaTime, Space.Self);
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        print("OnTriggerStay!");
        if(isFollowingFinger)
        {
            Vector3 localPosition = transform.InverseTransformPoint(other.transform.position);
            localPosition.Set(0f, 0f, localPosition.z);
            transform.localPosition = localPosition;

            Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
            localVelocity.x = 0f;
            localVelocity.y = 0f;

            GetComponent<Rigidbody>().velocity = transform.TransformDirection(localVelocity);
        }
    }


}