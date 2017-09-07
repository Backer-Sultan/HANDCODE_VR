using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace HandCode
{
    public class PlayerTracker : MonoBehaviour
    {
        public Transform teleportPos4;
        public Transform teleportPos2;
        public UnityEvent onTeleportedToPosition4;
        public UnityEvent onTeleportedToPosition2;
        public bool isTeleportedPos4 { get { return _isTeleportedPos4; } }
        public bool isTeleportedPos2 { get { return _isTeleportedPos2; } }

        private bool _isTeleportedPos4;
        private bool _isTeleportedPos2;



        private void Start()
        {
            teleportPos4 = GameObject.Find("DestinationPoint_PaperGuide/Destination_Location").transform;
            teleportPos2 = GameObject.Find("DestinationPoint_ArmConsole/Destination_Location").transform;
        }

        public void TrackPlayer()
        {
            Transform t = VRTK_DeviceFinder.PlayAreaTransform();
            if (Vector3.Distance(t.position, teleportPos4.position) <= 1f)
            {
                _isTeleportedPos4 = true;
                onTeleportedToPosition4.Invoke();
            }
            else
                _isTeleportedPos4 = false;

            if (Vector3.Distance(t.position, teleportPos2.position) <= 1f)
            {
                _isTeleportedPos2 = true;
                onTeleportedToPosition2.Invoke();
            }
            else
                _isTeleportedPos2 = false;
        }

    }
}
