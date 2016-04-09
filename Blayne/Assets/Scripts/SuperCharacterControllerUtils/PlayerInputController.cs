using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    public PlayerInput Current;
    public Vector2 RightStickMultiplier = new Vector2(3, -1.5f);

	// Use this for initialization
	void Start () {
        Current = new PlayerInput();
	}

	// Update is called once per frame
	void Update () {
        
        // Retrieve our current WASD or Arrow Key input
        // Using GetAxisRaw removes any kind of gravity or filtering being applied to the input
        // Ensuring that we are getting either -1, 0 or 1
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        bool mouseAim = Input.GetMouseButton(1) || Input.GetMouseButtonDown(1);
        bool mouseFire = Input.GetMouseButton(0) || Input.GetMouseButtonDown(0);

        //Vector2 rightStickInput = new Vector2(Input.GetAxisRaw("RightH"), Input.GetAxisRaw("RightV"));
        Vector2 rightStickInput = Vector2.zero;

        // pass rightStick values in place of mouse when non-zero
        mouseInput.x = rightStickInput.x != 0 ? rightStickInput.x * RightStickMultiplier.x : mouseInput.x;
        mouseInput.y = rightStickInput.y != 0 ? rightStickInput.y * RightStickMultiplier.y : mouseInput.y;

        bool jumpInput = Input.GetButtonDown("Jump");

        bool sprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftShift);

        Current = new PlayerInput()
        {
            MoveInput = moveInput,
            MouseInput = mouseInput,
            JumpInput = jumpInput,
            SprintInput = sprintInput,
            MouseAim = mouseAim,
            MouseFire = mouseFire
        };
	}
}

public struct PlayerInput
{
    public Vector3 MoveInput;
    public Vector2 MouseInput;
    public bool JumpInput;
    public bool SprintInput;
    public bool MouseAim;
    public bool MouseFire;
}
