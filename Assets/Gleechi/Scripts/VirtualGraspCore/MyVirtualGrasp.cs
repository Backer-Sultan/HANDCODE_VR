// Copyright (C) 2014-2018 Gleechi AB. All rights reserved.

#define USE_VALVE_VE
#define USE_STEAM_VR
//#define ENABLE_RIGMOVER
//#define ENABLE_GRASP_LABELING

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using VirtualGrasp; 

/**
 * MyVirtualGrasp is the customizable interface to the 
 *
 * It inherits from VirtualGraspBase, which wraps the main communication functions of the API.
 * VirtualGraspBase inherits from Monobehavior so you can use this as a component to a GameObject in Unity.
 */
public class MyVirtualGrasp : MonoBehaviour
{
    // Currently we only assume one avatar in the scene
    private List<int> m_avatarIDs = new List<int>() { 1 };
    
    // An object selector, responsible for object selection and highlighting 
    private ObjectSelection pSelector = null;

    // A sensor configuration for the VirtualGrasp control
	private VG_SensorConfiguration pSensorMapper = null;

#if ENABLE_RIGMOVER
    // A rig mover, to move a selected avatar to the place where VG will access it.
    private RigMover pRigMover = null;
#endif
    
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        VG_SensorConfiguration[] sms = UnityEngine.Object.FindObjectsOfType<VG_SensorConfiguration> () as VG_SensorConfiguration[];

		if (sms.Length != 1) 
		{
			Debug.LogError ("Assure there is exactly 1 VG_SensorConfiguration in the Scene. There are " + sms.Length + ".");
			enabled = false;
			return;
		}

#if ENABLE_RIGMOVER
		pRigMover = GameObject.Find ("HandResources").GetComponent<RigMover> ();
		if (pRigMover == null) 
		{
			Debug.LogError ("No RigMover in Scene!");
			enabled = false;
			return;
		}
		pRigMover.MoveRig ();
#endif

		pSensorMapper = sms [0];

		foreach (VG_SensorSetup sensor in pSensorMapper.m_sensors) 
		{
			if (sensor.m_origin != null) continue;

			GameObject origin;
            // Assign a sensor origin based on the controller that is configured.
			switch (sensor.m_sensor) 
			{
				case VG_SensorType.Leap:
					if (!UnityEngine.XR.XRSettings.enabled && GameObject.Find ("OriginForLeapUnmounted"))
						sensor.m_origin = GameObject.Find ("OriginForLeapUnmounted").transform;
					else if (GameObject.Find ("OriginForLeapMounted"))
						sensor.m_origin = GameObject.Find ("OriginForLeapMounted").transform;
					else sensor.m_origin = null;
					break;
				case VG_SensorType.Manus:
					origin = GameObject.Find ("OriginForManus");
					sensor.m_origin = origin != null ? origin.transform : null; 
					break;
				case VG_SensorType.Vive:
				case VG_SensorType.ViveTracker:
				case VG_SensorType.OculusTouchOpenVR:
					origin = GameObject.Find ("OriginForVive");
					sensor.m_origin = origin != null ? origin.transform : null; 
				break;
				case VG_SensorType.OculusTouchOVR:
					origin = GameObject.Find ("OriginForTouch");
					sensor.m_origin = origin != null ? origin.transform : null; 
					break;
				default:
					Debug.LogWarning ("No origin found for sensor " + sensor.m_sensor);
					break;
			}
		}
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    void OnApplicationQuit()
    {
        VG_Controller.Release();
    }

    void Start()
    {
        string libraryDirectory = "";
#if UNITY_EDITOR_64
		libraryDirectory = Application.dataPath + "/Gleechi/Plugins/x86_64/";
#else

#if UNITY_EDITOR
		libraryDirectory = Application.dataPath + "/Gleechi/Plugins/x86/";
#else
        // TODO: figure out why the common Application.dataPath does not work (GD-200)
        libraryDirectory ="./piia_april2018_Data/Plugins/";
#endif
#endif

        Debug.Log ("Initializing VG library from " + libraryDirectory);
		VG_Controller.Initialize(libraryDirectory);

        if (!VG_Controller.IsEnabled())
		{
			Debug.LogError ("Failed to initialize VirtualGrasp plugin.");
			enabled = false;
			return;
		}

        if (pSensorMapper != null)
		{
            pSensorMapper.Register ();
            // Option: set this to true to have the alternative button pushing interaction
            //VG_Controller.SetPushByGrabStrength(true);
            VG_Controller.RegisterHands(m_avatarIDs[0]);
		}

        pSelector = GetComponent<ObjectSelection>();
        if (pSelector == null)
            pSelector = gameObject.AddComponent<ObjectSelection>();

        VG_Controller.Update();
        pSelector.InitFromHands(VG_Controller.current[m_avatarIDs[0]]);
        VG_Controller.SetPushByGrabStrength(true);

        // note: this only works with wrapper that has baking enabled
        // TODO: move this to wrapper
        if (VG_Controller.IsBakingInterfaceEnabled())
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].activeInHierarchy)
                    VG_Controller.BakeMesh(objects[i]);
            }
        }
        //else Debug.LogWarning("No BakingInterface available.");
    }
    
    // Custom event handling
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            pSelector.toggleHints();
        if (Input.GetKeyDown(KeyCode.F3))
            toggleTriggerMode();

#if ENABLE_GRASP_LABELING
		// Grip button press while holding object labels grasp
		for (uint handID = 0; handID < 2; handID++)	
		{
			if (SteamVR_Controller.Input (3 + (int)handID).GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip) &&
				(current[handID].mode == VG_InteractionMode.HOLD || current[handID].mode == VG_InteractionMode.MANIPULATE))
			{
				Debug.Log("Labeling primary grasp for:" + current[handID].selectedObject.name);
				VG_Controller.LabelSelectedGraspAndSaveNewBlob(current[handID].side, current[handID].selectedObject.name, VG_GraspLabel.PRIMARY);
			}
        }
#endif
	}

    void StartVelocityEstimation(VG_HandStatus hand)
    {
#if USE_VALVE_VE
        Valve.VR.InteractionSystem.VelocityEstimator ve = hand.hand.GetComponent<Valve.VR.InteractionSystem.VelocityEstimator>();
        if (ve == null)
        {
            ve = hand.hand.gameObject.AddComponent(typeof(Valve.VR.InteractionSystem.VelocityEstimator)) as Valve.VR.InteractionSystem.VelocityEstimator;
        }
        //if (VG_Controller.former[avatarID][handID].mode == VG_InteractionMode.EMPTY)
        ve.BeginEstimatingVelocity();
#endif
    }

    void StopVelocityEstimation(int avatarId, VG_HandStatus hand, out Vector3 pvel, out Vector3 rvel)
    {
#if USE_VALVE_VE
        pvel = Vector3.zero;
        rvel = Vector3.zero;

        Valve.VR.InteractionSystem.VelocityEstimator ve = hand.hand.GetComponent<Valve.VR.InteractionSystem.VelocityEstimator>();
        if (ve != null)
        {
            ve.FinishEstimatingVelocity();
            pvel = 2.0f * ve.GetVelocityEstimate();
            rvel = ve.GetAngularVelocityEstimate();
        }
#else
        VG_Controller.GetObjectVelocities(avatarId, hand.side, out pvel, out rvel);
#endif
    }

    void ThrowObject(int avatarId, VG_HandStatus hand)
	{
        //VG_HandStatus former = VG_Controller.former[avatarId][hand.side == VG_HandSide.LEFT ? 0 : 1];
        //if (former.selectedObject != null)
        // hand = former;

        Rigidbody obj_rb = hand.selectedObject.GetComponent<Rigidbody>();
        if (obj_rb != null && !obj_rb.useGravity &&
            hand.selectedObject.GetComponent<VG_Articulation>() == null)
        {
            Vector3 pvel, rvel;
            StopVelocityEstimation(avatarId, hand, out pvel, out rvel);

            obj_rb.useGravity = true;
            obj_rb.maxAngularVelocity = rvel.magnitude;
            obj_rb.velocity = pvel;
            obj_rb.angularVelocity = rvel;
        }
	}

    void FixedUpdate()
	{
		if (!VG_Controller.IsEnabled())
            return;

        // Update hand status from the library
        VG_Controller.UpdateHands();

        // Select and highlight based on the current hand status
        foreach (int avatarID in m_avatarIDs)
        {
            pSelector.Select(VG_Controller.current[avatarID]);
            pSelector.HighlightObjects(VG_Controller.current[avatarID], VG_Controller.former[avatarID]);
        }
        
        // Make a step in the VirtualGrasp library
        VG_Controller.Update();

        // Decide what to do based on the current hand status
        foreach (KeyValuePair<int, VG_HandStatus[]> avatar in VG_Controller.current)
        {
            foreach (VG_HandStatus hand in avatar.Value)
            {
                if (hand.selectedObject == null)
                    continue;

                switch (hand.mode)
                {
                    case VG_InteractionMode.EMPTY:
                        break;
                    case VG_InteractionMode.RELEASE:
                        {
                            ThrowObject(avatar.Key, hand);
                            break;
                        }
                    case VG_InteractionMode.GRASP:
                        {
                            StartVelocityEstimation(hand);
                            break;
                        }
                    case VG_InteractionMode.PREVIEW:
                        break;
                    case VG_InteractionMode.HOLD:
                    case VG_InteractionMode.HOLD2:
                    case VG_InteractionMode.MANIPULATE:
                    case VG_InteractionMode.MANIPULATE2:
                        {
                            // Disable gravity if needed
                            Rigidbody obj_rb = hand.selectedObject.GetComponent<Rigidbody>();
                            if (obj_rb != null && obj_rb.useGravity)
                            {
                                if (hand.selectedObject.GetComponent<VG_Articulation>() == null)
                                    obj_rb.useGravity = false;
                            }
                            break;
                        }
                }
            }
        }

        // Finally, set the objects according to VirtualGrasp
        VG_Controller.GetObjectTransforms();
    }

    void LateUpdate()
    {
        foreach (KeyValuePair<int, VG_HandStatus[]> avatar in VG_Controller.current)
        {
            foreach (VG_HandStatus hand in avatar.Value)
            {
                if (hand.selectedObject == null) continue;
                if (hand.mode == VG_InteractionMode.EMPTY) continue;
                if (hand.mode == VG_InteractionMode.RELEASE) continue;
                Rigidbody obj_rb = hand.selectedObject.GetComponent<Rigidbody>();
                if (obj_rb == null) continue;
                if (obj_rb.velocity.magnitude < 0.001f) continue;

#if USE_STEAM_VR
                // If we get a collision (some impact) on the object held,
                // trigger a haptic pulse.
                SteamVR_Controller.Input(hand.side == VG_HandSide.LEFT ? 3 : 4).TriggerHapticPulse(500);
#endif

                // If we get a collision (some impact) on the object held,
                // we also set the object back into the hand.
                obj_rb.isKinematic = true;
                VG_Controller.GetObjectTransforms();
                obj_rb.isKinematic = false;
            }
        }
    }

    public void toggleHints()
    {
        pSelector.toggleHints();
    }

    public void toggleTriggerMode()
    {
        if (pSelector.m_selectionType == SelectionType.SPHERE &&
            pSelector.m_triggerPattern == TriggerPattern.TRIGGER_ONLY
            )
        {
            Debug.Log("switch from" + pSelector.m_selectionType + "," + pSelector.m_triggerPattern);
            pSelector.m_selectionType = SelectionType.PUSH_GRASP_RAYCAST;
            pSelector.m_triggerPattern = TriggerPattern.GRIP_AND_TRIGGER;
            Debug.Log("switched to" + pSelector.m_selectionType + "," + pSelector.m_triggerPattern);
        }
        else
        {
            Debug.Log("switch from" + pSelector.m_selectionType + "," + pSelector.m_triggerPattern);
            pSelector.m_selectionType = SelectionType.SPHERE;
            pSelector.m_triggerPattern = TriggerPattern.TRIGGER_ONLY;
            Debug.Log("switched to" + pSelector.m_selectionType + "," + pSelector.m_triggerPattern);
        }
    }

    /* Info canvas
    public void toggleInfos()
    {
        infoCanvas.SetActive(!infoCanvas.activeSelf);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            infoCanvas.SetActive(!infoCanvas.activeSelf);
    }
    */
}
