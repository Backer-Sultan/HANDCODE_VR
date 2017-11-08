using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class AdhesiveTaopHandler : MonoBehaviour
    {
        public bool isSapped = false;

        public void SetAdhesiveTapeSnapState(bool value)
        {
            isSapped = value;
        }
    }
}