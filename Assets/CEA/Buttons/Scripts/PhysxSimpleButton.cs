using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysxSimpleButton : MonoBehaviour {

  public EventTrigger.TriggerEvent onPhysxButtonPressed;
  public EventTrigger.TriggerEvent onPhysxButtonReleased;

	public Transform trigger;

  private bool pressed;
	// Use this for initialization
	void Start () {
    pressed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (trigger != null)
		{
			float pressure = transform.localPosition.y - trigger.localPosition.y;

			if (pressure < 0)
			{
				if (!pressed)
				{
					BaseEventData eventData = new BaseEventData(EventSystem.current);
					onPhysxButtonPressed.Invoke(eventData);
					pressed = true;
				}
			}
			else
			{
				if (pressed)
				{
					BaseEventData eventData = new BaseEventData(EventSystem.current);
					onPhysxButtonReleased.Invoke(eventData);
					pressed = false;
				}
			}
		}
	}
}
