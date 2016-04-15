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
            mZoom = 1.0f;
            mPannedOffset = Vector3.zero;
            Transform target = PlayerTarget.transform;

            yRotation += input.Current.MouseInput.y;

            Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);

            rot = Quaternion.LookRotation(machine.lookDirection, controller.up);
            rot = Quaternion.AngleAxis(yRotation, left) * rot;
            pos = target.position;

            pos -= rot * target.forward * Distance;
            pos += rot * target.up * Height;
            pos += rot * (new Vector3(horizontalOffset, 0, 0));

            // When I no longer rotate the mesh the above code, and perhaps the two lines at the bottom
            // causes the camera rotation to break.
            
            transform.position = target.position;

            yRotation += input.Current.MouseInput.y;

            left = Vector3.Cross(machine.lookDirection, controller.up);

            transform.rotation = Quaternion.LookRotation(machine.lookDirection, controller.up);
            transform.rotation = Quaternion.AngleAxis(yRotation, left) * transform.rotation;

            transform.position -= transform.forward * Distance;

            if (machine.crouch)
                transform.position += controller.up * crouchHeight;
            else
                transform.position += controller.up * Height;

            transform.position += rot * (new Vector3(horizontalOffset, 0, 0));


            //pos = target.position;
            //pos -= rot * target.forward * Distance;
            //pos += rot * target.up * Height;
            //pos += rot * (new Vector3(horizontalOffset, 0, 0));


        }
        else
        {
            mZoom -= Input.mouseScrollDelta.y * 0.1f;
            mZoom = Mathf.Clamp((mZoom) * zoomingSpeed, minZoom, maxZoom);
            if (Input.GetMouseButton(2))
            {
                Debug.Log("PANNING!!!!!");
                Vector3 deltaMouse = currentMousePos - lastMousePos;
                mPannedOffset += new Vector3(deltaMouse.x, 0, deltaMouse.y) * panningSpeed * mZoom;
            }
            transform.position = PlayerTarget.transform.position + Vector3.up * topDownHeight * mZoom - mPannedOffset;
            transform.rotation = Quaternion.LookRotation(-Vector3.up, Vector3.forward);
        }
        //gameObject.transform.position = Vector3.Lerp(pos, gameObject.transform.position, Time.deltaTime * movementSpeed);
        //gameObject.transform.rotation = Quaternion.Slerp(rot, gameObject.transform.rotation, Time.deltaTime * rotationSpeed);
    }
}
