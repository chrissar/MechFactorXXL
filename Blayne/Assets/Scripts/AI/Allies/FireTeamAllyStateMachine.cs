using System;


public class FireTeamAllyStateMachine
{

	public IMovement currentMovementState;
	public readonly FireTeamAllyMovingState movingState;
	public readonly FireTeamAllyIdlingState idlingState;
	public IStatus currentStatusState;
	public readonly FireTeamAllyEaseState atEaseState;
	public readonly FireTeamAllySuppressedState suppressedState;
	public readonly FireTeamAlly mStatePatternFTAlly;

	public FireTeamAllyStateMachine (FireTeamAlly fireTeamAlly)
	{
		mStatePatternFTAlly = fireTeamAlly;

		// Initialize states.
		movingState = new FireTeamAllyMovingState (mStatePatternFTAlly, this);
		idlingState = new FireTeamAllyIdlingState (mStatePatternFTAlly, this);
		atEaseState = new FireTeamAllyEaseState (mStatePatternFTAlly, this);
		suppressedState = new FireTeamAllySuppressedState (mStatePatternFTAlly, this);
		currentMovementState = idlingState;
		currentStatusState = atEaseState;
	}

	public void UpdateStates()
	{
		// Update movement.
		currentMovementState.UpdateState();
		// Update status
		currentStatusState.UpdateState();
	}
}

