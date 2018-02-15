using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class AdhesiveTape : MonoBehaviour
    {
        public bool isSnapped = false;

        public void SetAdhesiveTapeSnapState(bool value)
        {
            isSnapped = value;
        }
    }
}