using System;

public class FireTeamAllyMovingState : IMovement
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public FireTeamAllyMovingState (FireTeamAlly statePatternFTAlly, FireTeamAllyStateMachine stateMachine){
		mStatePatternFTAlly = statePatternFTAlly;
		mStateMachine = stateMachine;
	}

	public void UpdateState()
	{
		// Move to position marked by slot position.
		if (mStatePatternFTAlly.fireTeam != null) {
			mStatePatternFTAlly.navMeshAgent.destination = 
				mStatePatternFTAlly.fireTeam.GetSlotPosition (mStatePatternFTAlly.slotPosition);
		} else {
			ToIdling ();
		}
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
