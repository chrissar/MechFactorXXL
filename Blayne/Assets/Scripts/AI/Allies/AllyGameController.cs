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
	public Camera mainCamera;
    public FireTeam fireTeamPrefab;
	private FireTeam mAllyFireTeam;
	private FireTeam mEnemyFireTeam;
	private TeamList mTeamList;
	private int mTestedAllyFireTeamNumber;
	private int mTestedEnemyFireTeamNumber;
	private FireTeamAlly mAlly0;
	private FireTeamAlly mAlly1;
	private FireTeamAlly mAlly2;
	private FireTeamAlly mAlly3;
	private FireTeamAlly mEnemy0;
	private FireTeamAlly mEnemy1;

	public void Start()
	{
		// Initialize the team list and create the fire teams to add to it.
		mTeamList = GameObject.Find ("TeamList").GetComponent<TeamList>() as TeamList;
        if (!fireTeamPrefab) throw new UnityException("No Fire Team Prefab attached to the Ally Game Controller");
		// Get ally fire team.
		mTestedAllyFireTeamNumber = 1;
		mTestedEnemyFireTeamNumber = 0;
		mAllyFireTeam = mTeamList.getTeamWithNumber(mTestedAllyFireTeamNumber);
		// Get enemy fire team.
		mEnemyFireTeam = mTeamList.getTeamWithNumber(mTestedEnemyFireTeamNumber);

		// Add the NPCs to the fire team.
		// Get NPCs.
		mAlly0 = mAllyFireTeam.GetAllyAtSlotPosition(0);
		mAlly1 = mAllyFireTeam.GetAllyAtSlotPosition(1);
		mAlly2 = mAllyFireTeam.GetAllyAtSlotPosition(2);
		mAlly3 = mAllyFireTeam.GetAllyAtSlotPosition(3);
		mEnemy0 = mEnemyFireTeam.GetAllyAtSlotPosition(0);
		mEnemy1 = mEnemyFireTeam.GetAllyAtSlotPosition(1);

		// Have the characters set their enemies.
		mAlly0.SetEnemies();
		mAlly1.SetEnemies();
		mAlly2.SetEnemies();
		mAlly3.SetEnemies();
		mEnemy0.SetEnemies();
		mEnemy1.SetEnemies();

		// Set default formation.
		new ChangeFireTeamFormationCommand(FireTeamFormation.WEDGE).execute(mAllyFireTeam);
		new ChangeFireTeamFormationCommand(FireTeamFormation.WEDGE).execute(mEnemyFireTeam);
	}

	void Update()
	{
		acceptInput ();
	}

	private void acceptInput()
	{
		if (mAllyFireTeam.MemberCount > 0) {
			DestinationInputs ();
			FormationInputs ();
			UnitRemovalInputs ();
		}
		UnitDisableInputs ();
		CameraMovementInputs ();
	}

	private void DestinationInputs ()
	{
		// Give the move command to the leader if a movement button is pressed. For demonstration 
		// purposes right now.
		if (Input.GetKeyDown (KeyCode.W)) {
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, 10));
			mAllyFireTeam.executeCommand (moveCommand);
		} else if (Input.GetKeyDown (KeyCode.S)) {
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (0, 0.5f, -10));
			mAllyFireTeam.executeCommand (moveCommand);
		} else if (Input.GetKeyDown (KeyCode.A)) {
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (-10, 0.5f, 0));
			mAllyFireTeam.executeCommand (moveCommand);
		} else if (Input.GetKeyDown (KeyCode.D)) {
			// Give the move command to the leader.
			MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (new Vector3 (10, 0.5f, 0));
			mAllyFireTeam.executeCommand (moveCommand);
		}
	}

	private void FormationInputs ()
	{
		// Give the formation change command to the leader if a movement button is pressed. 
		// For demonstration  purposes right now.
		if (Input.GetKeyDown (KeyCode.E)) {
			// Give the set wedge formation command to the leader.
			ChangeFireTeamFormationCommand wedgeCommand = new ChangeFireTeamFormationCommand (FireTeamFormation.WEDGE);
			mAllyFireTeam.executeCommand (wedgeCommand);
		}
		else if (Input.GetKeyDown (KeyCode.Q)) {
			// Give the set file formation command to the leader.
			ChangeFireTeamFormationCommand fileCommand = new ChangeFireTeamFormationCommand (FireTeamFormation.FILE);
			mAllyFireTeam.executeCommand (fileCommand);
		}
	}

	private void UnitRemovalInputs ()
	{
		// Accept inputs for testing the removal of the member at slot posiiton 0.
		if (Input.GetKeyDown (KeyCode.R)) {
			FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
			// Get member at slot 0.
			FireTeamAlly allyToRemove = fireTeam.GetAllyAtSlotPosition(0);
			if (allyToRemove != null) {
				// Destroy the old team leader in the scene.
				Destroy (allyToRemove.gameObject);
			}
		}
		// Accept inputs for testing the removal of the member at slot position 1.
		if (Input.GetKeyDown (KeyCode.T)) {
			FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
			FireTeamAlly slotPosition1Ally = fireTeam.GetAllyAtSlotPosition (1);
			if (slotPosition1Ally != null) {
				// Destroy the removed team member in the scene.
				Destroy (slotPosition1Ally.gameObject);
			}
		}

		// Accept inputs for testing the removal of the enemy member at slot posiiton 0.
		if (Input.GetKeyDown (KeyCode.G)) {
			// Get member at slot 0.
			FireTeamAlly enemyToRemove = mEnemyFireTeam.GetAllyAtSlotPosition(0);
			if (enemyToRemove != null) {
				// Destroy the old team leader in the scene.
				Destroy (enemyToRemove.gameObject);
			}
		}
	}

	private void UnitDisableInputs ()
	{
		// Accept inputs for testing the disabling/enabling ally 0.
		if (Input.GetKeyDown (KeyCode.Alpha0)) {
			if (mAlly0.IsDisabled) {
				mAlly0.IsDisabled = false;
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly0);
				ChangeColorOfFireTeamAlly(mAlly0, defaultColor);
			} else {
				mAlly0.IsDisabled = true;
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				fireTeam.DisableFireTeamAlly (mAlly0);
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly0, disabledColor);
			}
		}
		// Accept inputs for testing the disabling/enabling ally 1.
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if (mAlly1.IsDisabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly1);
				ChangeColorOfFireTeamAlly(mAlly1, defaultColor);
				mAlly1.IsDisabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				fireTeam.DisableFireTeamAlly (mAlly1);
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly1, disabledColor);
				mAlly1.IsDisabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 2.
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if (mAlly2.IsDisabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly2);
				ChangeColorOfFireTeamAlly(mAlly2, defaultColor);
				mAlly2.IsDisabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (mAlly2);
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly2, disabledColor);
				mAlly2.IsDisabled = true;
			}
		}
		// Accept inputs for testing the disabling/enabling ally 2.
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			if (mAlly3.IsDisabled) {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Enable ally 0 and set it as the leader.
				fireTeam.EnableFireTeamAlly (mAlly3);
				ChangeColorOfFireTeamAlly(mAlly3, defaultColor);
				mAlly3.IsDisabled = false;
			} else {
				FireTeam fireTeam = mTeamList.getTeamWithNumber (mTestedAllyFireTeamNumber);
				// Set the current leader as the ally that replaces the original leader.
				FireTeamAlly replacementAlly = fireTeam.DisableFireTeamAlly (mAlly3);
				// Change the color of the disabled original leader.
				ChangeColorOfFireTeamAlly (mAlly3, disabledColor);
				mAlly3.IsDisabled = true;
			}
		}
	}

	private void CameraMovementInputs ()
	{
		if (Input.GetKey (KeyCode.LeftArrow)) {
			mainCamera.transform.position += Vector3.left * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			mainCamera.transform.position += Vector3.right * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			mainCamera.transform.position += Vector3.forward * 5 * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			mainCamera.transform.position += Vector3.back * 5 * Time.deltaTime;
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

