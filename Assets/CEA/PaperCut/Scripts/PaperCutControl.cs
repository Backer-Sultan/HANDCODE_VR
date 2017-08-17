using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperCutControl : MonoBehaviour {

  public Transform cutPlan1;
  public Transform cutPlan2;
  public ClothJoint clothJoint;

  float start;
  bool rebuild = false;
  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (clothJoint && rebuild)
    {
      if (Time.time - start > 1.0f)
      {
        clothJoint.RebuildJoint();
        rebuild = false;
        cutPlan2.localPosition = new Vector3(0, 0, 0.12f);
      }
    }
  }

  public void RebuildJointWithDelay()
  {
    start = Time.time;
    rebuild = true;
  }

  public void FlipCutSide()
  {
    Quaternion flip = Quaternion.AngleAxis(180, cutPlan1.right);
    cutPlan1.rotation = flip * cutPlan1.rotation;
  }

  public void PuchCutPlan()
  {
    cutPlan2.position -= new Vector3(0, 0, 0.06f);
  }
}
