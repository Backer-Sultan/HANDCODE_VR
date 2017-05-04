using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysxSimpleButton : MonoBehaviour {

  public EventTrigger.TriggerEvent onPhysxButtonPressed;
  public EventTrigger.TriggerEvent onPhysxButtonReleased;

	public ConfigurableJoint joint;
	public float triggerThreshold = 0.5f;

	protected float positionOffset = 0;
	[Range(1, 0)] //Tolerance in percent of the joint linear limit
	public float triggerTolerance = 0.15f;

  private bool pressed;
	// Use this for initialization
	void Start () {
    pressed = false;

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
			float pressure = Mathf.Abs(joint.connectedBody.transform.localPosition.y - transform.localPosition.y - positionOffset);

			if (pressure > triggerThreshold)
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
