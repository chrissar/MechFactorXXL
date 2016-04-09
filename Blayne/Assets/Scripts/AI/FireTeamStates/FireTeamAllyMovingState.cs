using System;

public class FireTeamAllyMovingState : IMovement
{
	private readonly FireTeamAlly mStatePatternFTAlly;

	public FireTeamAllyMovingState (FireTeamAlly statePatternFireTeamAlly){
		mStatePatternFTAlly = statePatternFireTeamAlly;
	}

	public void UpdateState()
	{
		// Move to position marked by slot position.
		if (mStatePatternFTAlly.fireTeam != null) {
			mStatePatternFTAlly.navMeshAgent.destination = 
				mStatePatternFTAlly.fireTeam.getSlotPosition (mStatePatternFTAlly.slotPosition);
		} else {
			mStatePatternFTAlly.navMeshAgent.destination = 
				mStatePatternFTAlly.transform.position;
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

	}

	public void ToSprinting()
	{

	}
}
