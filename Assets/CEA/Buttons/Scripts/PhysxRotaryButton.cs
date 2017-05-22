using UnityEngine;
using UnityEngine.EventSystems;

public class PhysxRotaryButton : MonoBehaviour {

	public HingeJoint joint;
	[Range (2,12)]
	public int numberOfPositions = 2;
	[Range(1, 360)]
	public int rotationAmplitude = 90;

	private int currentPositionId = 0;
	public int CurrentPosition
	{
		get { return this.currentPositionId; }
	}

	private JointSpring spring;

	public EventTrigger.TriggerEvent onPhysxButtonSwitch;

	// Use this for initialization
	void Start () {

		if (joint != null)
		{
			JointLimits limits = new JointLimits();
			limits.min = -rotationAmplitude / 2.0f;
			limits.max = rotationAmplitude / 2.0f;
			limits.bounciness = 0;
			limits.bounceMinVelocity = 0.2f;
			limits.contactDistance = 0;
			joint.limits = limits;

			spring = new JointSpring();
			spring.damper = 0.002f;
			spring.spring = 0.02f;
			spring.targetPosition = -rotationAmplitude / 2.0f;
			joint.spring = spring;

			//transform.localRotation = Quaternion.AngleAxis(spring.targetPosition, Vector3.up);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (joint != null)
		{
			float buttonAngle = transform.localRotation.eulerAngles.y - joint.connectedBody.transform.localRotation.eulerAngles.y;
			if (buttonAngle > 180)
				buttonAngle -=360 ;

			//Debug.Log(buttonAngle);
			float stepNotch = rotationAmplitude / (float)(numberOfPositions-1);
			for (int i = 0;i< numberOfPositions;i++)
			{
				if (i != currentPositionId)
				{
					float notchAngle = -rotationAmplitude / 2.0f + i * stepNotch;
					if (Mathf.Abs(buttonAngle - notchAngle) < stepNotch / 2.0f)
					{
						spring.targetPosition = notchAngle;
						joint.spring = spring;
						currentPositionId = i;
						BaseEventData eventData = new BaseEventData(EventSystem.current);
						onPhysxButtonSwitch.Invoke(eventData);
					}
				}
			}
		}
	}
}
