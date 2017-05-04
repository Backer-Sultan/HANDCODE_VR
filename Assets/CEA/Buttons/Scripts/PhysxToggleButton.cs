using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysxToggleButton : PhysxSimpleButton
{

	public enum PhysxToggleButtonState { Released,PressedToEnable, Pressed, PressedToDisable }
	private PhysxToggleButtonState state;

	// Use this for initialization
	void Start () {
		state = PhysxToggleButtonState.Released;

		if (joint != null)
		{
			SoftJointLimit limit = new SoftJointLimit();
			limit.bounciness = 0;
			limit.contactDistance = 0;
			limit.limit = triggerThreshold + triggerTolerance * triggerThreshold;
			joint.linearLimit = limit;

			positionOffset = joint.connectedBody.transform.localPosition.y - transform.localPosition.y;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (joint != null)
		{
			float pressure = Mathf.Abs(joint.connectedBody.transform.localPosition.y - transform.localPosition.y);

			if (state == PhysxToggleButtonState.Released)
			{
				if (pressure > triggerThreshold)
				{
					BaseEventData eventData = new BaseEventData(EventSystem.current);
					onPhysxButtonPressed.Invoke(eventData);
					state = PhysxToggleButtonState.PressedToEnable;
				}
			}
			else if (state == PhysxToggleButtonState.PressedToEnable)
			{
				if (pressure < triggerThreshold)
				{
					joint.connectedBody.transform.position -= new Vector3(0, triggerThreshold / 2, 0);
					state = PhysxToggleButtonState.Pressed;
				}
			}
			else if (state == PhysxToggleButtonState.Pressed)
			{
				if (pressure > triggerThreshold / 2)
				{
					joint.connectedBody.transform.position += new Vector3(0, triggerThreshold / 2, 0);
					BaseEventData eventData = new BaseEventData(EventSystem.current);
					onPhysxButtonReleased.Invoke(eventData);
					state = PhysxToggleButtonState.PressedToDisable;
				}
			}
			else
			{
				if (pressure < triggerTolerance * joint.linearLimit.limit)
					state = PhysxToggleButtonState.Released;
			}
		}
	}
}
