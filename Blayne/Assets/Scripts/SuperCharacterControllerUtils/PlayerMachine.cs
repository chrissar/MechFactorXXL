using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMachine : SuperStateMachine {

    public Transform AnimatedMesh;
    public Combat.Gun gun;
    public GameObject HandPosition;
    public Transform RightHandPosition;
    Animator animator;

    public float WalkSpeed = 4.0f;
    public float crouchSpeed = 2.0f;
    public float RunSpeed = 10.0f;
    public float RotateSpeed = 10.0f;
    public float movementSpeed = 0;
    [HideInInspector]public float forwardAmount = 0;
    [HideInInspector]public float sidewaysAmount = 0;
    [HideInInspector]public float rotateAmount = 0;
    public float WalkAcceleration = 30.0f;
    public float RunAcceleration = 60.0f;
    public float JumpAcceleration = 5.0f;
    public float JumpHeight = 3.0f;
    public float Gravity = 25.0f;
    public float maxSpeed;
    float IKweight;

    public bool aim;
    public float aimingWeight;
    public bool firing = false;
    public bool crouch = false;
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

    public Transform cam; //reference to our case

    // IK Stuff
    public Transform spine;
    public float aimingZ = 200;
    public float aimingY = 200;
    public float aimingX = 200;
    public float point;

    public Vector3 rotation;

    public bool debug = false;

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

        if (!gun) throw new UnityException("Player Machine does not have a Gun Script linked");

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
        if (firing)
        {
            firing = false;
            animator.SetTrigger("Fire 0");
        }

        if (!MaintainingGround())
        {         
            animator.SetBool("Grounded", false);
        }
            

        if (AcquiringGround())
            animator.SetBool("Grounded", true);

        animator.SetBool("Crouch", crouch);
        animator.SetFloat("Sideways", sidewaysAmount, 0.1f, Time.deltaTime);
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
        if (!debug)
            aim = input.Current.MouseAim;

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

        //previousRotation = AnimatedMesh.rotation;
        previousRotation = gameObject.transform.rotation;

        // Rotate our mesh to face where we are "looking"
        //AnimatedMesh.rotation = Quaternion.LookRotation(lookDirection, controller.up);
        gameObject.transform.rotation = Quaternion.LookRotation(lookDirection, controller.up);

        rotateAmount = Input.GetAxis("Mouse X") * 0.1f;

        UpdateAnimator();
    }

    void CorrectIK()
    {

    }

    void Update()
    {
        IKweight = Mathf.MoveTowards(IKweight, (aim) ? 1.0f : 0.0f, Time.deltaTime * 5);
    }

    void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKweight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IKweight);

        Vector3 pos = HandPosition.transform.TransformPoint(Vector3.zero);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, HandPosition.transform.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, HandPosition.transform.rotation);

        // right hand
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IKweight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, IKweight);

        Vector3 rhp = HandPosition.transform.TransformPoint(Vector3.zero);

        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandPosition.transform.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandPosition.transform.rotation);
    }

    void LateUpdate()
    {
        if (!debug)
            crouch = input.Current.Crouch;

        if (aim)
        {
            Vector3 eulerAngleOffset = Vector3.zero;
            eulerAngleOffset = new Vector3(aimingX, aimingY, aimingZ);
            Ray ray = new Ray(cam.position, cam.forward);
            Vector3 lookPosition = ray.GetPoint(point);
            spine.LookAt(lookPosition);            
            spine.Rotate(eulerAngleOffset, Space.Self);

            if (debug)
            {
                if (Input.GetKeyDown(KeyCode.CapsLock))
                {                    
                    Debug.DrawRay(transform.position, transform.forward * 30, Color.red, 60);
                    Debug.DrawRay(cam.transform.position, cam.transform.forward * 30, Color.green, 60);
                }
            }
        }
        else
        {
            //spine.LookAt(Vector3.zero);
        }
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

        if (crouch)
        {
            if (isRunning)
                movementSpeed = crouchSpeed * 2;
            else
                movementSpeed = crouchSpeed;
        }
        else
        {
            if (!isRunning)
                movementSpeed = WalkSpeed;
            else
                movementSpeed = RunSpeed;
        }   

        if (input.Current.MoveInput.x != 0)
        {
            local += right * input.Current.MoveInput.x;
            sidewaysAmount = input.Current.MoveInput.x;
        }
        else
        {
            sidewaysAmount = 0;
        }

        if (input.Current.MoveInput.z != 0)
        {
            local += lookDirection * input.Current.MoveInput.z;
            if (isRunning)
                forwardAmount = input.Current.MoveInput.z;
            else
                forwardAmount = map(input.Current.MoveInput.z, 0, 1, 0, 0.5f);
                //forwardAmount = input.Current.MoveInput.z / 1.5f;
        }

        return local.normalized * movementSpeed;
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
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
        sidewaysAmount = 0;
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

        Shoot();

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, 10.0f * controller.deltaTime);
    }
    private void Shoot()
    {
        if(aim && input.Current.MouseFire)
        {
            gun.Shoot();
            firing = true; // for one frame.            
        }
    }

    void Idle_LateUpdate()
    {

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
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement(true), RunAcceleration * controller.deltaTime);
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

        Shoot();
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
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement(false), WalkAcceleration * controller.deltaTime);
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

        Shoot();
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

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement(false), JumpAcceleration * controller.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;
        Shoot();
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
