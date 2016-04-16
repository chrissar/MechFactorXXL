using System;
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTeamAlly : Ally
{
    public AudioSource mGunSound;
    public AudioSource walking;
    private float audioStepLengthWalk = 0.75f;

    public const float kSensingProximityRadius = 20.0f;
	public const float kVisionConeRadius = 50.0f;
	public const float kVisionConeHalfAngle = 30.0f;

    public bool debugGunLOS = false;
    public bool debugSpineLOS = false;
    public bool debugTLOS = false;

    [HideInInspector] public int fireTeamNumber;
	[HideInInspector] public int slotPosition;
	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public FireTeam fireTeam;
	[HideInInspector] public FireTeam targetEnemyTeam;
	private bool mIsDisabled;
	private List<FireTeamAlly> mEnemies;
	private FireTeamAllyStateMachine mStateMachine;
    private Gun mGun;

    Animator animator;
    public bool aim = false;
    public bool firing = false;
    public bool crouch = false;
    public bool grounded = true;

    public float rotateAmount = 0f;
    public float forwardAmount = 0f;
    public float sidewaysAmount = 0f;

    private bool step = true;

    public GameObject LeftHandAimingPosition;
    public GameObject LeftHandIdlePosition;

    Vector3 movement;
    Vector3 prevPos;
    Vector3 newPos;
    Vector3 fwd;
    Vector3 side;

    float currentRotation;
    float previousRotation;

    // IK Stuff
    public Transform spine;
    public float aimingZ = 200;
    public float aimingY = 200;
    public float aimingX = 200;
    public float point;
    float IKWeight = 0;

    public FireTeamAlly currentTarget;
    Vector3 lookPosition;

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

    public Vector3 Position
	{
		get
		{ 
			return transform.position; 
		}
	}
	public Quaternion Orientation
	{
		get
		{ 
			return transform.rotation;
		}
	}
	public bool IsDisabled
	{
		get 
		{
			return mIsDisabled;
		}
		set
		{
			mIsDisabled = value;
			// Enable/Disable the nav mesh as appropriate.
			navMeshAgent.enabled = !mIsDisabled;
		}
	}

	public FireTeamAllyStateMachine StateMachine
	{
		get
		{ 
			return mStateMachine;
		}
	}

	public void Awake()
	{
		Initialize ();
        SetUpAnimator();
        SetTargetToDefault();
    }

    void OnAnimatorIK()
    {
        Transform LHP;

        if (aim)
        {
            LHP = LeftHandAimingPosition.transform;
        }
        else
        {
            LHP = LeftHandIdlePosition.transform;
        }

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IKWeight);

        Vector3 pos = LHP.transform.TransformPoint(Vector3.zero);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, LHP.transform.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LHP.transform.rotation);

        // right hand
        //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IKWeight);
        //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, IKWeight);

        //Vector3 rhp = LeftHandPosition.transform.TransformPoint(Vector3.zero);

        //animator.SetIKPosition(AvatarIKGoal.RightHand, LeftHandPosition.transform.position);
        //animator.SetIKRotation(AvatarIKGoal.RightHand, LeftHandPosition.transform.rotation);
    }

    public void Update()
	{
        IKWeight = Mathf.MoveTowards(IKWeight, (true) ? 1.0f : 0.0f, Time.deltaTime * 5);

        newPos = transform.position;
        movement = (newPos - prevPos);
        currentRotation = transform.rotation.eulerAngles.y;

        if (!mIsDisabled) {
			// Check for visible enemies.
			checkForVisibleEnemies ();

			// Update movement.
			mStateMachine.UpdateStates ();
            crouch = false;

            if (fireTeam.CurrentFireTeamFormation == FireTeamFormation.COVER)
            {
                crouch = true;
            }
            else
            {
                crouch = false;
            }

            if (mStateMachine.currentStatusState == mStateMachine.suppressedState)
            {
                aim = false;
                crouch = true;
            }

            if (mStateMachine.currentCombatState == mStateMachine.aimingState ||
                mStateMachine.currentCombatState == mStateMachine.firingState)
            {
                aim = true;
            }
            else
            {
                aim = false;
            }

            if (mStateMachine.currentCombatState == mStateMachine.firingState)
            {
                if (!mGunSound.isPlaying)
                    mGunSound.Play();
                //firing = true;
            }

            if (mStateMachine.currentMovementState == mStateMachine.movingState)
            {
                if (currentRotation > previousRotation)
                {

                    float amount = currentRotation / 360;
                    amount = ((currentRotation - previousRotation) / 90);
                    rotateAmount = 0.65f;
                    rotateAmount = amount;
                }
                else if (currentRotation < previousRotation)
                {
                    rotateAmount = -0.65f;
                    float amount = (previousRotation - currentRotation) / 90;
                    rotateAmount = -amount;
                }
                else
                {
                    rotateAmount = 0.0f;
                }

                if (Vector3.Dot(side, movement) < 0)
                {
                    sidewaysAmount = -1.0f;
                }
                else if (Vector3.Dot(side, movement) > 0)
                {
                    sidewaysAmount = 1.0f;
                }                 
                else
                {
                    sidewaysAmount = 0.0f;
                }

                if (Vector3.Dot(fwd, movement) < 0)
                {
                    if (step)
                        Walk();

                    forwardAmount = -0.65f;
                }
                else if (Vector3.Dot(fwd, movement) > 0)
                {
                    if (step)
                        Walk();

                    forwardAmount = 0.65f;
                }                    
                else
                {
                    forwardAmount = 0.0f;
                }                    
            }
            else if (mStateMachine.currentMovementState == mStateMachine.idlingState)
            {
                forwardAmount = 0.0f;
                sidewaysAmount = 0.0f;
                aim = false;
                crouch = false;
            }
		}
	}

    public void LateUpdate()
    {
        previousRotation = currentRotation;
        prevPos = newPos;
        fwd = transform.forward;
        side = transform.right;

        if (currentTarget != null && aim)
        {
            Vector3 eulerAngleOffset = Vector3.zero;
            eulerAngleOffset = new Vector3(aimingX, aimingY, aimingZ);
            spine.LookAt(currentTarget.transform.position);
            spine.Rotate(eulerAngleOffset, Space.Self);
        }

        if (fireTeam.TeamSide == FireTeam.Side.Enemy)
        {
            if (debugSpineLOS)
                Debug.DrawRay(spine.transform.position, spine.transform.forward * 40, Color.black);

            if (debugTLOS)
                Debug.DrawRay(transform.position, transform.forward * 40, Color.red);

            if (debugGunLOS)
                Debug.DrawRay(mGun.muzzle.transform.position, mGun.muzzle.transform.forward * 40, Color.green);
        }            
        else
        {
            if (debugSpineLOS)
                Debug.DrawRay(spine.transform.position, spine.transform.forward * 40, Color.white);

            if (debugTLOS)
                Debug.DrawRay(transform.position, transform.forward * 40, Color.blue);

            // doesn't work for some strange reason.            
            if (debugGunLOS)
                Debug.DrawRay(mGun.muzzle.transform.forward, mGun.muzzle.transform.forward * 40, Color.magenta);
        }           

        UpdateAnimator();
    }

    void SetTargetToDefault()
    {
        Vector3 eulerAngleOffset = Vector3.zero;
        eulerAngleOffset = new Vector3(aimingX, aimingY, aimingZ);
        Ray ray = new Ray(transform.position, transform.forward);
        lookPosition = ray.GetPoint(30);
        spine.LookAt(lookPosition);
        spine.Rotate(eulerAngleOffset, Space.Self);
    }

    void UpdateAnimator()
    {
        if (firing)
        {
            firing = false;
            animator.SetTrigger("Fire 0");
            
        }

        animator.SetBool("Crouch", crouch);
        animator.SetFloat("Sideways", sidewaysAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", rotateAmount, 0.1f, Time.deltaTime);
        animator.SetBool("Aim", aim);
        animator.SetBool("Grounded", grounded);
        animator.SetLayerWeight(1, 1);
    }

    public void OnDestroy()
	{
		// Remove self from fire team.
		fireTeam.RemoveFireTeamAlly(this);
	}

	public void checkForVisibleEnemies(){
		FireTeamAlly closestVisibleEnemy = GetClosestNewVisibleEnemy ();
		if (closestVisibleEnemy != null) {
			// Notify the fire team leader (located at slot 0 in the fire team) of the sighted enemy.
			NotifyOfEnemy(closestVisibleEnemy);
		}
	}

	public void NotifyOfEnemy(FireTeamAlly enemy){
		// Add enemy to the list of engaged enemies.
		fireTeam.EngagedEnemyTeams.Add(enemy.fireTeam);
	}

    public void Shoot()
    {
        if (mGun != null)
        {
            mGun.Shoot();
        }
    }

    
    IEnumerator WaitForFootSteps(float stepsLength)
    {
        step = false;
        yield return new WaitForSeconds(stepsLength);
        step = true;
    }

    void Walk()
    {
        walking.Play();
        StartCoroutine(WaitForFootSteps(audioStepLengthWalk));
    }

    public void SetEnemies ()
	{
		GameObject[] characterGameObjects = GameObject.FindGameObjectsWithTag ("NPC");
		foreach (GameObject characterGameObject in characterGameObjects) {
			FireTeamAlly character = characterGameObject.GetComponent<FireTeamAlly>() as FireTeamAlly;
			// If the character's fire team is not of the same alignment, 
			// set the character to be an enemy.
			if (character.fireTeam.TeamSide != fireTeam.TeamSide) {
				mEnemies.Add (character);
			}
		}
	}

	// Returns the closest enemy to the ally that is visible to the ally. 
	// Can return null if no enemies are visible.
	public FireTeamAlly GetClosestNewVisibleEnemy ()
	{
		FireTeamAlly closestEnemy = null;
		float closestEnemyDistance = -1.0f;
		foreach (FireTeamAlly enemy in mEnemies) {
			// Do not consider destroyed enemies.
			if (enemy == null) {
				continue;
			}
			// Only consider enemies that the team is not already engaging.
			if (fireTeam.EngagedEnemyTeams.Contains (enemy.fireTeam)) {
				continue;
			}
			// Only consider visible enemies.
			if(!IsFireTeamAllyVisible(enemy)){
				continue;
			}
			// If the enemy is the closest enemy so far, set it as the current closest enemy.
			float distanceToEnemy = Vector3.Distance(enemy.Position, transform.position);
			if(closestEnemyDistance < 0 || distanceToEnemy < closestEnemyDistance){
				closestEnemy = enemy;
				closestEnemyDistance = distanceToEnemy;
			}
		}

		return closestEnemy;
	}
		
	public bool IsAnyEnemyVisible ()
	{
		foreach (FireTeamAlly enemy in mEnemies) {
			// Do not consider destroyed enemies.
			if (enemy == null) {
				continue;
			}
			if(IsFireTeamAllyVisible(enemy)){
				return true;
			}
		}
		return false; // No enemies found.
	}

	public bool IsFireTeamAllyVisible(FireTeamAlly fireTeamAlly)
	{
		// If this ally is being suppressed by the given fire team ally, 
		// set it to visible.

		Vector3 allyDisplacement = fireTeamAlly.Position - transform.position;
		Vector3 currentFacingDirection = transform.rotation * Vector3.forward;

		// Get angle between the ally and the enemy as well as the distance between them.
		float distanceToAlly = allyDisplacement.magnitude;
		float angleToFaceAlly = Vector3.Angle(currentFacingDirection, allyDisplacement);
		// Visible if within line of sight or is within the sensing proximity.
		if (distanceToAlly < kSensingProximityRadius || (angleToFaceAlly < kVisionConeRadius &&
			Math.Abs(angleToFaceAlly) < kVisionConeHalfAngle)) {
			return true;
		}
		//if (fireTeam.TeamSide == FireTeam.Side.Enemy)
		//	print ("distance to ally: " + distanceToAlly);
		return false;
	}

	public void FiredUponByEnemy(FireTeamAlly firingEnemyFireTeamAlly)
	{
		if (fireTeam.EngagedEnemyTeams.Contains (firingEnemyFireTeamAlly.fireTeam) == false) {
			fireTeam.isBeingFiredUpon = true; // The fire team decision maker checks this.
			NotifyOfEnemy (firingEnemyFireTeamAlly);
		}
	}

	protected void Initialize()
	{
		// Get the nav mesh agent on the object.
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.destination = transform.position;
		navMeshAgent.updateRotation = true;

		// Initialize member variables.
		fireTeam = null;
		mEnemies = new List<FireTeamAlly> ();
		targetEnemyTeam = null;
		fireTeamNumber = -1;
		slotPosition = -1;
		mIsDisabled = false;
        mGun = GetComponentInChildren<Gun>();

		// Initialize state machine and leader actions helper.
		mStateMachine = new FireTeamAllyStateMachine(this);
	}
}
