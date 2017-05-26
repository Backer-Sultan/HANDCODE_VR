using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothJoint : MonoBehaviour {

	public Cloth clothParent;
	public Cloth clothChild;

	Mesh meshChild;

	int[] jointPointsIdParent;
	int[] jointPointsIdChild;

	int[] jointCutId;
	int[] jointCutPairId;

	public Transform knife;
	public float cuttingDistance = 0.02f;

	// Use this for initialization
	void Start () {

		ClothSkinningCoefficient[] coefficients;

		//identification of the parent cloth vertex joint + set them free
		coefficients = clothParent.coefficients;

		int count = 0;
		for (int i = 0; i < coefficients.Length; i++)
			if (coefficients[i].maxDistance == 1)
				count++;

		int index = 0;
		jointPointsIdParent = new int[count];
		for (int i = 0; i < coefficients.Length; i++)
			if (coefficients[i].maxDistance == 1)
			{
				coefficients[i].maxDistance = float.MaxValue;
				jointPointsIdParent[index] = i;
				index++;
			}

		clothParent.coefficients = coefficients;

		//identification of the child cloth vertex joint
		coefficients = clothChild.coefficients;

		count = 0;
		for (int i = 0; i < coefficients.Length; i++)
			if (coefficients[i].maxDistance <= 2)
				count++;

		index = 0;
		jointPointsIdChild = new int[count];
		for (int i = 0; i < coefficients.Length; i++)
		{
			if (coefficients[i].maxDistance <= 2)
			{
				jointPointsIdChild[index] = i;
				index++;
			}
		}

		//Reorganising the child vertices Ids to match with closest parent vertice Id
		int[] adjustedIds = new int[jointPointsIdChild.Length];
		for (int i = 0; i < jointPointsIdParent.Length; i++)
		{
			Vector3 v = clothParent.vertices[jointPointsIdParent[i]];
			v = clothParent.transform.TransformPoint(v); //local parent to world
			v = clothChild.transform.InverseTransformPoint(v); //world to local child
			float min = float.MaxValue;
			for (int j = 0; j < jointPointsIdChild.Length; j++)
			{
				float dist = Vector3.Distance(v, clothChild.vertices[jointPointsIdChild[j]]);
				if (dist < min)
				{
					min = dist;
					adjustedIds[i] = jointPointsIdChild[j];
				}
			}
		}

		jointPointsIdChild = adjustedIds;

		//copying  mesh befor modifiying it
		meshChild = (Mesh)Instantiate(clothChild.GetComponent<SkinnedMeshRenderer>().sharedMesh);
		clothChild.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshChild;

		//identification of the vertex joints that can be cut and their pair vertex that will be remove with them
		jointCutId = new int[jointPointsIdChild.Length / 2];
		jointCutPairId = new int[jointPointsIdChild.Length / 2];
		int indexCut = 0;
		int indexCutPair = 0;
		for (int i = 0; i < jointPointsIdChild.Length; i++)
		{
			if (coefficients[jointPointsIdChild[i]].maxDistance == 1)
			{
				jointCutId[indexCut] = jointPointsIdChild[i];
				indexCut++;

				//shearching its vertex joint pair
				float min = float.MaxValue;
				for (int j = 0; j < jointPointsIdChild.Length; j++)
				{
					if (coefficients[jointPointsIdChild[j]].maxDistance == 2)
					{
						float dist = Vector3.Distance(meshChild.vertices[jointPointsIdChild[i]], meshChild.vertices[jointPointsIdChild[j]]);
						if (dist < min)
						{
							min = dist;
							jointCutPairId[indexCutPair] = jointPointsIdChild[j];
							//coefficients[jointPointsIdChild[j]].maxDistance = 0;
						}
					}
				}
				indexCutPair++;
			}
		}


		//setting free the child vertex joints
		for (int i = 0; i < jointPointsIdChild.Length; i++)
			coefficients[jointPointsIdChild[i]].maxDistance = 0;

		clothChild.coefficients = coefficients;

	}
	
	// Update is called once per frame
	void Update () {

		//Joint update
		Vector3[] vertices = meshChild.vertices;
		for (int i=0;i< jointPointsIdParent.Length;i++)
		{
				Vector3 v = clothParent.vertices[jointPointsIdParent[i]];
				v = clothParent.transform.TransformPoint(v); //local parent to world
				v = clothChild.transform.InverseTransformPoint(v); //world to local child
				vertices[jointPointsIdChild[i]] = v;
		}
		meshChild.vertices = vertices;

		//Knife Cut
		Vector3 localKnife = clothChild.transform.InverseTransformPoint(knife.position);

		ClothSkinningCoefficient[] coefficients = clothChild.coefficients;
		for (int i = 0; i < jointCutId.Length; i++)
		{
			if (coefficients[jointCutId[i]].maxDistance == 0)
			{
				float dist = Vector3.Distance(clothChild.vertices[jointCutId[i]], localKnife);
				if (dist < cuttingDistance)
				{
					coefficients[jointCutId[i]].maxDistance = float.MaxValue;
					coefficients[jointCutPairId[i]].maxDistance = float.MaxValue;
				}
			}
		}
		clothChild.coefficients = coefficients;
	}
}
