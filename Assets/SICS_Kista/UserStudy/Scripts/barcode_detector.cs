using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barcode_detector : MonoBehaviour {

	private RaycastHit rayhit;
	public float rayLength;
	public float scanningTime; // Time in seconds it takes to perform a successfull scan
	private float elapsedScanningTime; // Time in seconds that have elapsed during an ongoing scan
	private Rigidbody scanningTarget;
	private GameObject emitterLine;
	private AudioSource scannerSpeaker;
	private 

	// Use this for initialization
	void Start () {
		elapsedScanningTime = 0.0f;
		emitterLine = GameObject.Find ("emitter_line");
		GameObject scannerHead = GameObject.Find ("scannerhead");
		scannerSpeaker = scannerHead.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 start = this.gameObject.transform.position;
		//Debug.Log ("ray start pos is: " + start.ToString ());
		Vector3 dir = this.gameObject.transform.TransformDirection(Vector3.forward);
		//Debug.Log ("ray dir is: " + dir.ToString ());
		//Debug.DrawRay (start, dir * rayLength, Color.red, 2.5f, false);
		if (Physics.Raycast (start, dir, out rayhit, rayLength)) {
			float hitdistance = rayhit.distance;
			// Debug.Log ("distance to hit is " + hitdistance);
			// Adjust length of laser line
			emitterLine.transform.localScale = new Vector3(1.0f, 1.0f, rayhit.distance * 1.0f);
			if (rayhit.collider.tag == "barcode") {
				// Debug.Log ("targeting barcode: " + rayhit.collider.name);
				if (rayhit.rigidbody == scanningTarget) {
					// Still scanning the same object
					elapsedScanningTime += Time.deltaTime;
				} else {
					// Starting scanning barcode
					scanningTarget = rayhit.rigidbody;
					elapsedScanningTime = 0.0f;
					Debug.Log ("Starting a scan");
				}
			} else {
				if (scanningTarget != null) {
					scanningTarget = null;
					Debug.Log ("End of scan. Duration " + elapsedScanningTime);
					elapsedScanningTime = 0.0f;
				}
			}
		} else {

		}
		if (elapsedScanningTime >= scanningTime) {
			// Performed a complete scan
			Debug.Log("Performed a complete scan of object: " + scanningTarget);
			scannerSpeaker.Play ();
			elapsedScanningTime = 0.0f;
		}
	}
}
