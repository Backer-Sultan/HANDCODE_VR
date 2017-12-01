using UnityEngine;
using UnityEngine.EventSystems;


[System.Serializable]
public class ClothSnapping : MonoBehaviour
{

  public Cloth cloth;
  public Transform spawnPoint;
  public ClothManipulation clothManipulation;
  public float snappingDistance = 0.01f;

  public EventTrigger.TriggerEvent clothSnapped;

  private Mesh mesh;
  private Vector3[] initialVertices;
  private int snappingSide = 0;
  private bool initialConfig = true;
  private bool snapped = false;

  [HideInInspector]
  public bool[] snappedVertices;

  // Use this for initialization
  void Awake()
  {
    if (cloth)
    {
      //saving inital mesh shape 
      initialVertices = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
      mesh = cloth.GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }
  }

  void Start()
  {
    Reinitialize();
  }

  // Update is called once per frame
  void Update()
  {
    if (cloth)
    {
      if (initialConfig)
      {
        Reinitialize();
        if(clothManipulation.IsAttached())
          initialConfig = false;
      }
      else if (!snapped)
      {
        Vector3[] vertices = mesh.vertices;
        ClothSkinningCoefficient[] coefficients = cloth.coefficients;

        for (int i = 0; i < cloth.vertices.Length; i++)
        {
          int manipulatedVertex = -1;
          if (i == clothManipulation.GetManipulatedPoint(0))
            manipulatedVertex = 0;

          if (i == clothManipulation.GetManipulatedPoint(1))
            manipulatedVertex = 1;

          if (coefficients[i].maxDistance > 0 || manipulatedVertex != -1)
          {
            if (snappingSide == 0)//Determining the side (the tape has a symmetry) 
            {
              float distance = Vector3.Distance(cloth.vertices[i], initialVertices[i]);
              Vector3 symmetry = cloth.vertices[i];
              symmetry.Scale(new Vector3(1, 1, -1));
              float distanceSymmetry = Vector3.Distance(symmetry, initialVertices[i]);
              if (distance <= snappingDistance)
                snappingSide = 1;
              else if (distanceSymmetry <= snappingDistance)
                snappingSide = -1;
            }
            else
            {
              Vector3 vertex = cloth.vertices[i];
              vertex.Scale(new Vector3(1, 1, snappingSide));
              float distance = Vector3.Distance(vertex, initialVertices[i]);

              float coef = 1;
              if (manipulatedVertex != -1)
                coef = 1.5f;

              if (distance <= coef * snappingDistance)
              {
                if (manipulatedVertex != -1)
                  clothManipulation.DetachTarget(manipulatedVertex);

                coefficients[i].maxDistance = -1;
                vertices[i] = initialVertices[i];
                vertices[i].Scale(new Vector3(1, 1, snappingSide));
                snappedVertices[i] = true;
              }
            }
          }
        }

        int snappedCount = 0;
        bool allsnapped = true;
        for (int i = 0; i < vertices.Length; i++)
          if (!snappedVertices[i])
          {
            allsnapped = false;
            snappedCount++;
          }



        if (allsnapped)
        {
          Debug.Log("allsnapped");
          snapped = true;
          clothSnapped.Invoke(new BaseEventData(EventSystem.current));
        }
        cloth.coefficients = coefficients;
        mesh.vertices = vertices;
      }
    }
  }

  public void Reinitialize()
  {
    snappingSide = 0;

    if (cloth && spawnPoint)
    {
      for (int i = 0; i < cloth.coefficients.Length; i++)
        cloth.coefficients[i].maxDistance = 0;

        mesh.vertices = initialVertices;
      Vector3[] vertices = mesh.vertices;
      Vector3 translation = spawnPoint.position - cloth.transform.position;
      translation = cloth.transform.InverseTransformDirection(translation);
      Quaternion rotation = Quaternion.Inverse(cloth.transform.rotation) * spawnPoint.rotation;
      for (int i = 0; i < vertices.Length; i++)
      {
        vertices[i] = rotation * vertices[i];
        vertices[i] +=  translation;
      }
      mesh.vertices = vertices;
      initialConfig = true;
      snapped = false;

      snappedVertices = new bool[vertices.Length];
      for (int i = 0; i < vertices.Length; i++)
        snappedVertices[i] = false;
    }
  }
}
