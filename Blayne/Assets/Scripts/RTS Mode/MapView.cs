﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapView : MonoBehaviour {
    public Transform playerPos;
    public Image sprite;
    public Image enemy;

    float width = 256;
    float height = 256;

    float ratio = 1.32f;

    Vector2 pos;
    public bool isRTS = false;

    public Canvas mCanvas;
	// Use this for initialization
	void Start () {
	
	}
    // TODO
    // For each Unit, spawn a sprite icon at it's relative position.
    // When left clicking, switch to top down view camera, ray trace,
    // select unit, and then switch back; such that the user does not see.
    // Better if it can be done with camera technically disabled.
    // Then everything slots together perfectly.

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isRTS)
            {
                mCanvas.GetComponent<CanvasGroup>().alpha = 0;
                mCanvas.GetComponent<CanvasGroup>().interactable = false;
                isRTS = false;
            }
            else
            {
                isRTS = true;
                mCanvas.GetComponent<CanvasGroup>().alpha = 1;
                mCanvas.GetComponent<CanvasGroup>().interactable = true;
                if (playerPos != null && sprite != null)
                {
                    sprite.transform.localPosition = new Vector3((playerPos.position.x - width) * ratio,
                        (playerPos.position.z - height) * ratio, 0);
                }
            }

        }

	}
}