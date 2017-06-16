using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperFlow : MonoBehaviour
{

	public Transform[] keypoints;
	public float width;
	public Material material;
	public Vector2 TopLeftUV, BottomRightUV;

	Mesh meshFront,meshBack;
	GameObject paperFront,paperBack;
	// Use this for initialization
	void Start()
	{
		paperFront = new GameObject("PaperFront");
		paperBack = new GameObject("PaperBack");
		paperFront.transform.SetParent(transform);
		paperBack.transform.SetParent(transform);

		meshFront = new Mesh();
		meshFront.MarkDynamic();

		meshBack = new Mesh();
		meshBack.MarkDynamic();

		if (keypoints.Length > 1)
		{
			UpdateMesh(meshFront);
			UpdateMesh(meshBack);

			//Building triangle list
			int[] trianglesFront = new int[(keypoints.Length - 1) * 6];
			int[] trianglesBack = new int[(keypoints.Length - 1) * 6];
			int idf = 0;
			int idb = 0;
			for (int i = 0; i < meshFront.vertices.Length - 3; i+=2)
			{
				trianglesFront[idf] = i; idf++;
				trianglesFront[idf] = i + 1; idf++;
				trianglesFront[idf] = i + 2; idf++;
				trianglesFront[idf] = i + 1; idf++;
				trianglesFront[idf] = i + 3; idf++;
				trianglesFront[idf] = i + 2; idf++;

				trianglesBack[idb] = i; idb++;
				trianglesBack[idb] = i + 2; idb++;
				trianglesBack[idb] = i + 1; idb++;
				trianglesBack[idb] = i + 1; idb++;
				trianglesBack[idb] = i + 2; idb++;
				trianglesBack[idb] = i + 3; idb++;
			}

			meshFront.SetTriangles(trianglesFront, 0);
			meshBack.SetTriangles(trianglesBack, 0);

			MeshFilter mff = paperFront.AddComponent<MeshFilter>();
			mff.mesh = meshFront;
			MeshRenderer mrf = paperFront.AddComponent<MeshRenderer>();
			mrf.material = material;

			MeshFilter mfb = paperBack.AddComponent<MeshFilter>();
			mfb.mesh = meshBack;
			MeshRenderer mrb = paperBack.AddComponent<MeshRenderer>();
			mrb.material = material;
		}
	}

	// Update is called once per frame
	void Update()
	{
		UpdateMesh(meshFront);
		UpdateMesh(meshBack);
	}

	void UpdateMesh(Mesh mesh)
	{
		//updating vertices list
		List<Vector3> vertices = new List<Vector3>();

		Vector3 side = keypoints[0].forward; 
		side.Normalize();
		side *= 0.5f * width;

		vertices.Add(keypoints[0].position + side);
		vertices.Add(keypoints[0].position - side);

		for (int i = 1; i < keypoints.Length; i++)
		{
			side = keypoints[i].forward;
			side.Normalize();
			side *= 0.5f * width;
			vertices.Add(keypoints[i].position + side);
			vertices.Add(keypoints[i].position - side);
		}

		mesh.SetVertices(vertices);

		//measuring paper length
		float paperLength = 0;
		for (int i = 2; i < vertices.Count; i += 2)
			paperLength += Vector3.Distance(vertices[i - 2], vertices[i]);

		//updating coordonate
		float paperDistanceS1 = 0;
		float paperDistanceS2 = 0;
		List<Vector2> uvs = new List<Vector2>();

		uvs.Add(new Vector2(TopLeftUV.x, TopLeftUV.y));
		uvs.Add(new Vector2(BottomRightUV.x, TopLeftUV.y));

		for (int i = 2; i < vertices.Count; i ++)
		{
			//texture coordonate paper Side 1
			
			paperDistanceS1 += Vector3.Distance(vertices[i - 2], vertices[i]);
			float t1 = paperDistanceS1 / paperLength;
			float y1 = TopLeftUV.y * (1 - t1) + BottomRightUV.y * t1;
			uvs.Add(new Vector2(TopLeftUV.x, y1));

			//texture coordonate paper Side 2
			i++;
			paperDistanceS2 += Vector3.Distance(vertices[i - 2], vertices[i]);
			float t2 = paperDistanceS2 / paperLength;
			float y2 = TopLeftUV.y * (1 - t2) + BottomRightUV.y * t2;
			uvs.Add(new Vector2(BottomRightUV.x, y2));
		}

		mesh.SetUVs(0, uvs);
		mesh.SetUVs(1, uvs);

		//Building normals
		mesh.RecalculateNormals();
	}
}
