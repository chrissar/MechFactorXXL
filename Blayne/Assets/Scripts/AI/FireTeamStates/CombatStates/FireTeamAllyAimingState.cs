using System;
using UnityEngine;

public class FireTeamAllyAimingState : ICombat
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public  FireTeamAllyAimingState (FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine)
    {
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{
		// Face target enemy team.
		if (mStatePatternFTAlly.targetEnemyTeam != null)
        {
			FireTeamAlly closestTeamMember = FireTeamHelper.GetClosestTeamMemberInFireTeam (mStatePatternFTAlly, mStatePatternFTAlly.targetEnemyTeam);
			// If the enemy is sufficiently close, continue aiming.
			if (closestTeamMember != null && Vector3.Distance (mStatePatternFTAlly.Position, closestTeamMember.Position) < FireTeamAlly.kVisionConeRadius)
            {
				// Face the closest team member in the enemy team and transition to firing state.
				FireTeamHelper.RotateToFaceTarget(mStatePatternFTAlly, closestTeamMember.Position);
				// The fire team member fired upon should be aware of the enemy attacking it.
				closestTeamMember.FiredUponByEnemy(mStatePatternFTAlly);
                ToFiring();
			}
            else
            {
				ToIdling ();
			}
		}

        else
        {
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
        mStateMachine.currentCombatState.OnStateExit();
        mStateMachine.currentCombatState = mStateMachine.firingState;
        mStateMachine.currentCombatState.OnStateEnter();
    }


}

