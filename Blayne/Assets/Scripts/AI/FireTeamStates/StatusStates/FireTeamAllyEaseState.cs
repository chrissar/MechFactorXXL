using System;
using UnityEngine;

public class FireTeamAllyEaseState : IStatus
{
	private const float mkSensingProximityRadius = 5.0f;
	private const float mkVisionConeRadius = 10.0f;
	private const float mkVisionConeHalfAngle = 30.0f;

	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;
	private GameObject[] mEnemyGameObjects;

	public FireTeamAllyEaseState(FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
		mEnemyGameObjects = GameObject.FindGameObjectsWithTag ("Enemy");
	}


	public void UpdateState()
	{
		// Check for enemies.
		GameObject closestVisibleEnemy = GetClosestVisibleEnemy ();
		if (closestVisibleEnemy != null) {
			// Notify the fire team leader (located at slot 0 in the fire team) of the sighted enemy.
			mStatePatternFTAlly.fireTeam.GetAllyAtSlotPosition(0).NotifyOfEnemy(closestVisibleEnemy);
		}
	}

	public void OnStateEnter()
	{

	}

	public void OnStateExit()
	{

	}

	public void ToAlert()
	{

	}

	public void ToAtEase()
	{

	}

	public void ToSuppressed()
	{
		mStateMachine.currentStatusState.OnStateExit ();
		mStateMachine.currentStatusState = mStateMachine.suppressedState;
		mStateMachine.currentStatusState.OnStateEnter();
	}

	// Returns the closest enemy to the ally that is visible to the ally. 
	// Can return null if no enemies are visible.
	private GameObject GetClosestVisibleEnemy (){
		GameObject closestEnemy = null;
		float closestEnemyDistance = -1.0f;
		foreach (GameObject enemyGameObject in mEnemyGameObjects) {
			Vector3 enemyPosition = enemyGameObject.transform.position;
			// Get angle between the ally and the enemy as well as the distance between them.
			float angleToFaceEnemy = Vector3.Angle(enemyPosition, mStatePatternFTAlly.Position);
			float distanceToEnemy = Vector3.Distance (enemyPosition, mStatePatternFTAlly.Position);
			// Only consider enemies that are in line of sight of the ally or which 
			// are within the sensing proximity of the ally.
			if (distanceToEnemy < mkSensingProximityRadius || (distanceToEnemy < mkVisionConeRadius && 
				Math.Abs(angleToFaceEnemy) < mkVisionConeHalfAngle)) {
				// Check if the enemy is the closest enemy found so far.
				if (closestEnemy == null || distanceToEnemy < closestEnemyDistance) {
					closestEnemy = enemyGameObject;
					closestEnemyDistance = distanceToEnemy;
				}
			}
		}

		return closestEnemy;
	}
}

