using System;
using UnityEngine;

public class FireTeamAllyIdlingCombatState : ICombat
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public FireTeamAllyIdlingCombatState (FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine)
    {
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{
		// Check if a unit in the target enemy team is within max attack range.
		if (mStatePatternFTAlly.targetEnemyTeam != null)
        {
			FireTeamAlly closestTeamMember = FireTeamHelper.GetClosestTeamMemberInFireTeam(mStatePatternFTAlly, mStatePatternFTAlly.targetEnemyTeam);
			// If the enemy is sufficiently close, move to aiming state.
			if (closestTeamMember != null && Vector3.Distance (mStatePatternFTAlly.Position, closestTeamMember.Position) < FireTeamAlly.kVisionConeRadius)
            {
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
}