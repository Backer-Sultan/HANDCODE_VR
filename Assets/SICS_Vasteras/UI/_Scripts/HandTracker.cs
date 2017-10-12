using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HandTracker : MonoBehaviour
    {
        public Transform pointToFollow;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        bool a;
        private void Update()
        {
            if (a)
            {
                transform.position = pointToFollow.position + positionOffset;
                transform.rotation = pointToFollow.rotation;
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                a = !a;
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                Vector3 p = transform.position - pointToFollow.position;
                print(string.Format("Position offset:\nX: {0}\nY: {1}\nZ: {2}", p.x, p.y, p.z));
                Vector3 q = transform.eulerAngles - pointToFollow.eulerAngles;
                print(string.Format("Rotation offset:\nX: {0}\nY: {1}\nZ: {2}", q.x, q.y, q.z));

            }
        }
    }

}