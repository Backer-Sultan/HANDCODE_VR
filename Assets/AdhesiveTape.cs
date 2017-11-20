using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class AdhesiveTape : MonoBehaviour
    {
        public bool isSapped = false;

        public void SetAdhesiveTapeSnapState(bool value)
        {
            isSapped = value;
        }
    }
}