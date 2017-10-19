using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class HintSystemTest : MonoBehaviour
    {
        HintSystem_VG hintSystem;
        private bool audioState = false;

        private void Start()
        {
            hintSystem = GetComponent<HintSystem_VG>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                hintSystem.instruction.SetActiveAnimation(true);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                hintSystem.instruction.PlaySlideAnimation ();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                audioState = !audioState;
                hintSystem.instruction.SetActiveAnimation(audioState);
            }
        }
    }

}