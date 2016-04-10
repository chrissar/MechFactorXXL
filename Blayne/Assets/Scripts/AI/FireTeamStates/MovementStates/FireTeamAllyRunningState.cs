using System;
using UnityEngine;

public class FireTeamAllyRunningState : IMovement
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public FireTeamAllyRunningState (FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{
		
	}

	public void OnStateEnter()
	{
		mStatePatternFTAlly.navMeshAgent.updateRotation = true;
	}

	public void OnStateExit()
	{

	}

	public void ToMoving()
	{
		mStateMachine.currentMovementState.OnStateExit ();
		mStateMachine.currentMovementState = mStateMachine.movingState;
		mStateMachine.currentMovementState.OnStateEnter();
	}

	public void ToIdling()
	{
		mStateMachine.currentMovementState.OnStateExit ();
		mStateMachine.currentMovementState = mStateMachine.idlingState;
		mStateMachine.currentMovementState.OnStateEnter();
	}

	public void ToSprinting()
	{

	}
}

