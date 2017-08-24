using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PaperFlow : MonoBehaviour
{
  TwoCirclesTangents tangents;

  public List<CapsuleCollider> rollers;
  public List<bool> sides;
  public float width = 1;
  public Material material;
  public Vector2 topLeftUV = new Vector2(0,0), bottomRightUV = new Vector2(1, 1);

  public int subdivision = 20;

  [HideInInspector]
  public bool showAdvance = false;

  [HideInInspector]
  public Transform[] keypoints;
  Mesh meshFront, meshBack;
  GameObject paperFront, paperBack;
  private int rollerDir = -1;
  private bool triangleMeshRefresh = false;

  // Use this for initialization
  void Start()
  {
    BuildKeypoints();

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

      /*UpdateMesh(meshFront);
      UpdateMesh(meshBack);
      UpdateTriangles();*/

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


  public void BuildKeypoints()
  {
    if (tangents == null)
      tangents = new TwoCirclesTangents();

    if (rollers != null)
    {
      int keypointCount = (rollers.Count - 1) * 2 + Mathf.Max(0, (rollers.Count - 2)) * subdivision;
      if (keypointCount > 0)
      {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
          if (transform.GetChild(i).GetComponent<MeshRenderer>() == null) //Avoid destruction of paper mesh
          {
            if (Application.isPlaying)
              GameObject.Destroy(transform.GetChild(i).gameObject);
            else
              GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
          }
        }

        keypoints = new Transform[keypointCount];

        for (int i = 0; i < keypoints.Length; i++)
        {
          GameObject o = new GameObject("Paper_Keypoint_" + i);
          o.transform.parent = this.transform;
          o.transform.position = transform.position;
          keypoints[i] = o.transform;
        }

        UpdateKeypoints();

        if (Application.isPlaying && keypoints.Length > 1)
        {
          triangleMeshRefresh = true;
        }
      }
    }
  } 

  public void UpdateKeypoints()
  {
    int kpIndex = 0;

    for (int i = 0; i < rollers.Count - 1; i++)
    {
      if (rollers[i] != null && rollers[i+1] != null)
      {
        if (rollerDir == -1)
          rollerDir = rollers[0].direction;

          //Defining center and radius of circle 0 and on which side has to pass the tangent (inner or outer)
         Vector3 center0 = rollers[i].transform.TransformPoint(rollers[i].center);
        if(rollerDir == 0)
          tangents.c0 = new Vector2(center0.y, center0.z);
        else if (rollerDir == 1)
          tangents.c0 = new Vector2(center0.x, center0.z);
        else
          tangents.c0 = center0;

        tangents.r0 = rollers[i].radius;
        tangents.side0 = sides[i];

        //Defining center and radius of circle 1 and on which side has to pass the tangent (inner or outer)
        Vector3 center1 = rollers[i + 1].transform.TransformPoint(rollers[i + 1].center);

        if (rollerDir == 0)
          tangents.c1 = new Vector2(center1.y, center1.z);
        else if (rollerDir == 1)
          tangents.c1 = new Vector2(center1.x, center1.z);
        else
          tangents.c1 = center1;

        tangents.r1 = rollers[i + 1].radius;
        tangents.side1 = sides[i + 1];

        tangents.CalculateTangents();

        //Connection the tangents with an arc of a circle
        if (i > 0)
        {
          Vector3 ori = keypoints[kpIndex - 1].position - center0;
          Vector3 des;
          if (rollerDir == 0)
            des = new Vector3(0,tangents.it0.x - center0.y, tangents.it0.y - center0.z);
          else if (rollerDir == 1)
            des = new Vector3(tangents.it0.x - center0.x, 0, tangents.it0.y - center0.z);
          else
            des = new Vector3(tangents.it0.x - center0.x, tangents.it0.y - center0.y, 0);

          Vector3 axis;
          if (rollerDir == 0)
            axis = rollers[i].transform.right; 
          else if (rollerDir == 1)
            axis = rollers[i].transform.up; 
          else
            axis = rollers[i].transform.forward;

          if (tangents.side0)
            axis = -axis;

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
            keypoints[kpIndex].rotation = rollers[i].transform.rotation;
            kpIndex++;
          }
        }

        Vector3 it0;
        if (rollerDir == 0)
          it0 = new Vector3(center0.x, tangents.it0.x, tangents.it0.y);
        else if (rollerDir == 1)
          it0 = new Vector3(tangents.it0.x, center0.y, tangents.it0.y);
        else
          it0 = new Vector3(tangents.it0.x, tangents.it0.y, center0.z);

        it0 -= rollers[i].transform.position + rollers[i].center;
        it0 = rollers[i].transform.rotation * it0;
        it0 += rollers[i].transform.position + rollers[i].center;

        keypoints[kpIndex].position = it0;
        keypoints[kpIndex].rotation = rollers[i].transform.rotation;
        kpIndex++;

        Vector3 it1;
        if (rollerDir == 0)
          it1 = new Vector3(center1.x, tangents.it1.x, tangents.it1.y);
        else if (rollerDir == 1)
          it1 = new Vector3(tangents.it1.x, center1.y, tangents.it1.y);
        else
          it1 = new Vector3(tangents.it1.x, tangents.it1.y, center1.z);

        it1 -= rollers[i + 1].transform.position + rollers[i + 1].center;
        it1 = rollers[i + 1].transform.rotation * it1;
        it1 += rollers[i + 1].transform.position + rollers[i + 1].center;

        keypoints[kpIndex].position = it1;
        keypoints[kpIndex].rotation = rollers[i + 1].transform.rotation;
        kpIndex++;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    UpdateMesh(meshFront);
    UpdateMesh(meshBack);
    UpdateTriangles();
  }

  void UpdateTriangles()
  {
    if (triangleMeshRefresh)
    {
      //Building triangle list
      int[] trianglesFront = new int[(keypoints.Length - 1) * 6];
      int[] trianglesBack = new int[(keypoints.Length - 1) * 6];
      int idf = 0;
      int idb = 0;
      for (int i = 0; i < meshFront.vertices.Length - 3; i += 2)
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

      triangleMeshRefresh = false;
    }
  }

  void UpdateMesh(Mesh mesh)
  {
    UpdateKeypoints();

    //updating vertices list
    List<Vector3> vertices = new List<Vector3>();

    Vector3 side;
    if(rollerDir == 0)
      side = keypoints[0].right;
    else if (rollerDir == 1)
      side = keypoints[0].up;
    else
      side = keypoints[0].forward;

    side.Normalize();
    side *= 0.5f * width;

    vertices.Add(keypoints[0].position + side);
    vertices.Add(keypoints[0].position - side);

    for (int i = 1; i < keypoints.Length; i++)
    {
      if (rollerDir == 0)
        side = keypoints[i].right;
      else if (rollerDir == 1)
        side = keypoints[i].up;
      else
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

    uvs.Add(new Vector2(topLeftUV.x, topLeftUV.y));
    uvs.Add(new Vector2(bottomRightUV.x, topLeftUV.y));

    for (int i = 2; i < vertices.Count; i++)
    {
      //texture coordonate paper Side 1
      paperDistanceS1 += Vector3.Distance(vertices[i - 2], vertices[i]);
      float t1 = (paperDistanceS1 / width) * bottomRightUV.y;
      uvs.Add(new Vector2(topLeftUV.x, t1));

      //texture coordonate paper Side 2
      i++;
      paperDistanceS2 += Vector3.Distance(vertices[i - 2], vertices[i]);
      float t2 = (paperDistanceS2 / width) * bottomRightUV.y;
      uvs.Add(new Vector2(bottomRightUV.x, t2));
    }

    mesh.SetUVs(0, uvs);
    mesh.SetUVs(1, uvs);

    //Building normals
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();
  }
}
