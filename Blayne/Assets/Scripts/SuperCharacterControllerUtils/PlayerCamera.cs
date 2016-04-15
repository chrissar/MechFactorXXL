using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public float Distance = 3.0f;
    public float Height = 2.0f;
    public float horizontalOffset = 2.0f;
    public float movementSpeed = 1.0f;
    public float rotationSpeed = 1.0f;
    public float topDownHeight = 10.0f;
    public float crouchHeight = 1.0f;
    public float panningSpeed = 2.0f;
    public float zoomingSpeed = 1.0f;
    public float minZoom = 0.3f;
    public float maxZoom = 3;
    public GameObject PlayerTarget;
	public Camera mainCamera;

    private PlayerInputController input;
    private PlayerMachine machine;
    private float yRotation;
    private float mZoom = 1.0f;
    private Vector3 mPannedOffset;
    private SuperCharacterController controller;
    private Vector3 lastMousePos;
    private Vector3 currentMousePos;

	// Use this for initialization
	void Start ()
    {
        input = PlayerTarget.GetComponent<PlayerInputController>();
        machine = PlayerTarget.GetComponent<PlayerMachine>();
        controller = PlayerTarget.GetComponent<SuperCharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pos;
        Quaternion rot;
        lastMousePos = currentMousePos;
        currentMousePos = Input.mousePosition;   
        if (!GameController.Instance.topDownView)
        {
			machine.cam.enabled = true;
            mZoom = 1.0f;            
            mPannedOffset = Vector3.zero;

            Transform target = PlayerTarget.transform;
            transform.position = target.position;

            yRotation += input.Current.MouseInput.y;

            Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);
            rot = Quaternion.LookRotation(machine.lookDirection, controller.up);
            rot = Quaternion.AngleAxis(yRotation, left) * rot;
            transform.rotation = Quaternion.LookRotation(machine.lookDirection, controller.up);
            transform.rotation = Quaternion.AngleAxis(yRotation, left) * transform.rotation;
            transform.position -= transform.forward * Distance;

            if (machine.crouch)
                transform.position += controller.up * crouchHeight;
            else
                transform.position += controller.up * Height;

            transform.position += rot * (new Vector3(horizontalOffset, 0, 0));
        }
        else
        {
			// Disable the main camera while in top down view mode.
			machine.cam.enabled = false;
        }
    }
}
