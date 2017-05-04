using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButtonColor : MonoBehaviour {

	public Material[] materials;

	public void SwitchButtonMaterial(PhysxRotaryButton button)
	{
		GetComponent<Renderer>().material = materials[button.CurrentPosition];
	}
}
