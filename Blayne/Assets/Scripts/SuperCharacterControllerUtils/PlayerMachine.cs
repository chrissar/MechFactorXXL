﻿using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMachine : SuperStateMachine {

    public Transform AnimatedMesh;

    Animator animator;

    public float WalkSpeed = 4.0f;
    public float RunSpeed = 10.0f;
    public float RotateSpeed = 10.0f;
    [HideInInspector]public float forwardAmount = 0;
    [HideInInspector]public float sidewaysAmount = 0;
    [HideInInspector]public float rotateAmount = 0;
    public float WalkAcceleration = 30.0f;
    public float RunAcceleration = 60.0f;
    public float JumpAcceleration = 5.0f;
    public float JumpHeight = 3.0f;
    public float Gravity = 25.0f;
    public float maxSpeed;

    public bool aim;
    public float aimingWeight;
    float cameraSpeedOffset;

    // Add more states by comma separating them
    enum PlayerStates { Idle, Walk, Jump, Fall, Run, Stopping }

    private SuperCharacterController controller;

    // current velocity
    private Vector3 moveDirection;
    // current direction our character's art is facing
    public Vector3 lookDirection { get; private set; }

    private PlayerInputController input;

    private Quaternion previousRotation;

    private Transform cam; //reference to our case

    void Start () {
        // Put any code here you want to run ONCE, when the object is initialized
        //Setup our camera reference
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }

        input = gameObject.GetComponent<PlayerInputController>();

        // Grab the controller object from our object
        controller = gameObject.GetComponent<SuperCharacterController>();
		
		// Our character's current facing direction, planar to the ground
        lookDirection = transform.forward;

        // Set our currentState to idle on startup
        currentState = PlayerStates.Idle;

        animator = GetComponentInChildren<Animator>();

        SetUpAnimator();
    }

    void SetUpAnimator()
    {
        animator = GetComponent<Animator>();

        foreach (var childAnimator in GetComponentsInChildren<Animator>())
        {
            if (childAnimator != animator)
            {
                animator.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break;
            }
        }
    }

    void UpdateAnimator()
    {
        
        animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", rotateAmount, 0.1f, Time.deltaTime);
        animator.SetBool("Aim", aim);
        animator.SetLayerWeight(1, 1);
    }

    protected override void EarlyGlobalSuperUpdate()
    {
		// Rotate out facing direction horizontally based on mouse input
        lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x * RotateSpeed, controller.up) * lookDirection;
        // Put any code in here you want to run BEFORE the state's update function.
        // This is run regardless of what state you're in
        aim = Input.GetMouseButton(1);

        aimingWeight = Mathf.MoveTowards(aimingWeight, (aim && !input.Current.SprintInput) ? 1.0f : 0.0f, Time.deltaTime * 5);

        Vector3 normalState = new Vector3(0, 0, 0);
        Vector3 aimingState = new Vector3(0, 0, 1.0f);

        Vector3 pos = Vector3.Lerp(normalState, aimingState, aimingWeight);

        cam.transform.localPosition = pos;
    }

    protected override void LateGlobalSuperUpdate()
    {
        // Put any code in here you want to run AFTER the state's update function.
        // This is run regardless of what state you're in

        // Move the player by our velocity every frame
        transform.position += moveDirection * controller.deltaTime;

        previousRotation = AnimatedMesh.rotation;

        // Rotate our mesh to face where we are "looking"
        AnimatedMesh.rotation = Quaternion.LookRotation(lookDirection, controller.up);

        rotateAmount = Input.GetAxis("Mouse X") * 0.1f;

        UpdateAnimator();
    }

    private bool AcquiringGround()
    {
        return controller.currentGround.IsGrounded(false, 0.01f);
    }

    private bool MaintainingGround()
    {
        return controller.currentGround.IsGrounded(true, 0.5f);
    }

    public void RotateGravity(Vector3 up)
    {
        lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
    }

    /// <summary>
    /// Constructs a vector representing our movement local to our lookDirection, which is
    /// controlled by the camera
    /// </summary>
    private Vector3 LocalMovement(bool isRunning)
    {
        Vector3 right = Vector3.Cross(controller.up, lookDirection);

        Vector3 local = Vector3.zero;

        if (input.Current.MoveInput.x != 0)
        {
            local += right * input.Current.MoveInput.x;
            //forwardAmount = input.Current.MoveInput.x;
        }

        if (input.Current.MoveInput.z != 0)
        {
            local += lookDirection * input.Current.MoveInput.z;
            if (isRunning)
                forwardAmount = input.Current.MoveInput.z;
            else
                forwardAmount = input.Current.MoveInput.z / 2;
        }

        return local.normalized;
    }

    // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

	/*void Update () {
	 * Update is normally run once on every frame update. We won't be using it
     * in this case, since the SuperCharacterController component sends a callback Update 
     * called SuperUpdate. SuperUpdate is recieved by the SuperStateMachine, and then fires
     * further callbacks depending on the state
	}*/

    // Below are the three state functions. Each one is called based on the name of the state,
    // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
    // Jump_SuperUpdate()
    void Idle_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();
    }

    void Idle_SuperUpdate()
    {
        // Run every frame we are in the idle state

        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            currentState = PlayerStates.Walk;
            return;
        }

        if (moveDirection.magnitude > 0)
        {
            forwardAmount = moveDirection.magnitude / maxSpeed;
        }
        else
        {
            forwardAmount = 0;
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, 10.0f * controller.deltaTime);
    }

    void Idle_ExitState()
    {
        // Run once when we exit the idle state
    }

    void Run_SuperUpdate()
    {
        if (input.Current.JumpInput)
        {
            forwardAmount = 0;
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            forwardAmount = 0;
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement(true) * RunSpeed, RunAcceleration * controller.deltaTime);
            if (!input.Current.SprintInput)
            {
                currentState = PlayerStates.Walk;
                return;
            }
        }
        else
        {
            // were sliding now, so we'll pretend to still be moving legs
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void Run_ExitState()
    {
        maxSpeed = moveDirection.magnitude;
    }

    void Walk_SuperUpdate()
    {
        if (input.Current.JumpInput)
        {
            forwardAmount = 0;
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            forwardAmount = 0;
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement(false) * WalkSpeed, WalkAcceleration * controller.deltaTime);
            if (input.Current.SprintInput)
            {
                currentState = PlayerStates.Run;
                return;
            }
        }
        else
        {
            forwardAmount = 0;
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void Walk_ExitState()
    {
        maxSpeed = moveDirection.magnitude;
    }

    void Jump_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
    }

    void Jump_SuperUpdate()
    {
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;            
        }

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement(false) * WalkSpeed, JumpAcceleration * controller.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;
    }

    void Fall_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        // moveDirection = trueVelocity;
    }

    void Fall_SuperUpdate()
    {
        if (AcquiringGround())
        {
            moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
            currentState = PlayerStates.Idle;
            return;
        }

        moveDirection -= controller.up * Gravity * controller.deltaTime;
    }
}
