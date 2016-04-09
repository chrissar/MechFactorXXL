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
    public FireTeam fireTeamPrefab;
	private TeamList mTeamList;
	private int mTestedAllyFireTeamNumber;
	private int mTestedEnemyFireTeamNumber;
	private FireTeamAlly mFireTeamLeaderAlly;
	private FireTeamAlly mAlly0;
	private FireTeamAlly mAlly1;
	private FireTeamAlly mAlly2;
	private FireTeamAlly mAlly3;
	private FireTeamAlly mEnemy0;
	private bool mAlly0Disabled;
	private bool mAlly1Disabled;
	private bool mAlly2Disabled;
	private bool mAlly3Disabled;

	void Start()
	{
		// Initialize the team list and create the fire teams to add to it.
		mTeamList = new TeamList ();
		mTestedAllyFireTeamNumber = 0;
		mTestedEnemyFireTeamNumber = 1;
        if (!fireTeamPrefab) throw new UnityException("No Fire Team Prefab attached to the Ally Game Controller");
		FireTeam allyFireTeam = Instantiate(fireTeamPrefab);
		allyFireTeam.TeamSide = FireTeam.Side.Friend;
		allyFireTeam.teamNumber = mTestedAllyFireTeamNumber;
		mTeamList.AddTeamToListWithNumber (allyFireTeam, allyFireTeam.teamNumber);
		FireTeam enemyFireTeam = Instantiate(fireTeamPrefab);
		enemyFireTeam.TeamSide = FireTeam.Side.Enemy;
		enemyFireTeam.teamNumber = mTestedEnemyFireTeamNumber;
		mTeamList.AddTeamToListWithNumber (enemyFireTeam, enemyFireTeam.teamNumber);

		mAlly0Disabled = false;
		mAlly1Disabled = false;
		mAlly2Disabled = false;
		mAlly3Disabled = false;

		// Add the NPCs to the fire team.
		mFireTeamLeaderAlly = null;
		// Get NPCs.
		mAlly0 = GameObject.Find ("NPC0").GetComponent<FireTeamAlly>() as FireTeamAlly;
		mAlly1 = GameObject.Find ("NPC1").GetComponent<FireTeamAlly>() as FireTeamAlly;
		mAlly2 = GameObject.Find ("NPC2").GetComponent<FireTeamAlly>() as FireTeamAlly;
		mAlly3 = GameObject.Find ("NPC3").GetComponent<FireTeamAlly>() as FireTeamAlly;
		mEnemy0 = GameObject.Find ("NPC4").GetComponent<FireTeamAlly>() as FireTeamAlly;

		// Add allies to the friendly fire team, with ally 0 being the leader.
		mAlly0.fireTeamRole = FireTeamRole.LEADER;
		allyFireTeam.AddFireTeamAlly (mAlly0);
		allyFireTeam.AddFireTeamAlly (mAlly1);
		allyFireTeam.AddFireTeamAlly (mAlly2);
		allyFireTeam.AddFireTeamAlly (mAlly3);
		mFireTeamLeaderAlly = mAlly0;

		// Add enemies to the enemy fire team, with enemy 0 being the leader.
		mEnemy0.fireTeamRole = FireTeamRole.LEADER;
		enemyFireTeam.AddFireTeamAlly (mEnemy0);

		// Have the characters set their enemies.
		mAlly0.SetEnemies();
		mAlly1.SetEnemies();
		mAlly2.SetEnemies();
		mAlly3.SetEnemies();
		mEnemy0.SetEnemies();
	}

	void Update()
	{
		acceptInput ();
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
			ChangeFireTeamFormationCommand wedgeCommand = new ChangeFireTeamFormationCommand (FireTeamFormation.WEDGE);
			mFireTeamLeaderAlly.executeCommand (wedgeCommand);
		}
		else if (Input.GetKeyDown (KeyCode.Q)) {
			// Give the set file formation command to the leader.
			ChangeFireTeamFormationCommand fileCommand = new ChangeFireTeamFormationCommand (FireTeamFormation.FILE);
			mFireTeamLeaderAlly.executeCommand (fileCommand);
		}
	}

	private void UnitRemovalInputs ()
	{
		// Accept inputs for testing the removal of the leader.
		if (Input.GetKeyDown (KeyCode.R)) {
			FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
			FireTeamAlly replacementLeader = fireTeam.RemoveFireTeamAlly (mFireTeamLeaderAlly);
			// Remove the old team leader from the scene.
			Destroy (mFireTeamLeaderAlly.gameObject);
			mFireTeamLeaderAlly = replacementLeader;
		}
		// Accept inputs for testing the removal of the member at slot position 1.
		if (Input.GetKeyDown (KeyCode.T)) {
			FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
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
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly0);
				mFireTeamLeaderAlly = mAlly0;
				ChangeColorOfFireTeamAlly(mAlly0, defaultColor);
				mAlly0Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementLeader = fireTeam.DisableFireTeamAlly (mAlly0);
				mFireTeamLeaderAlly = replacementLeader;
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly0, disabledColor);
				mAlly0Disabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 1.
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if (mAlly1Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly1);
				ChangeColorOfFireTeamAlly(mAlly1, defaultColor);
				mAlly1Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (mAlly1);
				if (mAlly1 != null && mAlly1 == mFireTeamLeaderAlly) {
					mFireTeamLeaderAlly = replacementAlly;
				}
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly1, disabledColor);
				mAlly1Disabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 2.
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if (mAlly2Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly2);
				ChangeColorOfFireTeamAlly(mAlly2, defaultColor);
				mAlly2Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (mAlly2);
				if (mAlly2 != null && mAlly2 == mFireTeamLeaderAlly) {
					mFireTeamLeaderAlly = replacementAlly;
				}
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly2, disabledColor);
				mAlly2Disabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 2.
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			if (mAlly3Disabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly3);
				ChangeColorOfFireTeamAlly(mAlly3, defaultColor);
				mAlly3Disabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (mAlly3);
				if (mAlly3 != null && mAlly3 == mFireTeamLeaderAlly) {
					mFireTeamLeaderAlly = replacementAlly;
				}
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly3, disabledColor);
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

