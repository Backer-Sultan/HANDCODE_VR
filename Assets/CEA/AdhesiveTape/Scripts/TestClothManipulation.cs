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
		if(Input.GetKeyDown(KeyCode.Space))
    {
      if(clothManipulation.IsTargetAttached(0))
        clothManipulation.DetachTarget(0);
      else
        clothManipulation.AttachTarget(0);
    }
    else if(Input.GetKeyDown(KeyCode.R))
    {
      clothSnapping.Reinitialize();
    }

  }
}
