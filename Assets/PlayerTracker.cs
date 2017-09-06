using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace HandCode
{
    public class PlayerTracker : MonoBehaviour
    {
        public Transform p4TeleportPos;
        public bool teleportedToPos4;
        public UnityEvent onTeleportedToPosition4;

        private void Start()
        {
            p4TeleportPos = GameObject.Find("DestinationPoint_PaperGuide/Destination_Location").transform;
        }

        public void TrackPlayer()
        {
            Transform t = VRTK_DeviceFinder.PlayAreaTransform();
            if (Vector3.Distance(t.position, p4TeleportPos.position) <= 1f)
            {
                teleportedToPos4 = true;
                onTeleportedToPosition4.Invoke();
            }
            else
                teleportedToPos4 = false;
        }

    }
}
