using System;
using UnityEngine;

public class FireTeamAllyAimingState : ICombat
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public  FireTeamAllyAimingState (FireTeamAlly statePatternFTAlly, 
		FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{
		// Face target enemy team.
		if (mStatePatternFTAlly.targetEnemyTeam != null) {
			FireTeamAlly closestTeamMember = 
				GetClosestTeamMemberInFireTeam (mStatePatternFTAlly.targetEnemyTeam);
			// If the enemy is sufficiently close, continue aiming.
			if (closestTeamMember != null &&
				Vector3.Distance (mStatePatternFTAlly.Position, 
					closestTeamMember.Position) < FireTeamAlly.kMaxAttackRange) {
				// Face the closest team member in the enemy team.
				rotateToFaceTarget (closestTeamMember.Position);
			} else {
				ToIdling ();
			}
		} else {
			ToIdling ();
		}
	}

	public void OnStateEnter()
	{
		mStatePatternFTAlly.navMeshAgent.updateRotation = false;
	}

	public void OnStateExit()
	{

	}
		
	public void ToIdling()
	{
		mStateMachine.currentCombatState.OnStateExit ();
		mStateMachine.currentCombatState = mStateMachine.idlingCombatState;
		mStateMachine.currentCombatState.OnStateEnter();
	}

	public void ToAiming()
	{

	}

	public void ToReloading()
	{

	}

	public void ToFiring()
	{

	}

	private void rotateToFaceTarget(Vector3 targetPoint)
	{
		Vector3 targetDirection = targetPoint - mStatePatternFTAlly.Position;
		// Do not consider vertical displacement.
		targetDirection = new Vector3 (targetDirection.x, 0, targetDirection.z);
		Quaternion targetRotation = Quaternion.FromToRotation (Vector3.forward, targetDirection);
		
		mStatePatternFTAlly.transform.rotation = 
			Quaternion.Slerp (mStatePatternFTAlly.transform.rotation, targetRotation, Time.deltaTime * 5);
	}

	private FireTeamAlly GetClosestTeamMemberInFireTeam(FireTeam fireTeam)
	{
		FireTeamAlly closestTeamMember = null;
		float closestDistance = -1.0f;
		// Find the member of the fire team that is closest to the state machine's fire team ally.
		for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
			FireTeamAlly teamMember = fireTeam.GetAllyAtSlotPosition (i);
			if (teamMember != null) {
				float distance = Vector3.Distance (mStatePatternFTAlly.Position, teamMember.Position);
				if (closestDistance < 0.0f || distance < closestDistance) {
					// Set the member as the closest team member found so far.
					closestTeamMember = teamMember;
					closestDistance = distance;
				}
			}
		}
		return closestTeamMember;
	}
}

