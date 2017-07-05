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
        MAINCONSOLE_CRADLE_MOVE_TO_LEFT,
        MAINCONSOLE_CRADLE_MOVE_TO_RIGHT,
        MAINCONSOLE_CRADLE_STOP,
        MAINCONSOLE_ARMS_CLOSE,
        MAINCONSOLE_ARMS_OPEN,
        MAINCONSOLE_ARMS_MOVE_RIGHT,
        MAINCONSOLE_ARMS_MOVE_LEFT,
        MAINCONSOLE_ARMRIG_ROTATE_UP,
        MAINCONSOLE_ARMRIG_ROTATE_DOWN,
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
            Debug.LogError("MachineButton.cs: ID can't be NONE! please set the button ID!");

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
