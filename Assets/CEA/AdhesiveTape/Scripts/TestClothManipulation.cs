using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClothManipulation : MonoBehaviour {

  public ClothManipulation clothManipulation;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
    {
      if(clothManipulation.IsTargetAttach(0))
        clothManipulation.DetachTarget(0);
      else
        clothManipulation.AttachTarget(0);
    }
	}
}
