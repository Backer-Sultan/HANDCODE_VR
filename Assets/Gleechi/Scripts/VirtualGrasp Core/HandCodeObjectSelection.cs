// Copyright (C) 2014-2017 Gleechi AB. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VirtualGrasp;
using HandCode;

/// Interaction params is a helper class for object selection based on multi-raycasting
public class InteractionParams
{
    /// <param name="distanceThreshold"></param> How long the raycasts should be
    /// <param name="angleThreshold"></param> When to skip an object selection based on the angle
    /// <param name="radiusScale"></param> How "thick" the raycasts should be 
    public InteractionParams(float distanceThreshold, float angleThreshold, float radiusScale)
    {
        m_distanceThreshold = distanceThreshold;
        m_angleThreshold = angleThreshold;
        m_radiusScale = radiusScale;
    }

    public float m_distanceThreshold = 0.25f;
    public float m_angleThreshold = 30.0f;
    public float m_radiusScale = 1.0f;
}

public class HandCodeObjectSelection
{
    // Dictionary to keep track of highlighted objects.
    private Dictionary<VG_HandSide, Transform> m_highlightedObjects = new Dictionary<VG_HandSide, Transform>();
    // Dictionary to initialize different interaction parameters for multi-raycast object selection.
    static private Dictionary<string, InteractionParams> m_interactionParameters = new Dictionary<string, InteractionParams>();
    // The seed points for multi-raycast object selection.
    List<KeyValuePair<Vector3, float>> seedPts = new List<KeyValuePair<Vector3, float>>();
    // Cached list of interactable objects
    private List<Transform> m_objects = new List<Transform>();

    // Method to generate the seed points for multi-raycast object selection.
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

    public HandCodeObjectSelection()
    {
        // Just cache the list of objects that are tagged
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Object"))
            m_objects.Add(obj.transform);

        // Initialize the highlighted objects dictionary.
        m_highlightedObjects[VG_HandSide.LEFT] = null;
        m_highlightedObjects[VG_HandSide.RIGHT] = null;

        // Parameters for raycast-based object selection
        sunflower(50, 0.05f);
        m_interactionParameters["Grasp"] = new InteractionParams(0.25f, 30.0f, 1.0f);
        m_interactionParameters["Push"] = new InteractionParams(0.075f, 90.0f, 0.75f);
    }

    // Highlight the object that is held by a hand.
    public void Highlight(VG_HandStatus hand)
    {
        // Got no object, got no highlight
        if (hand.selectedObject == null)
            return;

        // Highlight object is it has a component and if it's not already highlighted
        Highlighter highlighter = hand.selectedObject.gameObject.GetComponentInChildren<Highlighter>();
        if (!highlighter)
            highlighter = hand.selectedObject.gameObject.GetComponentInParent<Highlighter>();
        if (highlighter && !highlighter.enabled)
        {
            m_highlightedObjects[hand.side] = hand.selectedObject;
            highlighter.enabled = true;
        }
    }

    // Unhighlight the object that is held by a hand.
    public void Unhighlight(VG_HandStatus hand)
    {
        // Got no object (or the same object as before), got no unhighlight
        Transform highlightedObject = m_highlightedObjects[hand.side];
        if (highlightedObject == hand.selectedObject || highlightedObject == null)
            return;

        // Unhighlight object is it has a component and if it's not already unhighlighted
        Highlighter highlighter = highlightedObject.gameObject.GetComponentInChildren<Highlighter>();
        if (!highlightedObject)
            highlighter = highlightedObject.gameObject.GetComponentInParent<Highlighter>();
        if (highlighter && highlighter.enabled)
        {
            m_highlightedObjects[hand.side] = null;
            highlighter.enabled = false;
        }
    }

    // Logic to highlight objects based on all current hands.
    public void HighlightObjects(VG_HandStatus[] current)
    {
        if (current[0] == null || current[1] == null)
            return;

        if (current[0].graspStatus == VG_ReturnCode.SUCCESS &&
            current[1].graspStatus == VG_ReturnCode.SUCCESS &&
            current[0].selectedObject != null &&
            current[0].selectedObject == current[1].selectedObject)
        {
            if (current[0].mode == VG_InteractionMode.EMPTY && current[1].mode == VG_InteractionMode.EMPTY)
            {
                if (current[0].distance >= 0 && current[0].distance < current[1].distance)
                    Highlight(current[0]);
                else
                    Highlight(current[1]);
            }
            else if (current[0].mode == VG_InteractionMode.EMPTY)
                Highlight(current[0]);
            else if (current[1].mode == VG_InteractionMode.EMPTY)
                Highlight(current[1]);
        }
        else
        {
            for (int i = 0; i < current.Length; i++)
            {
                // Switch off highlight for former selected object if it switched
                Unhighlight(current[i]);

                // If there's no selected object, we have nothing to highlight
                if (current[i].selectedObject == null)
                    continue;

                // If the hand is not empty, we have nothing to highlight
                if (current[i].mode != VG_InteractionMode.EMPTY)
                    continue;

                Highlight(current[i]);
            }
        }
    }

    // Check a raycast from a given point in a given direction, and return the hit object and the distance to the hit.
    private bool CheckRaycast(Vector3 p, Vector3 dir, float threshold, out Transform selectedObject, out float distance)
    {
        selectedObject = null;
        distance = -1;

        Ray ray = new Ray(p, dir);
        RaycastHit raycasthit = new RaycastHit();
        if (Physics.Raycast(ray, out raycasthit, threshold, 1 << LayerMask.NameToLayer("Objects")))
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

            if (CheckRaycast(p + q * (seed * m_interactionParameters[mode].m_radiusScale),
                             q * Vector3.forward,
                             m_interactionParameters[mode].m_distanceThreshold,
                             out selectedObject, out distance))
            {
                if (!hits.ContainsKey(selectedObject)) hits.Add(selectedObject, 0);
                hits[selectedObject] += Mathf.Pow(pair.Value, 2);
                if (hits[selectedObject] > max_hits)
                    max_hits = hits[selectedObject];
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

        foreach (Transform obj in m_objects)
        {
            Collider r = obj.GetComponent<Collider>();

            Vector3 closestPoint = r.bounds.ClosestPoint(p);
            nxt_distance = Vector3.Distance(p, closestPoint);
            if (nxt_distance > m_interactionParameters[mode].m_distanceThreshold)
                continue;

            // Note: when angle threshold is 90, this is the same
            if (Vector3.Dot(dir, closestPoint - p) < 0 || Vector3.Angle(dir, closestPoint - p) > m_interactionParameters[mode].m_angleThreshold)
                continue;

            // draw white lines to show closest objects
            Debug.DrawLine(p, closestPoint);

            // store minimum
            cnt_distance = (r.bounds.center - p).magnitude;
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
        foreach (Transform obj in m_objects)
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

        SelectObjectByClosestBound("Push", p, q * Vector3.up, out hand.selectedObject, out hand.distance);

        if (hand.selectedObject == null)
            SelectObjectBySeedRaycasts("Push", p, Quaternion.LookRotation(q * (Vector3.up + Vector3.forward)), out hand.selectedObject, out hand.distance);

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
        Vector3 p;
        Quaternion q;
        int iid;

        // Get the pose of the grasp approach center.
        VG_Controller.GetBone(1, hand.side, VG_BoneType.APPROACH, out iid, out p, out q);

        SelectObjectBySeedRaycasts("Grasp", p, Quaternion.LookRotation(q * Vector3.up), out hand.selectedObject, out hand.distance);

        if (hand.selectedObject == null)
        {
            SelectObjectByClosestBound("Grasp", p, q * Vector3.up, out hand.selectedObject, out hand.distance);

            // Assure by single raycast that no other object is in front of the selected one
            //CheckRaycast(p, hand.selectedObject.GetComponent<Collider>().bounds.ClosestPoint(p) - p, m_interactionParameters["Grasp"].m_distanceThreshold, out hand.selectedObject, out hand.distance);
        }

        if (hand.selectedObject == null)
            SelectObjectByInBounds(p, out hand.selectedObject);
    }

    // Select objects based on current hands.
    public void Select(VG_HandStatus[] status)
    {
        foreach (VG_HandStatus hand in status)
        {
            // If the hand is invalid in any way, reset the current selection
            if (hand == null ||
                !hand.valid ||
                hand.hand == null)
            {
                hand.distance = -1;
                hand.selectedObject = null;
                continue;
            }

            // If the hand is anything but empty, keep the current selection
            if (hand.mode != VG_InteractionMode.EMPTY)
                continue;

            // If user started pushing interaction button, keep the current selection
            if (SteamVR_Controller.Input(hand.side == VG_HandSide.LEFT ? 3 : 4).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.01f)
                continue;

            // Decide from the grip button if we want push or grasp
            if (VG_Controller.IsIndexPushInteractionForSide(hand.side))
            {
                SelectToPush(hand);

                // Disable the selected object if it's not a pushable one.
                if (hand.selectedObject != null)
                {
                    VG_Articulation art = hand.selectedObject.GetComponent<VG_Articulation>();
                    if (art != null && art.m_type != VG_JointType.PRISMATIC)
                        hand.selectedObject = null;
                }
            }
            else
            {
                SelectToGrasp(hand);

                // Disable the selected object if it's not a graspable one.
                if (hand.selectedObject != null)
                {
                    VG_Articulation art = hand.selectedObject.GetComponent<VG_Articulation>();
                    if (art != null && art.m_type == VG_JointType.PRISMATIC)
                        hand.selectedObject = null;
                }
            }
        }
    }
}