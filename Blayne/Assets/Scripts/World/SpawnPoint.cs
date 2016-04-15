using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	private const float mkRefillRadius = FireTeam.kMinDistanceFromSlotPositionNeeded;

	public Color friendColor;
	public Color enemyColor;
    public FireTeam fireTeamPrefab;
    public FireTeamAlly allyPrefab;
    public FireTeam.Side teamSide;
 
    public Material enemy;
    public Material friendly;

    private FireTeam mSpawnedFireTeam;
	private Renderer mRenderer;
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
        GameObject teamListObj = GameObject.Find("TeamList");
        if (!teamListObj) throw new UnityException("No TeamList object found in the scene. Make sure you instantiated the prefab.");
        TeamList teamList = teamListObj.GetComponent<TeamList>() as TeamList;
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
			// print ("Ally : " + ally.fireTeam.TeamSide);
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
				Vector3 planeDisplacement = ally.Position - transform.position;
				planeDisplacement = new Vector3 (planeDisplacement.x, 0, planeDisplacement.z);
				if (planeDisplacement.magnitude < mkRefillRadius) {
					// Refill the fire team to full.
					for (int j = 0; j < missingTeamMembers; ++j) {
						CreateNewFireTeamAlly ();
					}
					// Don't add any more characters to the fire team.
					break;
				}
			}
			// Since te fire team made it back to the spawn point, reset their 
			// fomration to wedge formation.
			mSpawnedFireTeam.SetFormation(FireTeamFormation.WEDGE);
		}
	}

	private void CreateNewFireTeamAlly()
	{
        GameObject allyObj = Instantiate(allyPrefab.gameObject);
		FireTeamAlly ally = allyObj.GetComponent<FireTeamAlly>();
		mSpawnedFireTeam.AddFireTeamAlly(ally);
		allyObj.transform.position = mSpawnedFireTeam.GetSlotPosition(ally.slotPosition);
		ally.StateMachine.currentMovementState.ToMoving();
		ally.targetEnemyTeam = mSpawnedFireTeam.GetAllyAtSlotPosition (0).targetEnemyTeam;
		SetTeamColorOfFireTeamAlly (ally);
	}

	private void SetTeamColorOfFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		mRenderer = fireTeamAlly.transform.FindChild("CorrectedKataphrakt3-Rigged Attempt 2").
			FindChild("upper torso").GetComponent<Renderer>();
		if (mRenderer != null) {
			if (mSpawnedFireTeam.TeamSide == FireTeam.Side.Friend) {
				mRenderer.sharedMaterial = new Material (friendly);
			} else {
				mRenderer.sharedMaterial = new Material (enemy);
			}
		}
	}
}

