using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HandTracker : MonoBehaviour
    {
        public Transform pointToFollow;

        private void Update()
        {
            transform.position = pointToFollow.position;
            transform.rotation = pointToFollow.rotation;
        }
    }

}