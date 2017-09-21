using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClothAnimation : MonoBehaviour
{

  public Cloth cloth;
  public Transform[] keypoints = new Transform[4];
  public Transform constraintPoint;

  public float animationSpeed = 1; //animation speed in meter/sec

  private Mesh mesh;
  private Vector3[] initialVertices;
  private int[] animatedPointsId;
  private int[] constrainedPointsId;
  private int[] fixedPointsId;
  private int freeAfterReinit = -1;
  private float t = 0;
  private CubicBezierCurve curve;
  private int playAnimation = 0;
  private int animationPlayed = 0;

  public float T { get { return t; } }

  // Use this for initialization
  void Awake()
  {
    if (keypoints[0] != null && keypoints[1] != null)
    {
      curve = new CubicBezierCurve();

      curve.SetControlPoints(keypoints[0].position,
                            keypoints[1].position,
                            keypoints[2].position,
                            keypoints[3].position);
    }

    if (cloth)
    {
      ClothSkinningCoefficient[] coefficients;

      //saving inital mesh shape 
      initialVertices = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;

      //copying mesh before modifiying it 
      //initialMesh = (Mesh)Instantiate(cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh);

      mesh = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh;
      coefficients = cloth.coefficients;

      //identification of the cloth animated vertices
      int countAnim = 0, countFixed = 0, countConstrained = 0;
      for (int i = 0; i < coefficients.Length; i++)
      {
        if (coefficients[i].maxDistance == 3)
          countConstrained++;
        else if (coefficients[i].maxDistance == 2)
          countAnim++;
        else if (coefficients[i].maxDistance == 0)
          countFixed++;
      }

      int indexAnim = 0, indexFixed = 0, indexConstrained = 0;
      animatedPointsId = new int[countAnim];
      fixedPointsId = new int[countFixed];
      constrainedPointsId = new int[countConstrained];

      for (int i = 0; i < coefficients.Length; i++)
      {
        if (coefficients[i].maxDistance == 3)
        {
          constrainedPointsId[indexConstrained] = i;
          coefficients[i].maxDistance = float.MaxValue;
          indexConstrained++;
        }
        else if (coefficients[i].maxDistance == 2)
        {
          animatedPointsId[indexAnim] = i;
          coefficients[i].maxDistance = float.MaxValue;
          indexAnim++;
        }
        else if (coefficients[i].maxDistance == 0)
        {
          fixedPointsId[indexFixed] = i;
          indexFixed++;
        }
      }
    }
  }

  void Start()
  {
    //identification of the parent cloth vertex jointset them free
    ClothSkinningCoefficient[] coefficients;
    coefficients = cloth.coefficients;

    for (int i = 0; i < animatedPointsId.Length; i++)
      coefficients[animatedPointsId[i]].maxDistance = float.MaxValue;

    cloth.coefficients = coefficients;
  }

  // Update is called once per frame
  void Update()
  {
    if (cloth)
    {
      if (playAnimation != 0)
      {
        Vector3[] vertices = mesh.vertices;

        if (playAnimation == 1 || playAnimation == -1) //placing the paper at animation starting point
        {
          Vector3 target = curve.GetCurvePoint(t);
          for (int i = 0; i < animatedPointsId.Length; i++)
          {
            Vector3 vertex = cloth.transform.TransformPoint(cloth.vertices[animatedPointsId[i]]);
            Vector3 vertexTarget = new Vector3(target.x, target.y, vertex.z);
            float distance = Vector3.Distance(vertex, vertexTarget);
            float k = (2*animationSpeed / distance) * Time.deltaTime;
            if (k >= 1)
            {
              k = 1;
              if (playAnimation > 0)
                playAnimation++;
              else
                playAnimation--;
            }
            vertices[animatedPointsId[i]] = cloth.transform.InverseTransformPoint(Vector3.Lerp(vertex, vertexTarget, k));
          }
          mesh.vertices = vertices;

        }
        else //Playing the animation
        {
          if (playAnimation > 0) //Play animation forward
          {
            Vector3 speed = curve.GetCurveDerivative(t);
            t += animationSpeed * Time.deltaTime / speed.magnitude;

            if (t > 1)
            {
              playAnimation = 0;
              t = 1;
            }
          }
          else //Play animation backward
          {
            Vector3 speed = curve.GetCurveDerivative(t);
            t -= animationSpeed * Time.deltaTime / speed.magnitude;

            if (t < 0)
            {
              playAnimation = 0;
              t = 0;
            }
          }

          //Free the cloth
          if (playAnimation == 0)
          {
            ClothSkinningCoefficient[] coefficients = cloth.coefficients;
            for (int i = 0; i < animatedPointsId.Length; i++)
              coefficients[animatedPointsId[i]].maxDistance = float.MaxValue;

            if (constraintPoint != null)
            {
              for (int i = 0; i < constrainedPointsId.Length; i++)
                coefficients[constrainedPointsId[i]].maxDistance = 0;
              animationPlayed = 1;

              for (int i = 0; i < constrainedPointsId.Length; i++)
              {
                Vector3 v = cloth.transform.TransformPoint(cloth.vertices[constrainedPointsId[i]]);
                vertices[constrainedPointsId[i]] = cloth.transform.InverseTransformPoint(new Vector3(v.x, v.y, v.z));
              }
              mesh.vertices = vertices;
            }

            cloth.coefficients = coefficients;
          }
          else
          {
            Vector3 point = curve.GetCurvePoint(t);

            for (int i = 0; i < animatedPointsId.Length; i++)
            {
              Vector3 v = cloth.transform.TransformPoint(vertices[animatedPointsId[i]]);
              Vector3 p = cloth.transform.InverseTransformPoint(new Vector3(point.x, point.y, v.z));
              vertices[animatedPointsId[i]] = p;
            }
            mesh.vertices = vertices;
          }
        }
      }
      else
      {
        if (constraintPoint != null)
        {
          if (animationPlayed ==1 )
          {
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < constrainedPointsId.Length; i++)
            {
              Vector3 vertex = cloth.transform.TransformPoint(cloth.vertices[constrainedPointsId[i]]);
              Vector3 vertexTarget = new Vector3(constraintPoint.position.x, constraintPoint.position.y, vertex.z);
              float distance = Vector3.Distance(vertex, vertexTarget);
              float k = (animationSpeed / distance) * Time.deltaTime;
              if (k >= 1)
                animationPlayed++;
              else
                vertices[constrainedPointsId[i]] = cloth.transform.InverseTransformPoint(Vector3.Lerp(vertex, vertexTarget, k));
            }
            mesh.vertices = vertices;
          }
          else if(animationPlayed>=1)
          {
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < constrainedPointsId.Length; i++)
            {
              Vector3 v = cloth.transform.TransformPoint(vertices[constrainedPointsId[i]]);
              vertices[constrainedPointsId[i]] = cloth.transform.InverseTransformPoint(new Vector3(constraintPoint.position.x, constraintPoint.position.y, v.z));
            }
            mesh.vertices = vertices;
          }
        }
      }
      
      
      if (freeAfterReinit == 0)
      {
        ClothSkinningCoefficient[] coefficients = cloth.coefficients;

        for (int i = 0; i < coefficients.Length; i++)
          coefficients[i].maxDistance = float.MaxValue;

        for (int i = 0; i < fixedPointsId.Length; i++)
          coefficients[fixedPointsId[i]].maxDistance = 0;

        cloth.coefficients = coefficients;

        freeAfterReinit = -1;
        animationPlayed = 0;
      }
      else if (freeAfterReinit > 0)
        freeAfterReinit--;
    }
  }

  public void OnApplicationQuit()
  {
    mesh.vertices = initialVertices;
  }

  public void PlayAnimation()
  {
    playAnimation = 1;
    t = 0;

    ClothSkinningCoefficient[] coefficients = cloth.coefficients;
    for (int i = 0; i < animatedPointsId.Length; i++)
      coefficients[animatedPointsId[i]].maxDistance = 0;

    cloth.coefficients = coefficients;
  }

  public void RewindAnimation()
  {
    animationPlayed = 0;
    playAnimation = -1;
    t = 1;

    ClothSkinningCoefficient[] coefficients = cloth.coefficients;
    for (int i = 0; i < animatedPointsId.Length; i++)
      coefficients[animatedPointsId[i]].maxDistance = 0;

    for (int i = 0; i < constrainedPointsId.Length; i++)
      coefficients[constrainedPointsId[i]].maxDistance = float.MaxValue;

    cloth.coefficients = coefficients;
  }

  public void Reinitialize()
  {
    Vector3[] vertices = mesh.vertices;

    for (int i = 0; i < vertices.Length; i++)
      vertices[i] = initialVertices[i];

    mesh.vertices = vertices;

    ClothSkinningCoefficient[] coefficients = cloth.coefficients;
    for (int i = 0; i < coefficients.Length; i++)
      coefficients[i].maxDistance = 0;

    cloth.coefficients = coefficients;
    freeAfterReinit = (int)(1 / Time.deltaTime);
    playAnimation = 0;
    t = 0;
  }
}
