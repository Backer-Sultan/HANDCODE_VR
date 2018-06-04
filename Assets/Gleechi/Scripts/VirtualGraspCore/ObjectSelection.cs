// Copyright (C) 2014-2018 Gleechi AB. All rights reserved.

//#define USE_SHADERGLOW

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VirtualGrasp;

public enum SelectionType
{
    PUSH_GRASP_RAYCAST,
    SPHERE,
    VIRTUALGRASP_SELECTION
}

public enum TriggerPattern
{
    GRIP_AND_TRIGGER,
    TRIGGER_ONLY
}

/// Interaction params is a helper class for object selection
public class SelectionParams
{
    /// <param name="distanceThreshold"></param> How long the raycasts should be
    /// <param name="angleThreshold"></param> When to skip an object selection based on the angle
    /// <param name="radiusScale"></param> How "thick" the raycasts should be 
    public SelectionParams(float distanceThreshold, float angleThreshold, float radiusScale)
    {
        m_distanceThreshold = distanceThreshold;
        m_angleThreshold = angleThreshold;
        m_radiusScale = radiusScale;
    }

    public float m_distanceThreshold = 0.25f;
    public float m_angleThreshold = 30.0f;
    public float m_radiusScale = 1.0f;
}

public class ObjectSelection
{
    // Parameters for different selection methods (see SelectionParams above)
    static private Dictionary<string, SelectionParams> m_selectionParameters = new Dictionary<string, SelectionParams>();
    
    public Shader shader = null;
    private Color[] colors = new Color[] {Color.magenta, Color.green, Color.white};
    private int m_layerMask = 0;
#if USE_SHADERGLOW
//	private Dictionary<int, shaderGlow> highlights = new Dictionary<int, shaderGlow>();
#endif
	private Material[] m_highLightMaterials = new Material[2] {null, null};
	private Material[] m_unhighLightedMaterials = new Material[2] {null, null};
    // Dictionary to keep track of highlighted objects.
    private Dictionary<VG_HandSide, Transform> m_highlightedObjects = new Dictionary<VG_HandSide, Transform>();
    // Cached list of interactable objects
    private List<Transform> m_objects = new List<Transform>();
    // Selected list of objects close to hands
    private List<Transform> m_closeObjects = new List<Transform>();
    private bool m_filterOutInactiveObjectsFromSelection = true;

    public SelectionType m_selectionType = 
        // SelectionType.SPHERE; // example of external selection by shere
         SelectionType.VIRTUALGRASP_SELECTION; // VG internal selection
        // SelectionType.PUSH_GRASP_RAYCAST; // example of external selection by raycasting
    public TriggerPattern m_triggerPattern = TriggerPattern.TRIGGER_ONLY;

    private bool show_hints = false;
    // Lines for hints when external selection is active
    private GameObject[] lines = new GameObject[2] { new GameObject(), new GameObject() };
    private int m_avatarID = 1; // currently only support 1 avatar here

    // The seed points
    private List<KeyValuePair<Vector3, float>> seedPts = new List<KeyValuePair<Vector3, float>>();

    // Function to create seed points for raycasting according to sunflower pattern
    void sunflower(int numRays, float radius)
    {
        float r1, r2, theta;
        seedPts.Clear();
        seedPts.Add(new KeyValuePair<Vector3, float>(Vector3.forward, 1.0f)); // center
        float goldenRatio = 2 * Mathf.PI / Mathf.Pow((Mathf.Sqrt(5) + 1) / 2, 2);
        for (int k = 1; k < numRays; k++)
        {
            if (k > numRays) continue;
            r1 = Mathf.Sqrt(k - 1 / 2.0f) / Mathf.Sqrt(numRays - 1);
            r2 = radius * r1;
            theta = goldenRatio * k;
            seedPts.Add(new KeyValuePair<Vector3, float>(new Vector3(r2 * Mathf.Cos(theta), r2 * Mathf.Sin(theta), 1).normalized, 1 - r1));
        }
    }

    public void toggleHints()
    {
        show_hints = !show_hints;
    }

    private void drawLine(VG_HandSide side, Vector3 start, Vector3 end)
    {
        int s = side == VG_HandSide.LEFT ? 0 : 1;
        lines[s].transform.position = start;
        LineRenderer lr = lines[s].GetComponent<LineRenderer>();
        if (lr == null) lr = lines[s].AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = colors[s];
        lr.endColor = colors[s];
        lr.startWidth = 0.001f;
        lr.endWidth = 0.001f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public ObjectSelection(Shader shader_)
	{
        shader = shader_;

        // Initialize the layer mask for raycast-based object selection
        HashSet<int> _selectionLayers = new HashSet<int>();
        _selectionLayers.Add(LayerMask.NameToLayer("Objects"));
		_selectionLayers.Add(LayerMask.NameToLayer("chess pieces"));
		_selectionLayers.Add(LayerMask.NameToLayer("jenga pieces"));
        foreach (int layer in _selectionLayers)
            m_layerMask += 1 << layer;

        // Just cache the list of objects that are tagged
        foreach (GameObject obj in GameObject.FindObjectsOfTypeAll(typeof(GameObject)))
            if (obj.tag == "Object")
                m_objects.Add(obj.transform);

        // Initialize the highlighted objects dictionary.
        m_highlightedObjects[VG_HandSide.LEFT] = null;
        m_highlightedObjects[VG_HandSide.RIGHT] = null;

        // Parameters for raycast-based object selection
        sunflower(50, 0.05f);
        m_selectionParameters["Grasp"] = new SelectionParams(0.40f, 45.0f, 1.5f);
        m_selectionParameters["Push"] = new SelectionParams(0.075f, 90.0f, 0.75f);
        SetSelectionMethod(m_selectionType);

#if USE_SHADERGLOW
        foreach (Transform obj in m_objects)
		{
			// shaderGlow sg = obj.GetComponent<shaderGlow> ();
			// if (sg != null && sg.isActiveAndEnabled) 
			// {
			// 	//sg.lightOff ();
			// 	sg.glowIntensity = 2.0f;
			// 	sg.useNormal = false;
			// 	sg.noOcclusion = false;
			// 	sg.scaleGlow = false;
			// 	sg.outlined = false;
			// 	//sg.flashing = false;
			// 	sg.glowColor = Color.black;
			// 	sg.glowMode = shaderGlow.allowedModes.userCallFunctions;
			// 	sg.lightOn();
			// 	highlights[obj.GetInstanceID()] = sg;
			// }
		}
#endif
    }

    public void SetSelectionMethod(SelectionType type)
    {
        m_selectionType = type;
        VG_Controller.SetSelectObjectMethod(VG_HandSide.LEFT, m_selectionType == SelectionType.VIRTUALGRASP_SELECTION ? VG_SelectObjectMethod.vgsINTERNAL_SELECTION : VG_SelectObjectMethod.vgsEXTERNAL_SELECTION);
        VG_Controller.SetSelectObjectMethod(VG_HandSide.RIGHT, m_selectionType == SelectionType.VIRTUALGRASP_SELECTION ? VG_SelectObjectMethod.vgsINTERNAL_SELECTION : VG_SelectObjectMethod.vgsEXTERNAL_SELECTION);
    }

    public void InitFromHands(VG_HandStatus[] hands)
    {
        for (int i = 0; i < colors.Length; i++)
            colors[i].a = 0.5f;
        
        // Always initialize the little selection sphere (though we only use it in SelectionType.SPHERE),
        // since maybe we want to switch online between selection modes.

        Vector3 p; Quaternion q; int iid;
        foreach (VG_HandStatus h in hands)
        {
            VG_Controller.GetFingerBone(m_avatarID, h.side, 1, -1, out iid, out p, out q);

            GameObject pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointer.name = "Pointer";

            GameObject.DestroyImmediate(pointer.GetComponent<Collider>());
            //pointer.GetComponent<Renderer>().enabled = false;
            pointer.transform.SetParent(h.hand);
            pointer.transform.rotation = q;
            pointer.transform.position = p + q * new Vector3(0, 0, 0.02f);
            pointer.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        }
    }

	public void Highlight(VG_HandStatus hand)
    {
        // Got no object, got no highlight
        if (hand.selectedObject == null)
            return;

        if (hand.selectedObject == m_highlightedObjects[hand.side])
            return;
        if (hand.selectedObject == m_highlightedObjects[hand.side == VG_HandSide.LEFT ? VG_HandSide.RIGHT : VG_HandSide.LEFT])
            return;

#if USE_SHADERGLOW
        int iid = hand.selectedObject.GetInstanceID ();
		if (false) //highlights.ContainsKey (iid)) 
		{
            // m_highlightedObjects[hand.side] = hand.selectedObject;
            // highlights [iid].flashing = true;
			// highlights [iid].glowColor = hand.side < 0 ? colors [0] : colors [1];
			// highlights [iid].lightOn ();
		}
		else 
#endif
        {
            int id = hand.side < 0 ? 0 : 1;
			m_highlightedObjects [hand.side] = hand.selectedObject;
            m_unhighLightedMaterials[id] = new Material (hand.selectedObject.GetComponentInChildren<MeshRenderer> ().material);
            m_highLightMaterials[id] = new Material (hand.selectedObject.GetComponentInChildren<MeshRenderer> ().material);
            m_highLightMaterials[id].shader = shader;
			m_highlightedObjects [hand.side].GetComponentInChildren<MeshRenderer> ().material = m_highLightMaterials[id];
			m_highlightedObjects [hand.side].GetComponentInChildren<MeshRenderer> ().material.SetColor ("_RimColor", colors [id]);
		}
    }

	public void Unhighlight(VG_HandStatus hand)
    {
        // Got no object (or the same object as before), got no unhighlight
        Transform highlightedObject = m_highlightedObjects[hand.side];
        if (highlightedObject == hand.selectedObject || highlightedObject == null)
            return;
        
#if USE_SHADERGLOW
        int iid = highlightedObject.GetInstanceID ();
		if (false) //highlights.ContainsKey (iid)) 
		{
			// //highlights [iid].lightOff ();
			// highlights [iid].flashing = false;
			// highlights [iid].glowColor = Color.black;
            // m_highlightedObjects[hand.side] = null;
        } 
		else
#endif
        {
            int id = hand.side < 0 ? 0 : 1;
            highlightedObject.GetComponentInChildren<MeshRenderer> ().material = m_unhighLightedMaterials[id];
            m_highlightedObjects[hand.side] = null;
		}
    }

    public void HighlightObjects(VG_HandStatus[] current, VG_HandStatus[] former)
    {
        if (current[0] == null || current[1] == null)
            return;

        if (current[0].selectedObject != null &&
            current[0].selectedObject == current[1].selectedObject)
		{
	   		if (current[0].mode == VG_InteractionMode.EMPTY && current[1].mode == VG_InteractionMode.EMPTY) 
			{
				if (current [0].distance >= 0 && current [0].distance < current [1].distance)
					Highlight (current[0]);
				else
					Highlight (current[1]);
			}
			else if (current[0].mode == VG_InteractionMode.EMPTY)
				Highlight (current[0]);
			else if (current[1].mode == VG_InteractionMode.EMPTY)
				Highlight (current[1]);
		}
		else for (int i = 0; i < current.Length; i++)
		{
            // Switch off highlight for former selected object if it switched
            Unhighlight(former[i]);

			// If there's no selected object, we have nothing to highlight
			if (current [i].selectedObject == null)
				continue;

			// If the hand is not empty, we have nothing to highlight
			if (current [i].mode != VG_InteractionMode.EMPTY)
				continue;
			
			Highlight (current[i]);
		}
	}

	private bool CheckRaycast(Vector3 p, Vector3 dir, float threshold, out Transform selectedObject, out float distance)
    {
        selectedObject = null;
        distance = -1;

        Ray ray = new Ray(p, dir);
        RaycastHit raycasthit = new RaycastHit();

        if (Physics.Raycast(ray, out raycasthit, threshold, m_layerMask))
        {
            selectedObject = raycasthit.collider.gameObject.transform;
            distance = raycasthit.distance;
            Debug.DrawLine(p, p + raycasthit.distance * dir, Color.green);
            return true;
        }

        Debug.DrawLine(p, p + threshold * dir, Color.white);

        return false;
    }

    private void SelectObjectBySeedRaycasts(string mode, Vector3 p, Quaternion q, out Transform selectedObject, out float distance)
    {
        selectedObject = null;
        distance = -1;

        float max_hits = 0;
        Dictionary<Transform, float> hits = new Dictionary<Transform, float>();
        foreach (KeyValuePair<Vector3, float> pair in seedPts)
        {
            Vector3 seed = pair.Key;
            seed.z = 0;

            if (CheckRaycast(p + q * (seed * m_selectionParameters[mode].m_radiusScale),
                             q * Vector3.forward,
                             m_selectionParameters[mode].m_distanceThreshold,
                             out selectedObject, out distance))
            {
                if (!hits.ContainsKey(selectedObject)) hits.Add(selectedObject, 0);
                hits[selectedObject] += Mathf.Pow(pair.Value, 2);
                if (hits[selectedObject] > max_hits)
                    max_hits = hits[selectedObject];
            }
        }
    }

    private void SelectObjectByClosestBound(Vector3 p, out Transform selectedObject)
    {
        float nxt_distance;
        float cnt_distance;
        float min_cnt_distance = float.MaxValue;

        float d, distance = -1;
        selectedObject = null;
        Vector3 c, closestPoint;
        Collider collider;

        foreach (Transform obj in m_closeObjects)
        {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0) continue;
            closestPoint = colliders[0].bounds.ClosestPoint(p);
            nxt_distance = Vector3.Distance(p, closestPoint);
            collider = colliders[0];
            for (int i = 1; i < colliders.Length; i++)
            {
                c = colliders[i].bounds.ClosestPoint(p);
                d = Vector3.Distance(p, c);
                if (d < nxt_distance)
                {
                    nxt_distance = d;
                    closestPoint = c;
                    collider = colliders[i];
                }
            }
            if (nxt_distance > m_selectionParameters["Grasp"].m_distanceThreshold)
                continue;
            
            // draw white lines to show closest objects
            Debug.DrawLine(p, closestPoint, Color.green);

            // store minimum
            cnt_distance = (collider.bounds.center - p).magnitude;
            if (Mathf.Abs(nxt_distance - distance) < 0.01f)
            {
                if (cnt_distance < min_cnt_distance)
                {
                    min_cnt_distance = cnt_distance;
                    distance = Mathf.Min(nxt_distance, distance);
                    selectedObject = obj;
                }
            }
            else if (nxt_distance < distance || distance < 0)
            {
                min_cnt_distance = cnt_distance;
                distance = nxt_distance;
                selectedObject = obj;
            }
        }
    }

    private void SelectObjectByClosestBound(string mode, Vector3 p, Vector3 dir, out Transform selectedObject, out float distance)
    {
        float nxt_distance;
        float cnt_distance;
        float min_cnt_distance = float.MaxValue;

        distance = -1;
        selectedObject = null;
        float d;
        Vector3 c, closestPoint;
        Collider collider;

        foreach (Transform obj in m_closeObjects)
        {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0) continue;
            closestPoint = colliders[0].bounds.ClosestPoint(p);
            nxt_distance = Vector3.Distance(p, closestPoint);
            collider = colliders[0];
            for (int i = 1; i < colliders.Length; i++)
            {
                c = colliders[i].bounds.ClosestPoint(p);
                d = Vector3.Distance(p, c);
                if (d < nxt_distance)
                {
                    nxt_distance = d;
                    closestPoint = c;
                    collider = colliders[i];
                }
            }
           
            if (nxt_distance > m_selectionParameters[mode].m_distanceThreshold)
            {
                //if (mode == "Push") Debug.Log("closest bound distance too big:" + obj.name + ";" + nxt_distance + ">" + m_interactionParameters[mode].m_distanceThreshold);
                continue;
            }

            // Note: when angle threshold is 90, this is the same
            if (Vector3.Dot(dir, closestPoint - p) < 0 || Vector3.Angle(dir, closestPoint - p) > m_selectionParameters[mode].m_angleThreshold)
            {
                //if (mode == "Push") Debug.Log("closest bound angle too big.");
                continue;
            }

            // draw white lines to show closest objects
            Debug.DrawLine(p, closestPoint, Color.green);

            // store minimum
            cnt_distance = (collider.bounds.center - p).magnitude;
            if (Mathf.Abs(nxt_distance - distance) < 0.01f)
            {
                if (cnt_distance < min_cnt_distance)
                {
                    min_cnt_distance = cnt_distance;
                    distance = Mathf.Min(nxt_distance, distance);
                    selectedObject = obj;
                }
            }
            else if (nxt_distance < distance || distance < 0)
            {
                min_cnt_distance = cnt_distance;
                distance = nxt_distance;
                selectedObject = obj;
            }
        }
    }

    private void SelectObjectByInBounds(Vector3 p, out Transform selectedObject)
    {
        selectedObject = null;
        foreach (Transform obj in m_closeObjects)
        {
            // Note: there'll be a problem with nested object colliders
            if (obj.GetComponent<Collider>().bounds.Contains(p))
            {
                selectedObject = obj;
                return;
            }
        }
    }

    // Select an object for push interaction
    public void SelectToPush(VG_HandStatus hand)
    {
        Vector3 p;
        Quaternion q;
        int iid;

        // Get the pose of the first avatar's (1) index finger's (1) last (-1) limb, i.e. the fingertip.
        VG_Controller.GetFingerBone(1, hand.side, 1, -1, out iid, out p, out q);
        Vector3 dir = (Vector3.up + Vector3.forward).normalized;
        drawLine(hand.side, p, p + m_selectionParameters["Push"].m_distanceThreshold * (q * dir));

        //SelectObjectByInBounds(p, out hand.selectedObject);

        //if (hand.selectedObject == null)
        SelectObjectByClosestBound("Push", p, q * dir, out hand.selectedObject, out hand.distance);

        if (hand.selectedObject == null)
            SelectObjectBySeedRaycasts("Push", p, Quaternion.LookRotation(q * dir), out hand.selectedObject, out hand.distance);

        // Deselect if the object is no prismatic joint. We only want pushable objects to be pushable for now.
        if (hand.selectedObject != null)
        {
            VG_Articulation articulation = hand.selectedObject.GetComponent<VG_Articulation>();
            if (articulation == null || articulation.m_type != VG_JointType.PRISMATIC)
                hand.selectedObject = null;
        }
    }

    // Select an object for grasp interaction
    public void SelectToGrasp(VG_HandStatus hand)
    {
        Vector3 p, p2;
        Quaternion q;
        int iid;
        
        // Get the pose of the grasp approach center.
 
        VG_Controller.GetFingerBone(m_avatarID, hand.side, 1, 0, out iid, out p, out q);
        VG_Controller.GetBone(m_avatarID, hand.side, VG_BoneType.APPROACH, out iid, out p2, out q);
        Vector3 dir = Vector3.up;

        drawLine(hand.side, p, p + m_selectionParameters["Grasp"].m_distanceThreshold * (q * dir));

        SelectObjectBySeedRaycasts("Grasp", p, Quaternion.LookRotation(q * dir), out hand.selectedObject, out hand.distance);

        if (hand.selectedObject == null)
        {
            SelectObjectByClosestBound("Grasp", p, q * dir, out hand.selectedObject, out hand.distance);

            // Assure by single raycast that no other object is in front of the selected one
            //CheckRaycast(p, hand.selectedObject.GetComponent<Collider>().bounds.ClosestPoint(p) - p, m_interactionParameters["Grasp"].m_distanceThreshold, out hand.selectedObject, out hand.distance);
        }

        if (hand.selectedObject == null)
            SelectObjectByInBounds(p, out hand.selectedObject);

        // If there's no selected grasp, we have nothing to highlight
        //if (!VG_Controller.CanGrasp(avatarID, hand.side == VG_HandSide.LEFT ? 0 : 1))
        //    hand.selectedObject = null;
    }

    private void filterObjects(VG_HandStatus[] status)
    {
        int iid;
        Vector3 p;
        Quaternion q;
        Collider r;
        m_closeObjects.Clear();

        foreach (Transform obj in m_objects)
        {
            // skip deactivated objects
            if (!obj.gameObject.activeInHierarchy)
                continue;
            
            r = obj.GetComponent<Collider>();
            foreach (VG_HandStatus hand in status)
            { 
                VG_Controller.GetBone(m_avatarID, hand.side, VG_BoneType.APPROACH, out iid, out p, out q);
                if (Vector3.Distance(p, r.bounds.ClosestPoint(p)) < m_selectionParameters["Grasp"].m_distanceThreshold)
                    m_closeObjects.Add(obj);
            }
        }

        /*
        if (m_closeObjects.Count > 0)
        {
            string str = "filtered out " + m_closeObjects.Count + "/" + m_objects.Count + " that are in hand proximity:\n";
            foreach (Transform t in m_closeObjects)
                str += t.name + "\n";
            Debug.Log(str);
        }
        */
    }

    // Select objects based on current hands.
    public void Select(VG_HandStatus[] status)
    {
        bool trigger_pushed = false;

        if (m_selectionType == SelectionType.VIRTUALGRASP_SELECTION)
        {
            if (m_filterOutInactiveObjectsFromSelection)
            {
                List<Transform> hiddenObjects = new List<Transform>();
                foreach (Transform obj in m_objects)
                    if (!obj.gameObject.activeInHierarchy)
                        hiddenObjects.Add(obj);
                VG_Controller.SetHiddenObjects(hiddenObjects);
            }
        }
        else
        {
            filterObjects(status);
            VG_Controller.RegisterObjectsForSelection(m_closeObjects);
        }

        foreach (VG_HandStatus hand in status)
        {
            // If the hand is invalid in any way, reset the current selection
            if (hand == null)
                continue;

            if (!hand.valid || hand.hand == null)
            {
                hand.distance = -1;
                hand.selectedObject = null;
                continue;
            }

            trigger_pushed = SteamVR_Controller.Input(hand.side == VG_HandSide.LEFT ? 3 : 4).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.01f;

            // De/Activate the selection pointer
            Transform pointer = hand.hand.Find("Pointer");
            if (pointer != null) pointer.gameObject.SetActive(show_hints); // && hand.mode == VG_InteractionMode.EMPTY);
            lines[hand.side == VG_HandSide.LEFT ? 0 : 1].SetActive(show_hints && !trigger_pushed);

            // If the hand is anything but empty, keep the current selection
            if (hand.mode != VG_InteractionMode.EMPTY)
                continue;

            VG_Affordance affordance = VG_Affordance.GRASPABLE;
            switch (m_selectionType)
            {
                case SelectionType.VIRTUALGRASP_SELECTION:
                    hand.selectedObject = VG_Controller.GetSelectedObject(hand.side);
                    affordance = VG_Controller.GetObjectAffordance(hand.selectedObject);
                    break;
                case SelectionType.SPHERE:
                    SelectObjectByInBounds(pointer.position, out hand.selectedObject);
                    if (hand.selectedObject == null) SelectObjectByClosestBound(pointer.position, out hand.selectedObject);
                    affordance = VG_Controller.GetObjectAffordance(hand.selectedObject);
                    VG_Controller.SelectObject(hand.side, hand.selectedObject);
                    break;
                case SelectionType.PUSH_GRASP_RAYCAST:
                    
                    // Prioritize an object along pushing direction
                    if (hand.selectedObject != null)
                    {
                        // Do not select object if it's not a pushable one.
                        affordance = VG_Controller.GetObjectAffordance(hand.selectedObject);
                        if (affordance != VG_Affordance.PUSHABLE)
                            hand.selectedObject = null;
                    }

                    // If we did not find an object along pushable direction, or that object is not pushable,
                    // try to find an object along grasping direction
                    if (hand.selectedObject == null)
                    {
                        SelectToGrasp(hand);
                        if (hand.selectedObject != null)
                        {
                            // Disable the selected object if it's not a graspable one.
                            affordance = VG_Controller.GetObjectAffordance(hand.selectedObject);
                            if (affordance != VG_Affordance.GRASPABLE)
                                hand.selectedObject = null;
                        }

                    }

                    VG_Controller.SelectObject(hand.side, hand.selectedObject);
                    break;
            }
        }
    }
}
