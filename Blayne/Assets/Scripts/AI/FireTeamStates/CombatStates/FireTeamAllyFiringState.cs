using UnityEngine;
using System.Collections;
using System;

public class FireTeamAllyFiringState : ICombat
{
    private readonly FireTeamAlly mStatePatternFTAlly;
    private FireTeamAllyStateMachine mStateMachine;

    public FireTeamAllyFiringState(FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine)
    {
        mStatePatternFTAlly = statePatternFTAlly;
        mStateMachine = stateMachine;
    }

    public void OnStateEnter()
    {
        mStatePatternFTAlly.Shoot();
    }

    public void OnStateExit()
    {

    }

    public void ToAiming()
    {
        mStateMachine.currentCombatState.OnStateExit();
        mStateMachine.currentCombatState = mStateMachine.aimingState;
        mStateMachine.currentCombatState.OnStateEnter();
    }

    public void ToFiring()
    {
        mStateMachine.currentCombatState.OnStateExit();
        mStateMachine.currentCombatState = mStateMachine.firingState;
        mStateMachine.currentCombatState.OnStateEnter();
    }

    public void ToIdling()
    {
		mStateMachine.currentCombatState.OnStateExit ();
		mStateMachine.currentCombatState = mStateMachine.idlingCombatState;
		mStateMachine.currentCombatState.OnStateEnter();
    }

    public void ToReloading()
    {
        
    }

    public void UpdateState()
    {
        if (mStatePatternFTAlly.targetEnemyTeam != null)
        {
            FireTeamAlly closestTeamMember = FireTeamHelper.GetClosestTeamMemberInFireTeam(mStatePatternFTAlly, mStatePatternFTAlly.targetEnemyTeam);
            // If the enemy is sufficiently close, continue aiming.
            if (closestTeamMember != null && Vector3.Distance(mStatePatternFTAlly.Position, closestTeamMember.Position) < FireTeamAlly.kVisionConeRadius)
            {
                // Face the closest team member in the enemy team and transition to firing state.
                FireTeamHelper.RotateToFaceTarget(mStatePatternFTAlly, closestTeamMember.Position);
                ToFiring();
            }
            else
            {
                ToAiming();
            }
        }
    }
}
