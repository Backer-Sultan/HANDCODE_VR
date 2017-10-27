using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClothManipulation : MonoBehaviour
{

  public Cloth cloth;

  public Transform[] targetPoints = new Transform[2];
  private int[] manipulatedPoints;

  private Mesh mesh;
  private Vector3[] initialVertices;
  private HandCodeVirtualGrasp virtualGrasp;

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
      virtualGrasp = FindObjectOfType<HandCodeVirtualGrasp>();
    }
  }

  void Start()
  {
    if (cloth)
    {
      //identification of the parent cloth vertex jointset them free
      ClothSkinningCoefficient[] coefficients;
      coefficients = cloth.coefficients;

      for (int i = 0; i < coefficients.Length; i++)
        coefficients[i].maxDistance = 0;

      manipulatedPoints = new int[targetPoints.Length];
      for (int i = 0; i < manipulatedPoints.Length; i++)
        manipulatedPoints[i] = -1;

      cloth.coefficients = coefficients;
    }
  }

  void CheckPointer(VirtualGrasp.VG_HandSide side)
  {
    float dist, next_dist = float.MaxValue;
    int next_id = -1;
    int iid; Vector3 p; Quaternion q;
    VirtualGrasp.VG_Controller.GetBone(1, side, VirtualGrasp.VG_BoneType.APPROACH, out iid, out p, out q);
    for (int i = 0; i < cloth.vertices.Length; i++)
    {
      dist = Vector3.Distance(transform.position + transform.rotation * cloth.vertices[i], p);
      if (dist < next_dist)
      {
        next_dist = dist;
        next_id = i;
      }
    }

    if (next_id >= 0)
    {
      if (side == VirtualGrasp.VG_HandSide.LEFT) targetPoints[0].position = transform.position + transform.rotation * cloth.vertices[next_id];
      else if (side == VirtualGrasp.VG_HandSide.RIGHT) targetPoints[1].position = transform.position + transform.rotation * cloth.vertices[next_id];
    }
  }

  // Update is called once per frame

  void Update()
  {
    if (cloth)
    {
      if (virtualGrasp)
      {
        for (uint handID = 0; handID < 2; handID++)
        {
          if (virtualGrasp.GetCurrentSelection(handID) == targetPoints[handID])
          {
            if (manipulatedPoints[handID] == -1 && virtualGrasp.GetCurrentMode(handID) == VirtualGrasp.VG_InteractionMode.HOLD)
              AttachTarget((int)handID);
          }
          else if (manipulatedPoints[handID] != -1)
          {
            DetachTarget((int)handID);
          }
        }
      }

      Vector3[] vertices = mesh.vertices;
      for (int i = 0; i < manipulatedPoints.Length; i++)
      {
        if (manipulatedPoints[i] != -1)
        {
          vertices[manipulatedPoints[i]] = cloth.transform.InverseTransformPoint(targetPoints[i].position);
        }
        else
        {
          if (i == 0)
            CheckPointer(VirtualGrasp.VG_HandSide.LEFT);
          else if (i == 1)
            CheckPointer(VirtualGrasp.VG_HandSide.RIGHT);
        }
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

        for(int i = 0;i< manipulatedPoints.Length; i++)
          if(manipulatedPoints[i]!=-1)
            coefficients[manipulatedPoints[i]].maxDistance = 0;

        cloth.coefficients = coefficients;
      }
    }
  }

  public bool IsTargetAttached(int targetId)
  {
    if (manipulatedPoints.Length > targetId && manipulatedPoints[targetId] > -1)
      return true;
    else
      return false;
  }

  public bool IsAttached()
  {
    bool attached = false;
    for (int i = 0; i < manipulatedPoints.Length; i++)
    {
      if (manipulatedPoints[i] > -1)
        attached = true;
    }
    return attached;
  }

  public void DetachTarget(int targetId)
  {
    if (targetPoints.Length > targetId && manipulatedPoints[targetId] > -1)
    {
      ClothSkinningCoefficient[] coefficients = cloth.coefficients;

      coefficients[manipulatedPoints[targetId]].maxDistance = float.MaxValue;
      manipulatedPoints[targetId] = -1;

      //Checking if no attachment remains 
      bool freeze = true;
      for (int i = 0; i < coefficients.Length; i++)
      {
        if (coefficients[i].maxDistance == 0 && i != manipulatedPoints[targetId])
        {
          freeze = false;
          break;
        }
      }

      if (freeze)
      {
        //if no attachment remains then the cloth is freezed

        for (int i = 0; i < coefficients.Length; i++)
          coefficients[i].maxDistance = 0;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < cloth.vertices.Length; i++)
          vertices[i] = cloth.vertices[i];

        mesh.vertices = vertices;
      }
      cloth.coefficients = coefficients;
    }
  }

  public int GetManipulatedPoint(uint id)
  {
    if (id < manipulatedPoints.Length)
      return manipulatedPoints[id];
    else
      return -1;
  }
}
