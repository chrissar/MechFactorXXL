using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	private const float mkRefillRadius = 5.0f;

	public Color friendColor;
	public Color enemyColor;
    public FireTeam fireTeamPrefab;
    public FireTeamAlly allyPrefab;
    public FireTeam.Side teamSide;

	private FireTeam mSpawnedFireTeam;
    private static int msTopTeamNumber = 0;
    private static int GetNewTeamNumber()
    {
        return msTopTeamNumber++;
    }
    public void Awake()
    {
        if (!fireTeamPrefab) throw new UnityException("Fire Team Prefab is null at a Spawn Point");
        if (!allyPrefab) throw new UnityException("Ally Prefab is null at a Spawn Point");

        GameObject fireTeamObject = Instantiate(fireTeamPrefab.gameObject);
		fireTeamObject.transform.position = gameObject.transform.position;
		mSpawnedFireTeam = fireTeamObject.GetComponent<FireTeam>();
		FireTeamDecisionMaker decisionMaker = 
			mSpawnedFireTeam.gameObject.AddComponent<FireTeamDecisionMaker> ();
		decisionMaker.fireTeam = mSpawnedFireTeam;
		mSpawnedFireTeam.TeamSide = teamSide;
		mSpawnedFireTeam.teamNumber = GetNewTeamNumber();
		mSpawnedFireTeam.spawnPoint = transform.position;
		SpawnFireTeamAtPosition (mSpawnedFireTeam, fireTeamObject.transform.position);
    }
    public void Start()
    {
        TeamList teamList = GameObject.Find("TeamList").GetComponent<TeamList>() as TeamList;
        // Add team to fire team list;
        teamList.AddTeamToListWithNumber(mSpawnedFireTeam, mSpawnedFireTeam.teamNumber);
        teamList.AddTeamsWithSameAlignmentToTeam(mSpawnedFireTeam);
    }

	private void SpawnFireTeamAtPosition(FireTeam team, Vector3 spawnPosition){
		team.SetDestination (spawnPosition); // Sets the destination that fire team moves towards
		team.CurrentAnchorPosition = spawnPosition; // Sets the fire team's current position.
		team.SetFormation(FireTeamFormation.WEDGE);

		for(int i=0; i<FireTeam.kMaxFireTeamMembers; i++)
		{
			GameObject allyObj = Instantiate(allyPrefab.gameObject);
			allyObj.transform.position = team.GetSlotPosition(i);
			FireTeamAlly ally = allyObj.GetComponent<FireTeamAlly>();
			team.AddFireTeamAlly(ally);
			ally.StateMachine.currentMovementState.ToMoving();
			SetTeamColorOfFireTeamAlly (ally);
		}
	}


	public void Update()
	{
		// If the spawned fire team is in the vicinity of the spawn point, refill any
		// missign members of the fire team.
		int missingTeamMembers = FireTeam.kMaxFireTeamMembers - mSpawnedFireTeam.MemberCount;
		// Only try to fill the team if it is incomplete.
		if (missingTeamMembers > 0) {
			for (int i = 0; i < mSpawnedFireTeam.MemberCount; ++i) {
				// Check if any members of the fire team are close to the spawn point.
				FireTeamAlly ally = mSpawnedFireTeam.GetAllyAtSlotPosition (i);
				if (Vector3.Distance (ally.Position, transform.position) < mkRefillRadius) {
					// Refill the fire team to full.
					for (int j = 0; j < missingTeamMembers; ++j) {
						CreateNewFireTeamAlly();
					}
				}
			}
		}
	}

	private void CreateNewFireTeamAlly()
	{
		GameObject allyObj = Instantiate(allyPrefab.gameObject);
		FireTeamAlly ally = allyObj.GetComponent<FireTeamAlly>();
		mSpawnedFireTeam.AddFireTeamAlly(ally);
		allyObj.transform.position = mSpawnedFireTeam.GetSlotPosition(ally.slotPosition);
		ally.StateMachine.currentMovementState.ToMoving();
		SetTeamColorOfFireTeamAlly (ally);
	}

	private void SetTeamColorOfFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		Renderer renderer = fireTeamAlly.GetComponent<Renderer> ();
		if (renderer != null) {
			if (mSpawnedFireTeam.TeamSide == FireTeam.Side.Friend) {
				renderer.material.color = friendColor;
			} else {
				renderer.material.color = enemyColor;
			}
		}
	}
}

