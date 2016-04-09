using System;
using UnityEngine;

public enum FireTeamRole {LEADER, NON_LEADER};

public class FireTeamAlly : Ally
{
	[HideInInspector] public FireTeamRole fireTeamRole;
	[HideInInspector] public FireTeam fireTeam;
	[HideInInspector] public int fireTeamNumber;
	[HideInInspector] public int slotPosition;
	[HideInInspector] public IMovement currentMovementState;
	[HideInInspector] public FireTeamAllyMovingState movingState;
	[HideInInspector] public FireTeamAllyIdlingState idlingState;
	public NavMeshAgent navMeshAgent;

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

	void Awake()
	{
		Initialize ();
	}

	void Update()
	{
		// Update movement.
		currentMovementState.UpdateState();
	}

	public void PlaceInFireTeam(FireTeam newfireTeam)
	{
		if (newfireTeam != null) {
			fireTeam = newfireTeam;
			fireTeam.AddFireTeamAlly (this);
		} 
	}
		
	public void OnEnterMovementState(){
		currentMovementState.OnStateEnter ();
	}

	protected void Initialize()
	{
		// Get the nav mesh agent on the object.
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.destination = transform.position;

		// Initialize member variables.
		fireTeamRole = FireTeamRole.NON_LEADER;
		fireTeam = null;
		fireTeamNumber = -1;
		slotPosition = -1;

		// Set states.
		movingState = new FireTeamAllyMovingState (this);
		idlingState = new FireTeamAllyIdlingState (this);
		currentMovementState = idlingState;
		OnEnterMovementState ();
	}
}
