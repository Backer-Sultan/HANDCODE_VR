// Copyright (C) 2014-2017 Gleechi AB. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VirtualGrasp;

public class InteractionParams
{
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
    static private Dictionary<string, InteractionParams> m_interactionParameters = new Dictionary<string, InteractionParams>();

    public Shader shader = null;
    private Color[] colors = new Color[] { Color.magenta, Color.green, Color.white };
    
    private int[] highlighted = new int[2] { -1, -1 };

    private Material[] highLightMaterials = new Material[2] { null, null };
    private Material[] unhighLightedMaterials = new Material[2] { null, null };
    private GameObject[] highlightedObjects = new GameObject[2] { null, null };

    // The seed points
    List<KeyValuePair<Vector3, float>> seedPts = new List<KeyValuePair<Vector3, float>>();
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

    public HandCodeObjectSelection(Shader shader_)
    {
        shader = shader_;

        sunflower(50, 0.05f);

        m_interactionParameters["Grasp"] = new InteractionParams(0.25f, 30.0f, 1.0f);
        m_interactionParameters["Push"] = new InteractionParams(0.075f, 90.0f, 0.75f);
    }

    public void Highlight(VG_HandStatus hand)
    {
        if (hand.selectedObject == null || hand.selectedObject.GetComponent<MeshRenderer>() == null)
            return;
        
        int id = hand.side < 0 ? 0 : 1;
        highlightedObjects[id] = hand.selectedObject.gameObject;
        unhighLightedMaterials[id] = new Material(hand.selectedObject.GetComponent<MeshRenderer>().material);
        highLightMaterials[id] = new Material(hand.selectedObject.GetComponent<MeshRenderer>().material);
        highLightMaterials[id].shader = shader;
        highlightedObjects[id].GetComponent<MeshRenderer>().material = highLightMaterials[id];
        highlightedObjects[id].GetComponent<MeshRenderer>().material.SetColor("_RimColor", colors[id]);
    }

    public void ClearHighlight(Transform obj)
    {
        for (int i = 0; i < 2; i++)
        {
            if (highlightedObjects[i] == null)
                continue;
            if (highlightedObjects[i].transform == obj)
                highlightedObjects[i].GetComponent<MeshRenderer>().material = unhighLightedMaterials[i];
        }
    }

    public void ClearHighlight(int i)
    {
        if (highlighted[i] >= 0) highlighted[i] = -1;
        else
        {
            if (highlightedObjects[i] == null)
                return;
            highlightedObjects[i].GetComponent<MeshRenderer>().material = unhighLightedMaterials[i];
            highlightedObjects[i] = null;
        }
    }

    public void HighlightObjects(VG_HandStatus[] current)
    {
        ClearHighlight(0);
        ClearHighlight(1);
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

            for (int i = 0; i < current.Length; i++)
            {
                // Switch off highlight for former selected object if it switched
                if (highlightedObjects[i] != current[i].selectedObject && highlightedObjects[i] != null)
                {
                    ClearHighlight(highlightedObjects[i].transform);
                }

                // If there's no selected object, we have nothing to highlight
                if (current[i].selectedObject == null)
                    continue;

                // If the hand is not empty, we have nothing to highlight
                if (current[i].mode != VG_InteractionMode.EMPTY)
                    continue;

                // If there's no selected grasp, we have nothing to highlight
                //if (current [i].graspStatus != VG_ReturnCode.SUCCESS)
                //	continue;

                Highlight(current[i]);
            }
    }

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

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Object"))
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
                    selectedObject = obj.transform;
                }
            }
            else if (nxt_distance < distance || distance < 0)
            {
                min_cnt_distance = cnt_distance;
                distance = nxt_distance;
                selectedObject = obj.transform;
            }
        }
    }

    public void SelectToPush(VG_HandStatus hand)
    {
        Vector3 p;
        Quaternion q;
        int iid;

        // Get the pose of the first avatar's (1) index finger's (1) last (-1) limb, i.e. the fingertip.
        VG_Controller.GetFingerBone(1, hand.side, 1, -1, out iid, out p, out q);

        SelectObjectBySeedRaycasts("Push", p, Quaternion.LookRotation(q * (Vector3.up + Vector3.forward)), out hand.selectedObject, out hand.distance);
        if (hand.selectedObject != null) return;

        //SelectObjectByClosestBound("Push", p, q * Vector3.up, out hand.selectedObject, out hand.distance);
    }

    public void SelectToGrasp(VG_HandStatus hand)
    {
        Vector3 p;
        Quaternion q;
        int iid;

        // Get the pose of the grasp approach center.
        VG_Controller.GetBone(1, hand.side, VG_BoneType.APPROACH, out iid, out p, out q);

        // Check raycast if direct hit
        //if (CheckRaycast (p, dir, hand)) continue;

        SelectObjectBySeedRaycasts("Grasp", p, Quaternion.LookRotation(q * Vector3.up), out hand.selectedObject, out hand.distance);
        if (hand.selectedObject != null) return;

        SelectObjectByClosestBound("Grasp", p, q * Vector3.up, out hand.selectedObject, out hand.distance);
        if (hand.selectedObject == null) return;

        // Check raycast if another object is hit before
        CheckRaycast(p, hand.selectedObject.GetComponent<Collider>().bounds.ClosestPoint(p) - p, m_interactionParameters["Grasp"].m_distanceThreshold, out hand.selectedObject, out hand.distance);
    }

    public void Select(VG_HandStatus[] status)
    {
        ClearHighlight(0);
        ClearHighlight(1);
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
            }
            else
            {
                SelectToGrasp(hand);
            }
        }
    }
}

