using System;

public class FireTeamAllySuppressedState : IStatus
{
	private readonly FireTeamAlly mStatePatternFTAlly;
	private FireTeamAllyStateMachine mStateMachine;

	public  FireTeamAllySuppressedState (FireTeamAlly statePatternFTAlly,
		FireTeamAllyStateMachine stateMachine){
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

	}
}

