using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothDisplayBackSide : MonoBehaviour
{
  public Material BackFaceMaterial;

  Cloth cloth;
	GameObject clothBackSide;
	Mesh mesh;

	// Use this for initialization
	void Start () {
		cloth = GetComponent<Cloth>();
		clothBackSide = new GameObject("ClothBackSide");
		clothBackSide.transform.SetParent(transform);
		clothBackSide.transform.localPosition = Vector3.zero;
		clothBackSide.transform.localRotation = Quaternion.identity;
		clothBackSide.transform.localScale = Vector3.one;

		MeshFilter mf = clothBackSide.AddComponent<MeshFilter>();
		mf.mesh = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		mesh = mf.mesh;
		mesh.MarkDynamic();

		Vector3[] normals = mesh.normals;
		for (int i = 0; i < normals.Length; i++)
			normals[i] = -normals[i];
		mesh.normals = normals;

		for (int m = 0; m < mesh.subMeshCount; m++)
		{
			int[] triangles = mesh.GetTriangles(m);
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int temp = triangles[i + 0];
				triangles[i + 0] = triangles[i + 1];
				triangles[i + 1] = temp;
			}
			mesh.SetTriangles(triangles, m);
		}
		MeshRenderer mr = clothBackSide.AddComponent<MeshRenderer>();

    if (BackFaceMaterial == null)
      mr.material = GetComponent<SkinnedMeshRenderer>().material;
    else
      mr.material = BackFaceMaterial;
  }
	
	// Update is called once per frame
	void Update () {
		mesh.vertices = cloth.vertices;
		mesh.RecalculateNormals();
	}
}
