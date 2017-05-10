/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private bool isPressed = false;

    private void Start()
    {
        if (ID == ButtonID.NONE)
            Debug.LogError("MachineButton.cs: ID can't be NONE! please set the button ID!");
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name + "Entered!");
        if (other.tag == "Hand")
        {
            isPressed = true;
            onPressed.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print(other.name + "Leaved!");
        if (other.tag == "Hand")
        {
            isPressed = false;
            onReleased.Invoke();
        }
    }

    /* methods & coroutines */


}
