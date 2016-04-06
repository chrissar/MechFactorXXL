using System;
using UnityEngine;

public enum FireTeamRole {LEADER, NON_LEADER};

public class FireTeamAlly : Ally
{
	public FireTeam fireTeam;
	public int fireTeamNumber;
	public int slotPosition;
	public FireTeamRole fireTeamRole;
	protected NavMeshAgent mNavMeshAgent;

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
		// Move to position marked by slot position
		if (fireTeam != null) 
		{
			mNavMeshAgent.destination = fireTeam.getSlotPosition (slotPosition);
		} 
	}

	public void PlaceInFireTeam(FireTeam newfireTeam)
	{
		if (newfireTeam != null) 
		{
			fireTeam = newfireTeam;
			fireTeam.AddFireTeamAlly (this);
		}
	}

	protected void Initialize()
	{
		fireTeam = null;
		fireTeamNumber = -1;
		slotPosition = -1;
		fireTeamRole = FireTeamRole.NON_LEADER;

		// Get the nav mesh agent on the object.
		mNavMeshAgent = GetComponent<NavMeshAgent> ();
		mNavMeshAgent.destination = transform.position;
	}
}
