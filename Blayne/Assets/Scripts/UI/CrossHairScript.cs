using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrossHairScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;
	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!GameController.Instance.topDownView && Input.GetMouseButton(1))
            canvasGroup.alpha = 1;
        else
            canvasGroup.alpha = 0;
    }
}
