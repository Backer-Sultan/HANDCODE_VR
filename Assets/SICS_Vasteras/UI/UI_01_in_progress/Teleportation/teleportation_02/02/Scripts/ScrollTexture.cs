using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour
{
	public GameObject[] parts;
	Renderer[] rends;
	public float speed =1f;

	private void Start()
	{
		rends = GetComponentsInChildren<Renderer>();
	}


	public void RunPaper()
	{
		foreach(Renderer r in rends)
		{
			r.material.mainTextureOffset = new Vector2(r.material.mainTextureOffset.y - Time.deltaTime*speed, r.material.mainTextureOffset.x);
		}
	}


	private void FixedUpdate()

	{
		RunPaper();
	} 
}