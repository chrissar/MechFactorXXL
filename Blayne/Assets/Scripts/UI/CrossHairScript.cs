using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrossHairScript : MonoBehaviour
{
    private RawImage crossHair;
	// Use this for initialization
	void Start ()
    {
        crossHair = gameObject.GetComponent<RawImage>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        crossHair.enabled = !GameController.Instance.topDownView && Input.GetMouseButton(1);
	}
}
