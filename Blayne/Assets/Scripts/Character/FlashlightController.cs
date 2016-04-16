using UnityEngine;
using System.Collections;

public class FlashlightController : MonoBehaviour {

	public bool flashlight = false;
	public GameObject spotlight;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			flashlight = !flashlight;
		}
		if (flashlight) {
			spotlight.SetActive (true);
		} else {
			spotlight.SetActive (false);
		}
	}
}
