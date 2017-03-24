using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysxButtonEvent : MonoBehaviour {

  public EventTrigger.TriggerEvent onPhysxButtonPressed;
  public EventTrigger.TriggerEvent onPhysxButtonReleased;

  public float triggerThreshold = 0.25f;

  private float realeasedPositionY;
  private bool pressed;
	// Use this for initialization
	void Start () {
    pressed = false;
    realeasedPositionY = transform.localPosition.y;
  }
	
	// Update is called once per frame
	void Update () {
    float pressure = Mathf.Abs(realeasedPositionY - transform.localPosition.y);

    if (pressure > triggerThreshold)
    {
      if (!pressed)
      {
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        onPhysxButtonPressed.Invoke(eventData);
      }
      pressed = true;
    }
    else
    {
      if (pressed)
      {
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        onPhysxButtonReleased.Invoke(eventData);
      }
      pressed = false;
    }
  }
}
