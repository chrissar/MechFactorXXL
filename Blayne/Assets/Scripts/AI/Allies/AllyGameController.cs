// 
// Class used purely for testing out the team movement behaviour by assigning characters to teams.
// 
using System;
using System.Collections.Generic;
using UnityEngine;

public class AllyGameController : MonoBehaviour
{
	public Color defaultColor;
	public Color disabledColor;
	private TeamList mTeamList;
	private int mTestedFireTeamNumber;
	private FireTeamAlly mFireTeamLeaderAlly;
	private FireTeamAlly ally0;
	private FireTeamAlly ally1;
	private FireTeamAlly ally2;
	private FireTeamAlly ally3;
	private bool mAlly0Disabled;
	private bool mAlly1Disabled;
	private bool mAlly2Disabled;
	private bool mAlly3Disabled;

	void Start()
	{
		// Initialize the team list and add one fire team to it.
		mTeamList = new TeamList ();
		mTestedFireTeamNumber = 0;
		FireTeam fireTeam = new FireTeam ();
		fireTeam.teamNumber = mTestedFireTeamNumber;
		mTeamList.AddTeamToListWithNumber (fireTeam, fireTeam.teamNumber);

		mAlly0Disabled = false;
		mAlly1Disabled = false;
		mAlly2Disabled = false;
		mAlly3Disabled = false;

		// Add the NPCs to the fire team.
		mFireTeamLeaderAlly = null;
		// Get ally NPCs.
		ally0 = GameObject.Find ("NPC0").GetComponent<FireTeamAlly>() as FireTeamAlly;
		ally1 = GameObject.Find ("NPC1").GetComponent<FireTeamAlly>() as FireTeamAlly;
		ally2 = GameObject.Find ("NPC2").GetComponent<FireTeamAlly>() as FireTeamAlly;
		ally3 = GameObject.Find ("NPC3").GetComponent<FireTeamAlly>() as FireTeamAlly;
		// Add allies to the fire team, with ally 0 being the leader.
		ally0.fireTeamRole = FireTeamRole.LEADER;
		ally0.fireTeamNumber = mTestedFireTeamNumber;
		ally0.PlaceInFireTeam(fireTeam);
		ally1.fireTeamNumber = mTestedFireTeamNumber;
		ally1.PlaceInFireTeam(fireTeam);
		ally2.fireTeamNumber = mTestedFireTeamNumber;
		ally2.PlaceInFireTeam(fireTeam);
		ally3.fireTeamNumber = mTestedFireTeamNumber;
		ally3.PlaceInFireTeam(fireTeam);
		mFireTeamLeaderAlly = ally0;

		// Set formation of the fire team.
		FireTeamFormationCommand wedgeCommand = new FireTeamFormationCommand(FireTeamFormation.WEDGE);
		mFireTeamLeaderAlly.executeCommand (wedgeCommand);
	}

	void Update()
	{
		acceptInput ();
		// Update the fire team's movement.
		FireTeam fireTeam = mTeamList.getTeamWithNumber(mTestedFireTeamNumber);
		fireTeam.UpdateAnchor();
	}

	private void acceptInput()
	{
		if (mFireTeamLeaderAlly != null) {
			DestinationInputs ();
			FormationInputs ();
			UnitRemovalInputs ();
		}
		UnitDisableInputs ();
	}

	private void DestinationInputs ()
	{
		// Give the move command to the leader if a movement button is pressed. For demonstration 
		// purposes right now.
		if (Input.GetKeyDown (KeyCode.W)) {
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, 10));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		} else if (Input.GetKeyDown (KeyCode.S)) {
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, -10));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		} else if (Input.GetKeyDown (KeyCode.A)) {
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (-10, 0.5f, 0));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		} else if (Input.GetKeyDown (KeyCode.D)) {
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (10, 0.5f, 0));
			mFireTeamLeaderAlly.executeCommand (moveCommand);
		}
	}

	private void FormationInputs ()
	{
		// Give the formation change command to the leader if a movement button is pressed. 
		// For demonstration  purposes right now.
		if (Input.GetKeyDown (KeyCode.E)) {
			// Give the set wedge formation command to the leader.
			FireTeamFormationCommand wedgeCommand = new FireTeamFormationCommand (FireTeamFormation.WEDGE);
			mFireTeamLeaderAlly.executeCommand (wedgeCommand);
		}
		else if (Input.GetKeyDown (KeyCode.Q)) {
			// Give the set file formation command to the leader.
			FireTeamFormationCommand fileCommand = new FireTeamFormationCommand (FireTeamFormation.FILE);
			mFireTeamLeaderAlly.executeCommand (fileCommand);
		}
	}

	private void UnitRemovalInputs ()
	{
		// Accept inputs for testing the removal of the leader.
		if (Input.GetKeyDown (KeyCode.R)) {
			FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
			FireTeamAlly replacementLeader = fireTeam.RemoveFireTeamAlly (mFireTeamLeaderAlly);
			// Remove the old team leader from the scene.
			Destroy (mFireTeamLeaderAlly.gameObject);
			mFireTeamLeaderAlly = replacementLeader;
		}
		// Accept inputs for testing the removal of the member at slot position 1.
		if (Input.GetKeyDown (KeyCode.T)) {
			FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
			FireTeamAlly slotPosition1Ally = fireTeam.GetAllyAtSlotPosition (1);
			if (slotPosition1Ally != null) {
				fireTeam.RemoveFireTeamAlly (slotPosition1Ally);
				// Remove the removed team member from the scene.
				Destroy (slotPosition1Ally.gameObject);
			}
		}
	}

	private void UnitDisableInputs ()
	{
		// Accept inputs for testing the disabling/enabling ally 0.
		if (Input.GetKeyDown (KeyCode.Alpha0)) {
			if (mAlly0Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (ally0);
				mFireTeamLeaderAlly = ally0;
				ChangeColorOfFireTeamAlly(ally0, defaultColor);
				mAlly0Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementLeader = fireTeam.DisableFireTeamAlly (ally0);
				mFireTeamLeaderAlly = replacementLeader;
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (ally0, disabledColor);
				mAlly0Disabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 1.
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if (mAlly1Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (ally1);
				ChangeColorOfFireTeamAlly(ally1, defaultColor);
				mAlly1Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (ally1);
				if (ally1 != null && ally1 == mFireTeamLeaderAlly) {
					mFireTeamLeaderAlly = replacementAlly;
				}
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (ally1, disabledColor);
				mAlly1Disabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 2.
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if (mAlly2Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (ally2);
				ChangeColorOfFireTeamAlly(ally2, defaultColor);
				mAlly2Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (ally2);
				if (ally2 != null && ally2 == mFireTeamLeaderAlly) {
					mFireTeamLeaderAlly = replacementAlly;
				}
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (ally2, disabledColor);
				mAlly2Disabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 2.
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			if (mAlly3Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (ally3);
				ChangeColorOfFireTeamAlly(ally3, defaultColor);
				mAlly3Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (ally3);
				if (ally3 != null && ally3 == mFireTeamLeaderAlly) {
					mFireTeamLeaderAlly = replacementAlly;
				}
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (ally3, disabledColor);
				mAlly3Disabled = true;
			}
		}
	}

	private void ChangeColorOfFireTeamAlly(FireTeamAlly fireTeamAlly, Color color)
	{
		Renderer renderer = fireTeamAlly.GetComponent<Renderer> ();
		if (renderer != null) {
			renderer.material.color = color;
		}
	}
}

