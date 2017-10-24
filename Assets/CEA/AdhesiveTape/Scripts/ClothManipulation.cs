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

    manipulatedPoints = new int[targetPoints.Length];
    for (int i = 0; i < manipulatedPoints.Length; i++)
      manipulatedPoints[i] = -1;

    cloth.coefficients = coefficients;

    InitPointers();
  }

  GameObject leftPointer = null;
  GameObject rightPointer = null;
  void InitPointers()
  {
    leftPointer = GameObject.Find("TapeEnd1");
    if (leftPointer == null) leftPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    leftPointer.name = "LeftPointer";
    GameObject.DestroyImmediate(leftPointer.GetComponent<Collider>());
    leftPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

    rightPointer = GameObject.Find("TapeEnd2");
    if (rightPointer == null) rightPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    rightPointer.name = "RightPointer";
    GameObject.DestroyImmediate(rightPointer.GetComponent<Collider>());
    rightPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
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
      if (side == VirtualGrasp.VG_HandSide.LEFT) leftPointer.transform.position = transform.position + transform.rotation * cloth.vertices[2];
      else if (side == VirtualGrasp.VG_HandSide.RIGHT) rightPointer.transform.position = transform.position + transform.rotation * cloth.vertices[90];
    }
  }

  // Update is called once per frame

  void Update()
  {
    if (cloth)
    {
      Vector3[] vertices = mesh.vertices;
      for (int i = 0; i < manipulatedPoints.Length; i++)
      {
        if (manipulatedPoints[i] != -1)
        {
          vertices[manipulatedPoints[i]] = cloth.transform.InverseTransformPoint(targetPoints[i].position);

        }
      }
      mesh.vertices = vertices;

      //Not good
      /*if (leftPointer != null)
        leftPointer.transform.position = transform.position + transform.rotation * cloth.vertices[2];
      if (rightPointer != null)
        rightPointer.transform.position = transform.position + transform.rotation * cloth.vertices[90];*/
    }
  }

  public void OnApplicationQuit()
  {
    mesh.vertices = initialVertices;
  }

  public void AttachTarget(int targetId)
  {
    if (leftPointer != null)
      leftPointer.transform.position = transform.position + transform.rotation * cloth.vertices[2];
    if (rightPointer != null)
      rightPointer.transform.position = transform.position + transform.rotation * cloth.vertices[90];

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

        for (int i = 0; i < manipulatedPoints.Length; i++)
          if (manipulatedPoints[i] > -1)
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

  public int GetManipulatedPoint(int id)
  {
    return manipulatedPoints[id];
  }
}
