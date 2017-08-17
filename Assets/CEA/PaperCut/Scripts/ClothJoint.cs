using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClothJoint : MonoBehaviour
{

  public Cloth clothParent;
  public Cloth clothChild;


  Mesh meshChild;

  int[] jointPointsIdParent;
  int[] jointPointsIdChild;

  int[] jointCutId;
  List<int>[] jointCutLinkId;

  public Transform knife;
  public float cuttingDistance = 0.02f;

  //The forward axis and the position of the EnableCutPlan transform define the plan above which the points can be cut.
  public Transform EnableCutPlan1, EnableCutPlan2;
  public EventTrigger.TriggerEvent OnJointCut;
  public EventTrigger.TriggerEvent OnHalfJointCut;
  public EventTrigger.TriggerEvent OnJointPointCut;

  private bool jointCut, jointHalfCut;
  private int cutPointsCount;

  public int CutPointsCount { get { return cutPointsCount; } }
  public int JointPointsCount { get { return jointCutId.Length; } }

  // Use this for initialization
  void Start()
  {
    cutPointsCount = 0;
    jointCut = jointHalfCut = false;

    if (clothParent && clothChild)
    {
      ClothSkinningCoefficient[] coefficients;

      //identification of the parent cloth vertex joint + set them free
      coefficients = clothParent.coefficients;

      int count = 0;
      for (int i = 0; i < coefficients.Length; i++)
        if (coefficients[i].maxDistance == 1)
          count++;

      int index = 0;
      jointPointsIdParent = new int[count];
      for (int i = 0; i < coefficients.Length; i++)
        if (coefficients[i].maxDistance == 1)
        {
          //coefficients[i].maxDistance = float.MaxValue;
          jointPointsIdParent[index] = i;
          index++;
        }

      clothParent.coefficients = coefficients;

      //identification of the child cloth vertex joint
      coefficients = clothChild.coefficients;

      count = 0;
      for (int i = 0; i < coefficients.Length; i++)
        if (coefficients[i].maxDistance <= 2)
          count++;

      index = 0;
      jointPointsIdChild = new int[count];
      for (int i = 0; i < coefficients.Length; i++)
      {
        if (coefficients[i].maxDistance <= 2)
        {
          jointPointsIdChild[index] = i;
          index++;
        }
      }

      //Reorganising the child vertices Ids to match with closest parent vertice Id
      int[] adjustedIds = new int[jointPointsIdParent.Length];
      for (int i = 0; i < jointPointsIdParent.Length; i++)
      {
        Vector3 v = clothParent.vertices[jointPointsIdParent[i]];
        v = clothParent.transform.TransformPoint(v); //local parent to world
        v = clothChild.transform.InverseTransformPoint(v); //world to local child
        float min = 0.01f;
        adjustedIds[i] = -1;
        for (int j = 0; j < jointPointsIdChild.Length; j++)
        {
          float dist = Vector3.Distance(v, clothChild.vertices[jointPointsIdChild[j]]);
          if (dist <= min)
          {
            min = dist;
            adjustedIds[i] = jointPointsIdChild[j];
          }
        }
      }

      jointPointsIdChild = adjustedIds;

      //copying  mesh before modifiying it
      meshChild = (Mesh)Instantiate(clothChild.GetComponent<SkinnedMeshRenderer>().sharedMesh);
      clothChild.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshChild;

      count = 0;
      for (int i = 0; i < coefficients.Length; i++)
        if (coefficients[i].maxDistance == 1)
          count++;

      //identification of the vertex joints that can be cut
      jointCutId = new int[count];


      int indexCut = 0;
      int indexLink = 0;
      for (int i = 0; i < jointPointsIdChild.Length; i++)
      {
        if (jointPointsIdChild[i] != -1)
        {
          if (coefficients[jointPointsIdChild[i]].maxDistance == 1)
          {
            jointCutId[indexCut] = jointPointsIdChild[i];
            indexCut++;
          }
        }
      }

      jointCutLinkId = new List<int>[count];

      //identification of the vertex joints that will be cut with their parent vertex
      for (int i = 0; i < jointPointsIdChild.Length; i++)
      {
        if (jointPointsIdChild[i] != -1)
        {
          if (coefficients[jointPointsIdChild[i]].maxDistance == 2)
          {
            //shearching its vertex joint corresponding
            float min = float.MaxValue;
            int minId = -1;
            for (int j = 0; j < jointCutId.Length; j++)
            {
              if (coefficients[jointCutId[j]].maxDistance == 1)
              {
                float dist = Vector3.Distance(meshChild.vertices[jointPointsIdChild[i]], meshChild.vertices[jointCutId[j]]);
                if (dist < min)
                {
                  min = dist;
                  minId = j;
                }
              }
            }
            if (minId != -1)
            {
              if (jointCutLinkId[minId] == null)
                jointCutLinkId[minId] = new List<int>();
              jointCutLinkId[minId].Add(jointPointsIdChild[i]);
            }
            indexLink++;
          }
        }
      }

      for (int i = indexCut; i < jointCutId.Length; i++)
        jointCutId[i] = -1;

      //setting free the child vertex joints
      for (int i = 0; i < jointPointsIdChild.Length; i++)
        if (jointPointsIdChild[i] != -1)
          coefficients[jointPointsIdChild[i]].maxDistance = 0;

      clothChild.coefficients = coefficients;

    }
  }

  // Update is called once per frame
  void Update()
  {
    JointUpdate();
    KnifeCut();
    JointCutEvent();
    JointPointCutEvent();
  }

  private void JointUpdate()
  {
    if (clothParent && clothChild)
    {
      //Joint update
      Vector3[] vertices = meshChild.vertices;
      for (int i = 0; i < jointPointsIdParent.Length; i++)
      {
        if (jointPointsIdChild[i] != -1)
        {
          Vector3 v = clothParent.vertices[jointPointsIdParent[i]];
          v = clothParent.transform.TransformPoint(v); //local parent to world
          v = clothChild.transform.InverseTransformPoint(v); //world to local child
          vertices[jointPointsIdChild[i]] = v;
        }
      }
      meshChild.vertices = vertices;
    }
  }

  private void KnifeCut()
  {
    if (clothParent && clothChild)
    {
      //knife to local frame of reference of cloth
      Vector3 localKnife = clothChild.transform.InverseTransformPoint(knife.position);

      //Enable Cut Plan in cloth frame of reference
      Vector3 localPosEnableCut1 = new Vector3(0, 0, 0);
      Vector3 localDirEnableCut1 = new Vector3(0, 0, 0);
      if (EnableCutPlan1 != null)
      {
        localPosEnableCut1 = clothChild.transform.InverseTransformPoint(EnableCutPlan1.position);
        localDirEnableCut1 = clothChild.transform.InverseTransformDirection(EnableCutPlan1.forward);
      }

      Vector3 localPosEnableCut2 = new Vector3(0, 0, 0);
      Vector3 localDirEnableCut2 = new Vector3(0, 0, 0);
      if (EnableCutPlan2 != null)
      {
        localPosEnableCut2 = clothChild.transform.InverseTransformPoint(EnableCutPlan2.position);
        localDirEnableCut2 = clothChild.transform.InverseTransformDirection(EnableCutPlan2.forward);
      }

      ClothSkinningCoefficient[] coefficients = clothChild.coefficients;

      for (int i = 0; i < jointCutId.Length; i++)
      {
        if (jointCutId[i] != -1)
        {
          bool cutEnable = true;
          if (localPosEnableCut1 != Vector3.zero)
          {
            float d = Vector3.Dot(localDirEnableCut1, clothChild.vertices[jointCutId[i]]) - Vector3.Dot(localDirEnableCut1, localPosEnableCut1);
            if (d < 0)
              cutEnable = false;
          }

          if (localPosEnableCut2 != Vector3.zero)
          {
            float d = Vector3.Dot(localDirEnableCut2, clothChild.vertices[jointCutId[i]]) - Vector3.Dot(localDirEnableCut2, localPosEnableCut2);
            if (d < 0)
              cutEnable = false;
          }

          if (cutEnable)
          {
            float dist = Vector3.Distance(clothChild.vertices[jointCutId[i]], localKnife);
            if (dist < cuttingDistance)
            {
              coefficients[jointCutId[i]].maxDistance = float.MaxValue;

              if (jointCutLinkId[i] != null)
                for (int j = 0; j < jointCutLinkId[i].Count; j++)
                  coefficients[jointCutLinkId[i][j]].maxDistance = float.MaxValue;
            }
          }
        }
      }
      clothChild.coefficients = coefficients;
    }
  }

  public void RebuildJoint()
  {
    ClothSkinningCoefficient[] coefficients = clothChild.coefficients;
    //setting free the child vertex joints
    for (int i = 0; i < jointPointsIdChild.Length; i++)
      if (jointPointsIdChild[i] != -1)
        coefficients[jointPointsIdChild[i]].maxDistance = 0;

    clothChild.coefficients = coefficients;

    jointCut = jointHalfCut = false;
    cutPointsCount = 0;
  }

  private void JointCutEvent()
  {
    if (!jointCut)
    {
      int i = 0;
      bool cut = true;
      ClothSkinningCoefficient[] coefficients = clothChild.coefficients;
      do
      {
        if (coefficients[jointCutId[i]].maxDistance != float.MaxValue)
          cut = false;

        i++;
      } while (cut && i < jointCutId.Length);

      if (cut)
      {
        jointCut = true;
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        OnJointCut.Invoke(eventData);
      }
    }
  }

  private void JointPointCutEvent()
  {
    if (!jointCut)
    {
      ClothSkinningCoefficient[] coefficients = clothChild.coefficients;
      int cutPointsCountCur = 0;
      for (int i = 0; i < jointCutId.Length; i++)
      {
        if (coefficients[jointCutId[i]].maxDistance == float.MaxValue)
        {
          cutPointsCountCur++;
        }
      }

      if (!jointHalfCut)
      {
        if (cutPointsCountCur >= jointCutId.Length / 2)
        {
          jointHalfCut = true;
          BaseEventData eventData = new BaseEventData(EventSystem.current);
          OnHalfJointCut.Invoke(eventData);
        }
      }

      if(cutPointsCount< cutPointsCountCur)
      {
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        cutPointsCount = cutPointsCountCur;
        OnJointPointCut.Invoke(eventData);
      }
    }
  }
}
