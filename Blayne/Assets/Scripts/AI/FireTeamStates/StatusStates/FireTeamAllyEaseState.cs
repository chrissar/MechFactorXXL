using System;
using UnityEngine;

public class FireTeamAllyEaseState : IStatus
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public FireTeamAllyEaseState(FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}


	public void UpdateState()
	{

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
}

