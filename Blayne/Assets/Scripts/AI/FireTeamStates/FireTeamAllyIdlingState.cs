using System;

public class FireTeamAllyIdlingState : IMovement
{
	private readonly FireTeamAlly mStatePatternFTAlly;

	public FireTeamAllyIdlingState (FireTeamAlly statePatternFireTeamAlly){
		mStatePatternFTAlly = statePatternFireTeamAlly;
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

	}

	public void ToIdling()
	{

	}

	public void ToSprinting()
	{

	}
}

