// 
// Class used purely for testing out the team movement behaviour by assigning characters to teams.
// 
using System;
using UnityEngine;

public class AllyGameController : MonoBehaviour
{
	private TeamList teamList;
	private FireTeamAlly fireTeamLeaderAlly;

	void Start()
	{
		
		// Initialize the team list and add one fire team to it.
		teamList = new TeamList ();
		int fireTeamNumber = 0;
		FireTeam fireTeam = new FireTeam ();
		fireTeam.teamNumber = fireTeamNumber;
		teamList.AddTeamToListWithNumber (fireTeam, fireTeam.teamNumber);

		// Add the NPCs to the fire team.
		fireTeamLeaderAlly = null;
		GameObject[] npcGameObjects = GameObject.FindGameObjectsWithTag("NPC");
		foreach (GameObject npcGameObject in npcGameObjects) 
		{
			FireTeamAlly fireTeamAlly = npcGameObject.GetComponent<FireTeamAlly> () as FireTeamAlly;
			if (fireTeamAlly != null) 
			{
				// Place the fire team ally into the fire team in the team list.
				fireTeamAlly.fireTeamNumber = fireTeamNumber;
				fireTeamAlly.PlaceInFireTeam(teamList.getTeamWithNumber(fireTeamNumber));
				// Set one of the fire team allies to be the team leader.
				if (fireTeamLeaderAlly == null) 
				{
					fireTeamLeaderAlly = fireTeamAlly;
					fireTeamLeaderAlly.fireTeamRole = FireTeamRole.LEADER;
				}
			}
		}

		// Set formation of the fire team
		fireTeam.SetFormation(FireTeamFormation.WEDGE);
	}

	void Update()
	{
		// Give the move command to the leader if a movement button is pressed. For demonstration 
		// purposes right now.
		if (Input.GetKeyDown (KeyCode.W)) 
		{
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, 10));
			fireTeamLeaderAlly.executeCommand (moveCommand);
		} 
		else if (Input.GetKeyDown (KeyCode.S)) 
		{
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, -10));
			fireTeamLeaderAlly.executeCommand (moveCommand);
		} 
		else if (Input.GetKeyDown (KeyCode.A)) 
		{
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (-10, 0.5f, 0));
			fireTeamLeaderAlly.executeCommand (moveCommand);
		} 
		else if (Input.GetKeyDown (KeyCode.D)) 
		{
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (10, 0.5f, 0));
			fireTeamLeaderAlly.executeCommand (moveCommand);
		}
	}
}

