// 
// Class used purely for testing out the team movement behaviour by assigning characters to teams.
// 
using System;
using UnityEngine;

public class AllyGameController : MonoBehaviour
{
	private TeamList teamList;
	private int testedFireTeamNumber;
	private FireTeamAlly fireTeamLeaderAlly;

	void Start()
	{
		// Initialize the team list and add one fire team to it.
		teamList = new TeamList ();
		testedFireTeamNumber = 0;
		FireTeam fireTeam = new FireTeam ();
		fireTeam.teamNumber = testedFireTeamNumber;
		teamList.AddTeamToListWithNumber (fireTeam, fireTeam.teamNumber);

		// Add the NPCs to the fire team.
		fireTeamLeaderAlly = null;
		GameObject[] npcGameObjects = GameObject.FindGameObjectsWithTag("NPC");
		foreach (GameObject npcGameObject in npcGameObjects) 
		{
			FireTeamAlly fireTeamAlly = npcGameObject.GetComponent<FireTeamAlly> () as FireTeamAlly;
			if (fireTeamAlly != null) 
			{
				// Set one of the fire team allies to be the team leader.
				if (fireTeamLeaderAlly == null) 
				{
					fireTeamAlly.fireTeamRole = FireTeamRole.LEADER;
					fireTeamLeaderAlly = fireTeamAlly;
				}
				// Place the fire team ally into the fire team in the team list.
				fireTeamAlly.fireTeamNumber = testedFireTeamNumber;
				fireTeamAlly.PlaceInFireTeam(fireTeam);
			}
		}

		// Set formation of the fire team
		FireTeamFormationCommand wedgeCommand = new FireTeamFormationCommand(FireTeamFormation.WEDGE);
		fireTeamLeaderAlly.executeCommand (wedgeCommand);
		// Move team to default location.
		MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (Vector3.zero);
		fireTeamLeaderAlly.executeCommand (moveCommand);
	}

	void Update()
	{
		acceptInput ();
		// Update the fire team's movement.
		FireTeam fireTeam = teamList.getTeamWithNumber(testedFireTeamNumber);
		fireTeam.UpdateAnchor();
	}

	private void acceptInput()
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
		// Give the formation change command to the leader if a movement button is pressed. 
		// For demonstration  purposes right now.
		if(Input.GetKeyDown(KeyCode.E)){
			// Give the set wedge formation command to the leader.
			FireTeamFormationCommand wedgeCommand = new FireTeamFormationCommand(FireTeamFormation.WEDGE);
			fireTeamLeaderAlly.executeCommand (wedgeCommand);
		} else if(Input.GetKeyDown(KeyCode.Q)){
			// Give the set file formation command to the leader.
			FireTeamFormationCommand fileCommand = new FireTeamFormationCommand(FireTeamFormation.FILE);
			fireTeamLeaderAlly.executeCommand (fileCommand);
		}
	}
}

