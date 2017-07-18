/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;
using UnityEngine.Events;
using VirtualGrasp;

namespace HandCode
{
    public enum MachineButtonID
    {
        NONE,
        CRADLE_MOVE_LEFT,
        CRADLE_MOVE_RIGHT,
        CRADLE_STOP,
        ARMS_CLOSE,
        ARMS_OPEN,
        ARMS_MOVE_RIGHT,
        ARMS_MOVE_LEFT,
        ARMRIG_ROTATE_UP,
        ARMRIG_ROTATE_DOWN,
        BREAK_TOGGLE,
        DOUBLE_COMMAND,
    }


    public class MachineButton : InteractiveObject
    {
        /* fields & properties */
        
        public MachineButtonID ID;
        public bool isPushed { get { return _isPushed; } }
        [Header("Events")]
        public UnityEvent onPushed;
        public UnityEvent onReleased;

        private bool _isPushed = false;
        private Machine machine;


        /* methods & coroutines */

        private new void Start()
        {
            // Initialization
            machine = FindObjectOfType<Machine>();
            if (machine == null)
                Debug.LogError(string.Format("{0}\nMachineButton.cs: No `Machine` script is found!"));

            base.Start();
            if (ID == MachineButtonID.NONE)
                Debug.LogError(string.Format("{0}\nMachineButton.cs: ID is not assigned!", Machine.GetPath(gameObject)));

            VG_TriggerEvent triggerEvent = GetComponentInChildren<VG_TriggerEvent>();
            if (triggerEvent == null)
                Debug.LogWarning(string.Format("{0}\nMachineButton.cs: Script `VG_TriggerEvent` is not found in the hierarchy!", Machine.GetPath(gameObject)));
        }

        public void OnPushed()
        {
            // pushing the button only if the double command button is pushed.
            if (machine.isButtonPushActive && !_isPushed)
            {
                _isPushed = true;
                onPushed.Invoke();
            }
        }

        public void OnReleased()
        {
            if(_isPushed)
            {
                _isPushed = false;
                onReleased.Invoke();
            }
        }

        private void Update()
        {
            if (!machine.isActiveAndEnabled)
                OnReleased();
        }
    } 
}
