using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClothManipulation : MonoBehaviour
{

  public Cloth cloth;
  public Transform[] targetPoints = new Transform[2];
  private int[] manipulatedPoints = new int[2];
  private Mesh mesh;
  private Vector3[] initialVertices;


  // Use this for initialization
  void Awake()
  {
    if (cloth)
    {
      ClothSkinningCoefficient[] coefficients;

      //saving inital mesh shape 
      initialVertices = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;

      mesh = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh;
      coefficients = cloth.coefficients;
    }
  }

  void Start()
  {
    //identification of the parent cloth vertex jointset them free
    ClothSkinningCoefficient[] coefficients;
    coefficients = cloth.coefficients;

    for (int i = 0; i < coefficients.Length; i++)
      coefficients[i].maxDistance = 0;

    for (int i = 0; i < manipulatedPoints.Length; i++)
      manipulatedPoints[i] = -1;

      cloth.coefficients = coefficients;
  }

  // Update is called once per frame
  void Update()
  {
    if (cloth)
    {
      Vector3[] vertices = mesh.vertices;
      for (int i = 0; i < manipulatedPoints.Length; i++)
      {
        if(manipulatedPoints[i]!=-1)
          vertices[manipulatedPoints[i]] = cloth.transform.InverseTransformPoint(targetPoints[i].position);
      }

      mesh.vertices = vertices;
    }
  }

  public void OnApplicationQuit()
  {
    mesh.vertices = initialVertices;
  }

  public void AttachTarget(int targetId)
  {
    if (targetPoints.Length > targetId && targetPoints[targetId] != null)
    {
      ClothSkinningCoefficient[] coefficients = cloth.coefficients;

      //Look for closest point from target
      float minDistance = float.MaxValue;
      int minId = -1;
      Vector3 target = cloth.transform.InverseTransformPoint(targetPoints[targetId].position);
      for (int i = 0; i < cloth.vertices.Length; i++)
      {
        float distance = Vector3.Distance(cloth.vertices[i], target);
        if (minDistance > distance)
        {
          minDistance = distance;
          minId = i;
        }
      }
      if (minId != -1)
      {
        manipulatedPoints[targetId] = minId;

        for (int i = 0; i < coefficients.Length; i++)
          coefficients[i].maxDistance = float.MaxValue;

        coefficients[manipulatedPoints[targetId]].maxDistance = 0;
        cloth.coefficients = coefficients;
      }
    }
  }

  public bool IsTargetAttach(int targetId)
  {
    if (manipulatedPoints.Length > targetId &&  manipulatedPoints[targetId] > -1)
      return true;
    else
      return false;
  }

  public void DetachTarget(int targetId)
  {
    if (targetPoints.Length > targetId && manipulatedPoints[targetId] > -1)
    {
      ClothSkinningCoefficient[] coefficients = cloth.coefficients;

      coefficients[manipulatedPoints[targetId]].maxDistance = float.MaxValue;
      manipulatedPoints[targetId] = -1;

      //Checking if no attachment remains 
      bool noTarget = true;
      for (int i = 0; i < manipulatedPoints.Length; i++)
      {
        if (manipulatedPoints[targetId] > -1)
        {
          noTarget = false;
          break;
        }
      }

      //if no attachment remains then the cloth is freezed
      if (noTarget)
      {

        for (int i = 0; i < coefficients.Length; i++)
          coefficients[i].maxDistance = 0;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < cloth.vertices.Length; i++)
        {
           vertices[i] = cloth.vertices[i];
        }
        mesh.vertices = vertices;
      }
      cloth.coefficients = coefficients;
    }
  }

}
