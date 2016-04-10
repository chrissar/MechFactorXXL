using System;
using UnityEngine;

public class FireTeamAllyIdlingCombatState : ICombat
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public FireTeamAllyIdlingCombatState (FireTeamAlly statePatternFTAlly, 
		FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{
		// Check if a unit in the target enemy team is within max attack range.
		if (mStatePatternFTAlly.targetEnemyTeam != null) {
			FireTeamAlly closestTeamMember = 
				GetClosestTeamMemberInFireTeam (mStatePatternFTAlly.targetEnemyTeam);
			// If the enemy is sufficiently close, move to aiming state.
			if (closestTeamMember != null &&
				Vector3.Distance (mStatePatternFTAlly.Position, 
					closestTeamMember.Position) < FireTeamAlly.kMaxAttackRange) {
				ToAiming();
			} 
		} 
	}

	public void OnStateEnter()
	{
		mStatePatternFTAlly.navMeshAgent.updateRotation = true;
	}

	public void OnStateExit()
	{

	}

	public void ToIdling()
	{
		
	}

	public void ToAiming()
	{
		mStateMachine.currentCombatState.OnStateExit ();
		mStateMachine.currentCombatState = mStateMachine.aimingState;
		mStateMachine.currentCombatState.OnStateEnter();
	}

	public void ToReloading()
	{

	}

	public void ToFiring()
	{

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