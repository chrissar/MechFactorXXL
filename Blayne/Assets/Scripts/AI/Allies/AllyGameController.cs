// 
// Class used purely for testing out the team movement behaviour by assigning characters to teams.
// 
using System;
using UnityEngine;

public class AllyGameController : MonoBehaviour
{
	private TeamList teamList;
	private int testedFireTeamNumber;
	private FireTeamAlly mFireTeamLeaderAlly;

	void Start()
	{
		// Initialize the team list and add one fire team to it.
		teamList = new TeamList ();
		testedFireTeamNumber = 0;
		FireTeam fireTeam = new FireTeam ();
		fireTeam.teamNumber = testedFireTeamNumber;
		teamList.AddTeamToListWithNumber (fireTeam, fireTeam.teamNumber);

		// Add the NPCs to the fire team.
		mFireTeamLeaderAlly = null;
		GameObject[] npcGameObjects = GameObject.FindGameObjectsWithTag("NPC");
		foreach (GameObject npcGameObject in npcGameObjects) 
		{
			FireTeamAlly fireTeamAlly = npcGameObject.GetComponent<FireTeamAlly> () as FireTeamAlly;
			if (fireTeamAlly != null) 
			{
				// Set one of the fire team allies to be the team leader.
				if (mFireTeamLeaderAlly == null) 
				{
					fireTeamAlly.fireTeamRole = FireTeamRole.LEADER;
					mFireTeamLeaderAlly = fireTeamAlly;
				}
				// Place the fire team ally into the fire team in the team list.
				fireTeamAlly.fireTeamNumber = testedFireTeamNumber;
				fireTeamAlly.PlaceInFireTeam(fireTeam);
			}
		}

		// Set formation of the fire team
		FireTeamFormationCommand wedgeCommand = new FireTeamFormationCommand(FireTeamFormation.WEDGE);
		mFireTeamLeaderAlly.executeCommand (wedgeCommand);
		// Move team to default location.
		MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (Vector3.zero);
		mFireTeamLeaderAlly.executeCommand (moveCommand);
	}

	void Update()
	{
		if (mFireTeamLeaderAlly != null) {
			acceptInput ();
		}
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
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		} 
		else if (Input.GetKeyDown (KeyCode.S)) 
		{
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, -10));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		} 
		else if (Input.GetKeyDown (KeyCode.A)) 
		{
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (-10, 0.5f, 0));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		} 
		else if (Input.GetKeyDown (KeyCode.D)) 
		{
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (10, 0.5f, 0));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		}

		// Give the formation change command to the leader if a movement button is pressed. 
		// For demonstration  purposes right now.
		if(Input.GetKeyDown(KeyCode.E)){
			// Give the set wedge formation command to the leader.
			FireTeamFormationCommand wedgeCommand = new FireTeamFormationCommand(FireTeamFormation.WEDGE);
			mFireTeamLeaderAlly.executeCommand (wedgeCommand);
		} else if(Input.GetKeyDown(KeyCode.Q)){
			// Give the set file formation command to the leader.
			FireTeamFormationCommand fileCommand = new FireTeamFormationCommand(FireTeamFormation.FILE);
			mFireTeamLeaderAlly.executeCommand (fileCommand);
		}

		// Accept inputs for testing the disabling of the leader.
		if(Input.GetKeyDown(KeyCode.R)){
			FireTeam fireTeam = teamList.getTeamWithNumber(testedFireTeamNumber);
			FireTeamAlly replacementLeader = fireTeam.RemoveFireTeamAlly (mFireTeamLeaderAlly);
			// Remove the old team leader from the scene.
			Destroy(mFireTeamLeaderAlly.gameObject);
			mFireTeamLeaderAlly = replacementLeader;
		}

		// Accept inputs for testing the disabling of the member at slot position 1.
		if(Input.GetKeyDown(KeyCode.T)){
			FireTeam fireTeam = teamList.getTeamWithNumber(testedFireTeamNumber);
			FireTeamAlly slotPosition1Ally = fireTeam.GetAllyAtSlotPosition(1);
			if (slotPosition1Ally != null) {
				fireTeam.RemoveFireTeamAlly (slotPosition1Ally);
				// Remove the removed team member from the scene.
				Destroy (slotPosition1Ally.gameObject);
			}
		}
	}
}

