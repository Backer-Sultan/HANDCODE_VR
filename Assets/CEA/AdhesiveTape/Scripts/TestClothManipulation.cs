using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClothManipulation : MonoBehaviour {

  public ClothManipulation clothManipulation;
  public ClothSnapping clothSnapping;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Comma))
    {
      if(clothManipulation.IsTargetAttached(0))
        clothManipulation.DetachTarget(0);
      else
        clothManipulation.AttachTarget(0);
    }
    else if (Input.GetKeyDown(KeyCode.N))
    {
      Debug.Log("Semicolon");
      if (clothManipulation.IsTargetAttached(1))
        clothManipulation.DetachTarget(1);
      else
        clothManipulation.AttachTarget(1);
    }
    else if(Input.GetKeyDown(KeyCode.R))
    {
      clothSnapping.Reinitialize();
    }

  }
}
