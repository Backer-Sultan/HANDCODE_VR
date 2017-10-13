using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class TaskHint : MonoBehaviour
    {
        public Highlighter controllerHighlighter; // the position/console that the operator interacts with the machine from.
        public Highlighter controlledHighlighter; // the object or the machine part to be controlled.
        public Hologram instructionHologram;

        [Header("Voiceover Clips")]
        public AudioClip instructionAudio;
        public AudioClip explanationAudio;
        public AudioClip ControllerAudio;
        public AudioClip ControlledAudio;
    } 
}
