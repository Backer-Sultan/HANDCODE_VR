using UnityEngine;
using UnityEngine.EventSystems;


[System.Serializable]
[RequireComponent( typeof(ClothSnapping))]
public class ClothUnsnapping : MonoBehaviour
{

  private Cloth cloth;
  private ClothManipulation clothManipulation;
  public float unsnappingCoef = 1.05f;


  public EventTrigger.TriggerEvent clothUnsnapped;

  private Mesh mesh;
  private Vector3[] initialVertices;
  private bool initialConfig = true;
  private bool unsnapped = false;

  private ClothSnapping snapping;

  // Use this for initialization
  void Awake()
  {
    //saving inital mesh shape 
    snapping = GetComponent<ClothSnapping>();
    cloth = snapping.cloth;
    clothManipulation = snapping.clothManipulation;
    if (cloth)
    {
      initialVertices = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
      mesh = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }
  }

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (cloth)
    {
      if (snapping.Snapped &&  !unsnapped)
      {
        ClothSkinningCoefficient[] coefficients = cloth.coefficients;

        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
          int [] ids = new int [3];
          ids[0] = mesh.triangles[i];
          ids[1] = mesh.triangles[i + 1];
          ids[2] = mesh.triangles[i + 2];

          float maxDist = coefficients[ids[0]].maxDistance;
          if (maxDist != coefficients[ids[1]].maxDistance || maxDist != coefficients[ids[2]].maxDistance) //test the streching 
          {
            for(int j=0;j<3;j++)
            {
              int k = (j + 1) % 3;
              Vector3 v0 = cloth.vertices[ids[j]];
              Vector3 v1 = cloth.vertices[ids[k]];
              float dist = Vector3.Distance(v0, v1);

              Vector3 v0Ref = initialVertices[ids[j]];
              Vector3 v1Ref = initialVertices[ids[k]];
              float distRef = Vector3.Distance(v0Ref, v1Ref);

              if(dist >= distRef* unsnappingCoef)
              {
                if (ids[j] != clothManipulation.GetManipulatedPoint(0) && ids[j] != clothManipulation.GetManipulatedPoint(1)) //exception of the manipulated vertices
                {
                  coefficients[ids[j]].maxDistance = float.MaxValue;
                  snapping.snappedVertices[ids[j]] = false;
                }

                if (ids[k] != clothManipulation.GetManipulatedPoint(0) && ids[k] != clothManipulation.GetManipulatedPoint(1)) //exception of the manipulated vertices
                {
                  coefficients[ids[k]].maxDistance = float.MaxValue;
                  snapping.snappedVertices[ids[k]] = false;
                }
              }
            }
          }
        }
        int unsnappedCount = 0;
        bool allUnsnapped = true;
        for (int i = 0; i < mesh.vertices.Length; i++)
          if (snapping.snappedVertices[i])
          {
            allUnsnapped = false;
            unsnappedCount++;
          }


        if (allUnsnapped)
        {
          unsnapped = true;
          clothUnsnapped.Invoke(new BaseEventData(EventSystem.current));
        }

        cloth.coefficients = coefficients;
      }
    }
  }
}
