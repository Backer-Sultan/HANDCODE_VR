/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/
 
using UnityEngine;
using UnityEngine.Events;
using VirtualGrasp;
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
    public UnityEvent onPushed;
    public UnityEvent onReleased;

    private bool _isPressed = false;



    /* methods & coroutines */

    private new void Start()
    {
        base.Start();
        if (ID == ButtonID.NONE)
            Debug.LogError(string.Format("Object: {0}\nMachineButton.cs: ID can't be NONE! please set the button ID!", Machine.GetPath(gameObject)));

        VG_TriggerEvent triggerEvent = GetComponentInChildren<VG_TriggerEvent>();
        if (triggerEvent == null)
            return;
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
