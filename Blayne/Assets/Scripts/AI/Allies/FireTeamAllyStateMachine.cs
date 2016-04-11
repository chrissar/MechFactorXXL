using System;


public class FireTeamAllyStateMachine
{

	public IMovement currentMovementState;
	public readonly FireTeamAllyMovingState movingState;
	public readonly FireTeamAllyIdlingState idlingState;
	public IStatus currentStatusState;
	public readonly FireTeamAllyEaseState atEaseState;
	public readonly FireTeamAllySuppressedState suppressedState;
	public ICombat currentCombatState;
	public readonly FireTeamAllyAimingState aimingState;
	public readonly FireTeamAllyIdlingCombatState idlingCombatState;
	public readonly FireTeamAlly mStatePatternFTAlly;

	public FireTeamAllyStateMachine (FireTeamAlly fireTeamAlly)
	{
		mStatePatternFTAlly = fireTeamAlly;

		// Initialize states.
		movingState = new FireTeamAllyMovingState (mStatePatternFTAlly, this);
		idlingState = new FireTeamAllyIdlingState (mStatePatternFTAlly, this);
		atEaseState = new FireTeamAllyEaseState (mStatePatternFTAlly, this);
		suppressedState = new FireTeamAllySuppressedState (mStatePatternFTAlly, this);
		aimingState = new FireTeamAllyAimingState(mStatePatternFTAlly, this);
		idlingCombatState = new FireTeamAllyIdlingCombatState(mStatePatternFTAlly, this);

		// Set default states.
		currentMovementState = idlingState;
		currentStatusState = atEaseState;
		currentCombatState = idlingCombatState;
	}

	public void UpdateStates()
	{
		// Update movement.
		currentMovementState.UpdateState();
		// Update status.
		currentStatusState.UpdateState();
		// Update combat stance.
		currentCombatState.UpdateState();
	}
}

