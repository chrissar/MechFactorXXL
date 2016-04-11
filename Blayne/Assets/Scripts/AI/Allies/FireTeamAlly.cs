using System;
using System.Collections.Generic;
using UnityEngine;

public class FireTeamAlly : Ally
{
	public const float kSensingProximityRadius = 15.0f;
	public const float kVisionConeRadius = 30.0f;
	public const float kVisionConeHalfAngle = 30.0f;

	[HideInInspector] public int fireTeamNumber;
	[HideInInspector] public int slotPosition;
	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public FireTeam fireTeam;
	[HideInInspector] public FireTeam targetEnemyTeam;
	private bool mIsDisabled;
	private Vector3 mDetachDestination;
	private bool mIsDetached;
	private List<FireTeamAlly> mEnemies;
	private FireTeamAllyStateMachine mStateMachine;

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
	public Vector3 DetachDestination
	{
		get
		{ 
			return mDetachDestination;
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
	public bool IsDetached
	{
		get 
		{
			return mIsDetached;
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
	}

	public void Update()
	{
		if (!mIsDisabled) {
			// Check for visible enemies.
			checkForVisibleEnemies ();

			// Update movement.
			mStateMachine.UpdateStates ();
		}
	}

	public void checkForVisibleEnemies(){
		FireTeamAlly closestVisibleEnemy = GetClosestNewVisibleEnemy ();
		if (closestVisibleEnemy != null) {
			// Notify the fire team leader (located at slot 0 in the fire team) of the sighted enemy.
			fireTeam.GetAllyAtSlotPosition(0).NotifyOfEnemy(closestVisibleEnemy);
		}
	}

	public void NotifyOfEnemy(FireTeamAlly enemy){
		print ("Enemy at " + enemy.Position + " Found by " + gameObject.name);
		// Add enemy to the list of engaged enemies.
		fireTeam.EngagedEnemyTeams.Add(enemy.fireTeam);
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

	public void SetDetachDestination(Vector3 detachDestination)
	{
		mDetachDestination = detachDestination;
		mIsDetached = true;
	}

	public void ClearDetachDestination()
	{
		mDetachDestination = Vector3.zero;
		mIsDetached = false;
	}

	// Returns the closest enemy to the ally that is visible to the ally. 
	// Can return null if no enemies are visible.
	public FireTeamAlly GetClosestNewVisibleEnemy (){
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
			if(IsFireTeamAllyVisible(enemy)){
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
		
	public bool IsAnyEnemyVisible (){
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

	public bool IsFireTeamAllyVisible(FireTeamAlly fireTeamAlly){
		Vector3 allyDisplacement = fireTeamAlly.Position - transform.position;
		Vector3 currentFacingDirection = transform.rotation * Vector3.forward;

		// Get angle between the ally and the enemy as well as the distance between them.
		float distanceToAlly = allyDisplacement.magnitude;
		float angleToFaceAlly = Vector3.Angle(currentFacingDirection, allyDisplacement);
		// Visible if within line of sight or is within the sensing proximity.
		if (distanceToAlly< kSensingProximityRadius || (angleToFaceAlly < kVisionConeRadius &&
			Math.Abs(angleToFaceAlly) < kVisionConeHalfAngle)) {
			return true;
		}
		return false;
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
		ClearDetachDestination ();
		mIsDisabled = false;

		// Initialize state machine and leader actions helper.
		mStateMachine = new FireTeamAllyStateMachine(this);
	}
}
