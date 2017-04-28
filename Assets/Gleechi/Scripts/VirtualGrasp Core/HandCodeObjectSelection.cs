// Copyright (C) 2014-2017 Gleechi AB. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VirtualGrasp;

public class HandCodeObjectSelection
{
	public const float _selectionThreshold = 0.25f;
	public const float _selectionAngleThreshold = 30;

	public Shader shader = null;
	private Color[] colors = new Color[] {Color.magenta, Color.green, Color.white};

	private HashSet<int> _selectionLayers = new HashSet<int>();
	private int[] highlighted = new int[2] {-1, -1};

	private Material[] highLightMaterials = new Material[2] {null, null};
	private Material[] unhighLightedMaterials = new Material[2] {null, null};
	private GameObject[] highlightedObjects = new GameObject[2] {null, null};

	// The seed points
	List<KeyValuePair<Vector3, float>> seedPts = new List<KeyValuePair<Vector3, float>>();

	// The sunflower algorithm is producing seed points uniformly in a unit circle
	void sunflower(int numRays, float radius)
	{
		float r1, r2, theta;
		seedPts.Clear ();
		seedPts.Add(new KeyValuePair<Vector3, float>(new Vector3(0, 1, 0), 1.0f)); // center
		float goldenRatio = 2 * Mathf.PI / Mathf.Pow((Mathf.Sqrt(5) + 1) / 2, 2);
		for (int k = 1; k < numRays; k++)
		{
			if (k > numRays) continue;
			r1 = Mathf.Sqrt(k - 1 / 2.0f) / Mathf.Sqrt(numRays - 1);
			r2 = radius * r1;
			theta = goldenRatio * k;
			seedPts.Add(new KeyValuePair<Vector3, float>(new Vector3(r2 * Mathf.Cos(theta), 1, r2 * Mathf.Sin(theta)).normalized, 1 - r1));
		}
	}

	public HandCodeObjectSelection(Shader shader_)
	{
		shader = shader_;

		_selectionLayers.Add(LayerMask.NameToLayer("Objects"));

		sunflower (50, 0.05f);
    }

	public void Highlight(VG_HandStatus hand)
	{
		if (hand.selectedObject == null)
			return;
		
		int id = hand.side < 0 ? 0 : 1;
		highlightedObjects [id] = hand.selectedObject.gameObject;
		unhighLightedMaterials [id] = new Material (hand.selectedObject.GetComponent<MeshRenderer> ().material);
		highLightMaterials [id] = new Material (hand.selectedObject.GetComponent<MeshRenderer> ().material);
		highLightMaterials [id].shader = shader;
		highlightedObjects [id].GetComponent<MeshRenderer> ().material = highLightMaterials [id];
		highlightedObjects [id].GetComponent<MeshRenderer> ().material.SetColor ("_RimColor", colors [id]);
    }

	public void ClearHighlight(Transform obj)
	{
		for (int i = 0; i < 2; i++) 
		{
			if (highlightedObjects [i] == null)
				continue;
			if (highlightedObjects [i].transform == obj)
				highlightedObjects [i].GetComponent<MeshRenderer> ().material = unhighLightedMaterials [i];
		}
	}
    
	public void ClearHighlight(int i)
	{
		if (highlighted [i] >= 0) 
		{
			highlighted [i] = -1;
		} 
		else
		{
			if (highlightedObjects [i] == null)
				return;
			highlightedObjects [i].GetComponent<MeshRenderer> ().material = unhighLightedMaterials [i];
			highlightedObjects [i] = null;
		}
	}

	public void HighlightObjects(VG_HandStatus[] current)
	{
		if (current[0] == null || current [1] == null)
			return;
		
		ClearHighlight (0);
		ClearHighlight (1);
		if (current[0].grasp >= 0 && current[1].grasp >= 0 && current[0].selectedObject != null && current[0].selectedObject == current[1].selectedObject)
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
		else 

		for (int i = 0; i < current.Length; i++)
		{
			// Switch off highlight for former selected object if it switched
			if (highlightedObjects[i] != current [i].selectedObject && highlightedObjects[i] != null) 
			{
				ClearHighlight (highlightedObjects[i].transform);
			}

			// If there's no selected object, we have nothing to highlight
			if (current [i].selectedObject == null)
				continue;

			// If the hand is not empty, we have nothing to highlight
			if (current [i].mode != VG_InteractionMode.EMPTY)
				continue;
			
			// If there's no selected grasp, we have nothing to highlight
			if (current [i].grasp < 0)
				continue;
			
			Highlight (current[i]);
		}
	}

	private bool CheckRaycast(Vector3 p, Vector3 dir, VG_HandStatus hand)
	{
		Ray ray = new Ray(p, dir);

		RaycastHit raycasthit = new RaycastHit();
		if (Physics.Raycast(ray, out raycasthit, _selectionThreshold))
		{
			if (_selectionLayers.Contains (raycasthit.transform.gameObject.layer))
			{
				hand.selectedObject = raycasthit.transform;
				hand.distance = raycasthit.distance;
				Debug.DrawLine (p, p + raycasthit.distance * dir, Color.green);
				return true;
			}
		}

		Debug.DrawLine (p, p + _selectionThreshold * dir, Color.white);

		return false;
	}
		
	public void Select(VG_HandStatus[] status)
	{
		float nxt_distance;
		float cnt_distance;
		float dot;
		float angle;
		Vector3 p;
		Quaternion q;
		Vector3 dir;
		float min_cnt_distance = float.MaxValue;

		foreach (VG_HandStatus hand in status) 
		{
			if (hand == null)
				continue;

			if (!hand.valid)
				continue;
			
			if (hand.mode != VG_InteractionMode.EMPTY)
				continue;
			
			hand.distance = -1;
			hand.selectedObject = null;

			if (hand.hand == null)
				continue;
			
			int iid;
			VG_Controller.GetBone(1, hand.side, VG_BoneType.APPROACH, out iid, out p, out q);

			dir = q * Vector3.up;
			p  += q * new Vector3(-.01f, -.01f, -.01f);
				
			// Check raycast if direct hit
			//if (CheckRaycast (p, dir, hand)) continue;

			cnt_distance = -1;

			float max_hits = 0;
			Dictionary<Transform, float> hits = new Dictionary<Transform, float> ();
			Transform selectedObj = null;
			foreach (KeyValuePair<Vector3, float> pair in seedPts) 
			{
				Vector3 seed = pair.Key;
				seed.y = 0;

				if (CheckRaycast (p + q * seed, q * Vector3.up, hand)) 
				{
					if (!hits.ContainsKey(hand.selectedObject)) hits.Add(hand.selectedObject, 0);
					hits[hand.selectedObject] += Mathf.Pow(pair.Value, 2);
					if (hits [hand.selectedObject] > max_hits) 
					{
						max_hits = hits [hand.selectedObject];
						selectedObj = hand.selectedObject;
					}
				}
			}
			hand.selectedObject = selectedObj;

			if (hand.selectedObject != null)
				continue;
			
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Object")) 
			{
				Collider r = obj.GetComponent<Collider> ();

				Vector3 closestPoint = r.bounds.ClosestPoint(p);
				nxt_distance = Vector3.Distance (p, closestPoint);
				if (nxt_distance > _selectionThreshold) 
					continue;

				dot = Vector3.Dot(dir, closestPoint - p);
				angle = Vector3.Angle (dir, closestPoint - p);

				// Note: when angle threshold is 90, this is the same
				if (dot < 0 || angle > _selectionAngleThreshold)
					continue;

				// draw white lines to show closest objects
				Debug.DrawLine (p, closestPoint); 

				// store minimum
				cnt_distance = (r.bounds.center - p).magnitude;
				if (Mathf.Abs (nxt_distance - hand.distance) < 0.01f) 
				{
					if (cnt_distance < min_cnt_distance) 
					{
						min_cnt_distance = cnt_distance;
						hand.distance = Mathf.Min(nxt_distance,hand.distance);
						hand.selectedObject = obj.transform;
					}
				}
				else if (nxt_distance < hand.distance || hand.distance < 0) 
				{
					min_cnt_distance = cnt_distance;
					hand.distance = nxt_distance;
					hand.selectedObject = obj.transform;
				}
			}

			if (hand.selectedObject == null) continue;

        	// Check raycast if another object is hit before
			if (!CheckRaycast (p, hand.selectedObject.GetComponent<Collider> ().bounds.ClosestPoint (p) - p, hand)) 
			{
				hand.distance = -1;
				hand.selectedObject = null;
			}
		}
	}
}

