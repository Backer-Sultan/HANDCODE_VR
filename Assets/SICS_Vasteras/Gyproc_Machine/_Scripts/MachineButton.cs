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
    public class MachineButton : InteractiveObject
    {
        /* fields & properties */

        public enum ButtonID
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
        }
        public ButtonID ID;
        public bool isPressed { get { return isPressed; } }
        [Header("Events")]
        public UnityEvent onPushed;
        public UnityEvent onReleased;

        private bool _isPressed = false;



        /* methods & coroutines */

        private new void Start()
        {
            base.Start();
            if (ID == ButtonID.NONE)
                Debug.LogError(string.Format("{0}\nMachineButton.cs: ID is not assigned!", Machine.GetPath(gameObject)));

            VG_TriggerEvent triggerEvent = GetComponentInChildren<VG_TriggerEvent>();
            if (triggerEvent == null)
                Debug.LogWarning(string.Format("{0}\nMachineButton.cs: Script `VG_TriggerEvent` is not found in the hierarchy!", Machine.GetPath(gameObject)));
        }

        public void OnPushed()
        {
            onPushed.Invoke();
        }

        public void OnReleased()
        {
            onReleased.Invoke();
        }
    } 
}
