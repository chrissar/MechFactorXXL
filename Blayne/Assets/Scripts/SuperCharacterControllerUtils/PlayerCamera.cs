﻿using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public float Distance = 3.0f;
    public float Height = 2.0f;
    public float horizontalOffset = 2.0f;
    public float movementSpeed = 1.0f;
    public float rotationSpeed = 1.0f;
    public float topDownHeight = 10.0f;

    public GameObject PlayerTarget;    

    private PlayerInputController input;
    private Transform target;
    private PlayerMachine machine;
    private float yRotation;

    private SuperCharacterController controller;

	// Use this for initialization
	void Start ()
    {
        input = PlayerTarget.GetComponent<PlayerInputController>();
        machine = PlayerTarget.GetComponent<PlayerMachine>();
        controller = PlayerTarget.GetComponent<SuperCharacterController>();
        target = PlayerTarget.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pos;
        Quaternion rot;
        if (!GameController.Instance.topDownView)
        {
            

            yRotation += input.Current.MouseInput.y;

            Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);

            rot = Quaternion.LookRotation(machine.lookDirection, controller.up);
            rot = Quaternion.AngleAxis(yRotation, left) * rot;

            pos = target.position;
            pos -= target.forward * Distance;
            pos += target.up * Height;
            pos += rot * (new Vector3(horizontalOffset, 0, 0));
        }

        else
        {
            pos = PlayerTarget.transform.position + Vector3.up * topDownHeight;
            rot = Quaternion.LookRotation(-Vector3.up, Vector3.forward);
        }
        gameObject.transform.position = Vector3.Lerp(pos, gameObject.transform.position, Time.deltaTime * movementSpeed);
        gameObject.transform.rotation = Quaternion.Slerp(rot, gameObject.transform.rotation, Time.deltaTime * rotationSpeed);
    }
}
