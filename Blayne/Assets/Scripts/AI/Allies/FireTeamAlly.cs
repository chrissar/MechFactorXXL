using System;
using UnityEngine;

public enum FireTeamRole {LEADER, NON_LEADER};

public class FireTeamAlly : Ally
{
	[HideInInspector] public FireTeamRole fireTeamRole;
	[HideInInspector] public FireTeam fireTeam;
	[HideInInspector] public int fireTeamNumber;
	[HideInInspector] public int slotPosition;
	[HideInInspector] public NavMeshAgent navMeshAgent;
	private FireTeamAllyStateMachine mStateMachine;
	private FireTeamLeaderActionHelper mFireTeamLeaderHelper;

    public FireTeam.Side Side
    {
        get
        {
            return fireTeam.side;
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
	public FireTeamAllyStateMachine StateMachine
	{
		get
		{ 
			return mStateMachine;
		}
	}

	void Awake()
	{
		Initialize ();
	}

	void Update()
	{
		// Update movement.
		mStateMachine.UpdateStates();
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

		// Initialize state machine and leader actions helper.
		mStateMachine = new FireTeamAllyStateMachine(this);
		mFireTeamLeaderHelper = new FireTeamLeaderActionHelper (this);
	}

	public void PlaceInFireTeam(FireTeam newfireTeam)
	{
		if (newfireTeam != null) {
			fireTeam = newfireTeam;
			fireTeam.AddFireTeamAlly (this);
		} 
	}

	public void NotifyOfEnemy(GameObject enemyGameObject){
		print ("Enemy at " + enemyGameObject.transform.position);
		mFireTeamLeaderHelper.MoveTeamToCoverForEnemy ();
	}
}
