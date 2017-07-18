using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HintSystem : MonoBehaviour
    {
        public bool active { get { return _active; } }

        private bool _active = false;

        public void Activate()
        {
            //plays a voiceover guiding the player to use the hint-system.
        }
        public void Deactivate()
        {
            
        }
    } 
}
