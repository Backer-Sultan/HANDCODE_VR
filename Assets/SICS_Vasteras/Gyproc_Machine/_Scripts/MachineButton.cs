/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK.UnityEventHelper;

public class MachineButton : MonoBehaviour
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
    public UnityEvent onPressed;
    public UnityEvent onReleased;
    public UnityAction onHighlighted;

    private bool isPressed = false;



    /* methods & coroutines */

    private void Start()
    {
        if (ID == ButtonID.NONE)
            Debug.LogError("MachineButton.cs: ID can't be NONE! please set the button ID!");
    }

    /************************************************************************************
     * Here comes the Gleechi's implementatoin of pushing, releasing and highlighting.
     * Invoking onPressed, onReleased and onHighlighted will do the work for us.
     ************************************************************************************/
}
