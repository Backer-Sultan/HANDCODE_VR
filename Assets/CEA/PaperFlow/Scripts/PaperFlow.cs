using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperFlow : MonoBehaviour
{
  /*public struct Spool
  {
    public CapsuleCollider capsule;
    public enum Side { UNDER, ABOVE};
    public Side side;
  }

  public Spool[] spools;*/

  TwoCirclesTangents tangents;

  public CapsuleCollider[] capsules;

  public enum Side { UNDER, ABOVE };
  public Side[] sides;

  public Transform[] keypoints;
	public float width;
	public Material material;
	public Vector2 TopLeftUV, BottomRightUV;

  public int subdivision = 20;

	Mesh meshFront,meshBack;
	GameObject paperFront,paperBack;
	// Use this for initialization
	void Start()
	{
    tangents = new TwoCirclesTangents();

    if (keypoints.Length == 0)
    {
      int keypointCount = (capsules.Length - 1) * 2 + Mathf.Max(0, (capsules.Length - 2)) * subdivision;
      keypoints = new Transform[keypointCount];

      for (int i = 0; i < keypoints.Length; i++)
      {
        GameObject o = new GameObject("Paper_Keypoint_" +i );
        o.transform.parent = this.transform;
        keypoints[i] = o.transform;
      }

      BuildKeypoints();
    }

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

  void BuildKeypoints()
  {
    int kpIndex = 0;
    bool sideSwitch = false ;
    for(int i = 0;i<capsules.Length-1;i++)
    {
      bool skip = false;
      //Defining center and radius of circle 0 and on which side has to pass the tangent (inner or outer)
      Vector3 center0 = capsules[i].transform.TransformPoint(capsules[i].center);
      tangents.c0 = center0;
      tangents.r0 = capsules[i].radius;
      if (sides[i] == Side.ABOVE)
        tangents.side0 = true;
      else
        tangents.side0 = false;

      //Defining center and radius of circle 1 and on which side has to pass the tangent (inner or outer)
      Vector3 center1 = capsules[i + 1].transform.TransformPoint(capsules[i + 1].center);
      tangents.c1 = center1;
      tangents.r1 = capsules[i+1].radius;
      if (sides[i+1] == Side.ABOVE)
        tangents.side1 = true;
      else
        tangents.side1 = false;

      /*if (center0.x > center1.x)
      {
        if (sideSwitch)
          tangents.side0 = !tangents.side0;

        tangents.side1 = !tangents.side1;
        sideSwitch = true;
      }
      else if (sideSwitch)
      {
        tangents.side0 = !tangents.side0;
        sideSwitch = false;
      }
      else
      {
        sideSwitch = false;
      }*/

      tangents.CalculateTangents();

      //Connection the tangents with an arc of a circle
      if (i>0)
      {
        Vector3 p1 = keypoints[kpIndex - 2].position;
        Vector3 p2 = new Vector3(tangents.it1.x, tangents.it1.y, center1.z);
        Vector3 p3 = p1 + Vector3.forward;
        Vector3 n = Vector3.Cross(p1 - p3, p1 - p2);
        float d = -Vector3.Dot(n, p1);

        float distance = Vector3.Dot(n, center0) + d;
        if ((distance + capsules[i].radius < 0 && sides[i] == Side.ABOVE) || (distance - capsules[i].radius > 0 && sides[i] == Side.UNDER)) //no possible contact with the current spool
        {
          //skip = true;
          //kpIndex--;
        }

        if (!skip)
        {
          Vector3 ori = keypoints[kpIndex - 1].position - center0;
          Vector3 des = new Vector3(tangents.it0.x - center0.x, tangents.it0.y - center0.y, 0);

          Vector3 axis = Vector3.forward;
          if (tangents.side0)
            axis = -Vector3.forward;

          Vector3 signRef = Vector3.Cross(ori, axis);

          float angle = Vector3.Angle(ori, des);
          if (Vector3.Angle(signRef, des) < 90)
          {
            angle = 360 - angle;
          }

          Quaternion quat = Quaternion.AngleAxis(angle / (float)subdivision, axis);

          Vector3 step = ori;
          for (int j = 0; j < subdivision; j++)
          {
            step = quat * step;
            keypoints[kpIndex].position = center0 + step;
            kpIndex++;
          }
        }
      }

      if (!skip)
      {
        keypoints[kpIndex].position = new Vector3(tangents.it0.x, tangents.it0.y, center0.z);
        kpIndex++;
        keypoints[kpIndex].position = new Vector3(tangents.it1.x, tangents.it1.y, center1.z);
        kpIndex++;
      }
      else
      {
        keypoints[kpIndex].position = new Vector3(tangents.it1.x, tangents.it1.y, center1.z);
        kpIndex++;
      }
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
    BuildKeypoints();

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
    mesh.RecalculateBounds();
  }
}
