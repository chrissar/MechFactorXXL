using System;
using UnityEngine;

public class MoveFireTeamCommand : Command
{
	private Vector3 mMoveTarget;

	public MoveFireTeamCommand ()
	{
		mMoveTarget = Vector3.zero;
	}

	public MoveFireTeamCommand (Vector3 moveTarget)
	{
		mMoveTarget = moveTarget;
	}

	public override void execute(Ally ally)
	{
		if (ally is FireTeamAlly) 
		{
			FireTeamAlly fireTeamAlly = (FireTeamAlly)ally;
			// Set the destination of the ally's fire team.
			fireTeamAlly.fireTeam.setDestination(mMoveTarget);
		}
	}
}

