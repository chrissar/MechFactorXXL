using System;
using System.Collections.Generic;
using UnityEngine;

public enum FireTeamRole {LEADER, NON_LEADER};

public class FireTeamAlly : Ally
{
	public const float kSensingProximityRadius = 5.0f;
	public const float kVisionConeRadius = 20.0f;
	public const float kVisionConeHalfAngle = 30.0f;

	[HideInInspector] public FireTeamRole fireTeamRole;
	[HideInInspector] public int fireTeamNumber;
	[HideInInspector] public int slotPosition;
	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public FireTeam fireTeam;
	[HideInInspector] public FireTeam targetEnemyTeam;
	[HideInInspector] public bool isDisabled;
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
		if (!isDisabled) {
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
			// Only consider enemies that the team is not already engaging.
			if (fireTeam.EngagedEnemyTeams.Contains (enemy.fireTeam)) {
				continue;
			}
			Vector3 enemyPosition = enemy.Position;
			// Get angle between the ally and the enemy as well as the distance between them.
			float angleToFaceEnemy = Vector3.Angle(enemyPosition, transform.position);
			float distanceToEnemy = Vector3.Distance (enemyPosition, transform.position);
			// Only consider enemies that are in line of sight of the ally or which 
			// are within the sensing proximity of the ally.
			if (distanceToEnemy < kSensingProximityRadius || (distanceToEnemy < kVisionConeRadius && 
				Math.Abs(angleToFaceEnemy) < kVisionConeHalfAngle)) {
				// Check if the enemy is the closest enemy found so far.
				if (closestEnemy == null || distanceToEnemy < closestEnemyDistance) {
					closestEnemy = enemy;
					closestEnemyDistance = distanceToEnemy;
				}
			}
		}

		return closestEnemy;
	}
		
	public bool IsEnemyVisible (){
		foreach (FireTeamAlly enemy in mEnemies) {
			Vector3 enemyPosition = enemy.Position;
			// Get angle between the ally and the enemy as well as the distance between them.
			float angleToFaceEnemy = Vector3.Angle(enemyPosition, transform.position);
			float distanceToEnemy = Vector3.Distance (enemyPosition, transform.position);
			// Only consider enemies that are in line of sight of the ally or which 
			// are within the sensing proximity of the ally.
			if (distanceToEnemy < kSensingProximityRadius || (distanceToEnemy < kVisionConeRadius && 
				Math.Abs(angleToFaceEnemy) < kVisionConeHalfAngle)) {
				return true;
			}
		}

		return false; // No enemies found.
	}

	protected void Initialize()
	{
		// Get the nav mesh agent on the object.
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.destination = transform.position;
		navMeshAgent.updateRotation = true;

		// Initialize member variables.
		fireTeamRole = FireTeamRole.NON_LEADER;
		fireTeam = null;
		mEnemies = new List<FireTeamAlly> ();
		targetEnemyTeam = null;
		fireTeamNumber = -1;
		slotPosition = -1;
		ClearDetachDestination ();
		isDisabled = false;

		// Initialize state machine and leader actions helper.
		mStateMachine = new FireTeamAllyStateMachine(this);
	}
}
