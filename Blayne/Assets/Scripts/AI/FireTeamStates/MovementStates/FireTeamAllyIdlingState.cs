using System;

public class FireTeamAllyIdlingState : IMovement
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public FireTeamAllyIdlingState (FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{

	}

	public void OnStateEnter()
	{
		mStatePatternFTAlly.navMeshAgent.destination = mStatePatternFTAlly.transform.position;
		mStatePatternFTAlly.navMeshAgent.updateRotation = false;
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

	}

	public void ToSprinting()
	{

	}
}

