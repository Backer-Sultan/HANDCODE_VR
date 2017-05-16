using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysxToggleButton : PhysxSimpleButton
{

	public enum PhysxToggleButtonState { Released,PressedToEnable, Pressed, PressedToDisable }
	private PhysxToggleButtonState state;
	private ConfigurableJoint joint;

	[Range(0, 1f)]
	public float lockPosition = 0.5f;
	private float offsetTriggerButton;

	// Use this for initialization
	void Start () {
		state = PhysxToggleButtonState.Released;
		joint = GetComponent<ConfigurableJoint>();
		offsetTriggerButton = transform.localPosition.y - trigger.localPosition.y;
		Debug.Log(offsetTriggerButton);

	}
	
	// Update is called once per frame
	void Update () {

		if (trigger != null)
		{
			float pressure = transform.localPosition.y - trigger.localPosition.y;

			if (state == PhysxToggleButtonState.Released)
			{
				if (pressure <0)
				{
					BaseEventData eventData = new BaseEventData(EventSystem.current);
					onPhysxButtonPressed.Invoke(eventData);
					state = PhysxToggleButtonState.PressedToEnable;
					joint.connectedAnchor = new Vector3(0,0, lockPosition * offsetTriggerButton);
				}
			}
			else if (state == PhysxToggleButtonState.PressedToEnable)
			{
				if (pressure > 0)
				{
					state = PhysxToggleButtonState.Pressed;
				}
			}
			else if (state == PhysxToggleButtonState.Pressed)
			{
				if (pressure < 0)
				{
					BaseEventData eventData = new BaseEventData(EventSystem.current);
					onPhysxButtonReleased.Invoke(eventData);
					state = PhysxToggleButtonState.PressedToDisable;
				}
			}
			else
			{
				if (pressure > 0)
				{
					state = PhysxToggleButtonState.Released;
					joint.connectedAnchor = new Vector3(0, 0, offsetTriggerButton);
				}
			}
		}
	}
}
