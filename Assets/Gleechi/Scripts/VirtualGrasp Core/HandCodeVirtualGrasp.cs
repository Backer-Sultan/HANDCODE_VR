// Copyright (C) 2014-2017 Gleechi AB. All rights reserved.

//#define USE_STEAM_VR

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // for Text UI
using UnityEngine.VR; // for VR Settings
using VirtualGrasp;

[RequireComponent (typeof (VG_SensorConfiguration))]
public class HandCodeVirtualGrasp : MonoBehaviour
{
	// Check this if we want to use VR mode
	public bool useVR = true;
	// A shader for object highlighting. We have to add this here in order to bring the shader into a build.
	public Shader shader = null;

	// We have a scene with one specific avatar
	private int avatarID = 1;
	// An array of the current VirtualGrasp hand status (since we use two hands, we have two elements)
	private VG_HandStatus[] current = new VG_HandStatus[2];
	// An array of the former VirtualGrasp hand status (since we use two hands, we have two elements)
	private VG_HandStatus[] former  = new VG_HandStatus[2];
	// A reference to an object selector
	private HandCodeObjectSelection pSelector = null;
	// A reference to an sensor configuration
	private VG_SensorConfiguration pSensorMapper = null;

	void OnApplicationQuit()
	{
		VG_Controller.Release ();
	}

	void Awake()
	{
#if USE_STEAM_VR
		if (useVR) VRSettings.LoadDeviceByName ("OpenVR");
#endif
	}

    void Start()
	{
		// First, setup the cameras dependent on the play mode (VR or non-VR)
		GameObject cam2D = GameObject.Find("Camera (noVR)");
		GameObject cam3D = GameObject.Find("Camera (head)");

		VRSettings.enabled = useVR;

#if USE_STEAM_VR
		GameObject viveRig = GameObject.Find("[CameraRig]");
		if (useVR)
		{
			if (cam2D) cam2D.gameObject.SetActive(false);
			if (cam3D) cam3D.tag = "MainCamera";
		}
		else
		{
			if (viveRig != null)
			{
				viveRig.GetComponent<SteamVR_ControllerManager>().enabled = false;
				viveRig.GetComponent<SteamVR_PlayArea>().enabled = false;
				viveRig.GetComponent<MeshRenderer>().enabled = false;
			}
#else
		{
#endif
			if (cam3D) cam3D.gameObject.SetActive(false);
			if (cam2D) cam2D.tag = "MainCamera";   
		}


		// First, initialize the VirtualGrasp library from a specific folder.
		// Note that those folders differ dependent on configuration and editor mode.
		string libraryDirectory = "";
#if UNITY_EDITOR_64
		libraryDirectory = Application.dataPath + "/Gleechi/Plugins/x86_64/";
#else

#if UNITY_EDITOR
		libraryDirectory = Application.dataPath + "/Plugins/x86/";
#else
		libraryDirectory = Application.dataPath + "/Plugins/";
#endif
#endif

        // Check if we can access the VirtualGrasp interface
        VG_Controller.Initialize(libraryDirectory);
        if (!VG_Controller.IsEnabled())
		{
			Debug.LogError ("Failed to initialize VirtualGrasp plugin.");
			enabled = false;
			return;
		}

        // Hand the interface over to the ObjectSelection
		pSelector = new HandCodeObjectSelection (shader);

		pSensorMapper = GetComponent<VG_SensorConfiguration> ();
		// Register the sensor configuration to the VirtualGrasp library and create the HandStatus arrays
		if (pSensorMapper != null) 
		{
			pSensorMapper.Register ();

            // Temporary: set this to true to have the alternative button pushing interaction
            VG_Controller.SetPushByGrabStrength(true);

            Transform t;
			if (VG_Controller.GetBone(avatarID, VG_HandSide.LEFT, VG_BoneType.WRIST, out t) != VG_ReturnCode.SUCCESS)
			{
				current [0] = new VG_HandStatus ();
				former  [0] = new VG_HandStatus ();
			}
			else
			{
				current [0] = new VG_HandStatus (t, VG_HandSide.LEFT);
				former  [0] = new VG_HandStatus (t, VG_HandSide.LEFT);
			}

			if (VG_Controller.GetBone(avatarID, VG_HandSide.RIGHT, VG_BoneType.WRIST, out t) != VG_ReturnCode.SUCCESS)
			{
				current [1] = new VG_HandStatus ();
				former  [1] = new VG_HandStatus ();
			}
			else
			{
				current [1] = new VG_HandStatus (t, VG_HandSide.RIGHT);
				former  [1] = new VG_HandStatus (t, VG_HandSide.RIGHT);
			}
		}
    }
    
	void ReleaseObject(uint handID)
	{
		Rigidbody obj_rb = former[handID].selectedObject != null ? former[handID].selectedObject.GetComponent<Rigidbody> () : 
			current[handID].selectedObject.GetComponent<Rigidbody>();
		if (obj_rb != null && !obj_rb.useGravity && current[handID].selectedObject.GetComponent<VG_Articulation>() == null)
			obj_rb.useGravity = true;
	}

	void FixedUpdate()
	{
		if (!VG_Controller.IsEnabled()) return;

		// Update the VG library, including controllers, avatars, etc.
		VG_Controller.Update();

        pSelector.Select(current);
        pSelector.HighlightObjects(current);

        for (uint handID = 0; handID < 2; handID++)	
		{
			// Check if hand is valid
			if (current [handID] == null) continue;
			current[handID].valid = !VG_Controller.IsMissingSensorData(current[handID].side);
			if (!current [handID].valid)
				current [handID].hand.position = Vector3.zero;

			// Check if we have no object selected and inform the library if this is the case.
			if (current [handID].selectedObject == null) 
			{
				VG_Controller.SetNoObjectSelected (current[handID].side);
				continue;
			}

            // Cache old and get new status of the hands (interaction mode, pose, etc)
            former[handID].graspStatus = current[handID].graspStatus;
            former[handID].mode = current[handID].mode;

            current[handID].mode = VG_Controller.GetInteractionMode(avatarID, current[handID].side);
            if (current[handID].mode == VG_InteractionMode.EMPTY)
            {
                VG_Controller.PushWithFinger(current[handID].selectedObject.transform, current[handID].hand, current[handID].side);
            }

			// Do things based on interaction mode
			switch (current[handID].mode)
			{
				case VG_InteractionMode.EMPTY:
					break;
				case VG_InteractionMode.PREVIEW:
					break;
				case VG_InteractionMode.GRASP:
                    break;
				case VG_InteractionMode.RELEASE:
					{
						ReleaseObject (handID);
						break;
					}
				case VG_InteractionMode.HOLD:
				case VG_InteractionMode.HOLD2:
				case VG_InteractionMode.MANIPULATE:
				case VG_InteractionMode.MANIPULATE2:
				{
					Rigidbody obj_rb = current[handID].selectedObject.GetComponent<Rigidbody> ();
					if (obj_rb != null && current[handID].selectedObject.GetComponent<VG_Articulation>() == null)
						obj_rb.useGravity = false;

					VG_Controller.GetObjectTransform(avatarID, current[handID].side, current[handID].selectedObject.gameObject);
					break;
				}
			}

			// Finally, just fill in some status data for visualization
			current[handID].grab = VG_Controller.GetGrabStrength(current[handID].side);
			current[handID].grabVel = VG_Controller.GetGrabVelocity(current[handID].side);
        }
    }

	// Late update is only for debug canvas visualization
	void LateUpdate()
	{
		for (uint handID = 0; handID < 2; handID++)
		{
			string text = (handID == 0 ? "Left Hand" : "Right Hand") + "\n";
			if (current [handID] == null) continue;

			text += (current [handID].selectedObject != null ? current [handID].selectedObject.name : "null") + "\n";
			text += "distance: " + System.Math.Round (current [handID].distance, 2) + "\n";
            text += "grasp: " + current[handID].graspStatus + "\n";
            text += "sensor valid: " + current [handID].valid + "\n";
			text += "mode: " + current [handID].mode + "\n";
			text += "grab strength: " + System.Math.Round (current [handID].grab, 2) + "\n";
			text += "grab velocity: " + System.Math.Round (current [handID].grabVel, 2) + "\n";
			if (current [handID].selectedObject != null)
            {
                if (current[handID].selectedObject.GetComponent<VG_Articulation>() != null)
                    text += "state value: " + System.Math.Round(VG_Controller.GetObjectState(current[handID].selectedObject), 4) + "\n";

                Rigidbody obj_rb = current [handID].selectedObject.GetComponent<Rigidbody> ();
				if (obj_rb != null && obj_rb.angularVelocity.magnitude > 0.001f) {
					text += "pvel: " + obj_rb.velocity.magnitude + "\n";
					text += "rvel: " + obj_rb.angularVelocity.magnitude + "\n";
					if (current [handID].mode != VG_InteractionMode.EMPTY &&
						current [handID].mode != VG_InteractionMode.RELEASE) 
					{
#if USE_STEAM_VR
						// Trigger a haptic pulse just for fun
						SteamVR_Controller.Input ((int)handID + 3).TriggerHapticPulse (500);
#endif
						obj_rb.isKinematic = true;
						VG_Controller.GetObjectTransform (avatarID, current [handID].side, current [handID].selectedObject.gameObject);
						obj_rb.isKinematic = false;
					}
				}
			}
		
			MyDebug(handID == 0 ? "DebugCanvas/LeftHandDebug" : "DebugCanvas/RightHandDebug", text);
		}
	}

	private void MyDebug(string target, string str)
	{
		GameObject textUI = GameObject.Find (target);
		if (textUI != null) textUI.GetComponent<Text>().text = str;
	}
}
