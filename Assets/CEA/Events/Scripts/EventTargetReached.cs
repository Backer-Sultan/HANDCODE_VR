
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTargetReached : MonoBehaviour {

	public EventTrigger.TriggerEvent onTargetReached;
	public EventTrigger.TriggerEvent onTargetLeft;

	void OnTriggerEnter(Collider other)
	{
		BaseEventData eventData = new BaseEventData(EventSystem.current);
		onTargetReached.Invoke(eventData);
	}

	void OnTriggerExit(Collider other)
	{
		BaseEventData eventData = new BaseEventData(EventSystem.current);
		onTargetLeft.Invoke(eventData);
	}
}
