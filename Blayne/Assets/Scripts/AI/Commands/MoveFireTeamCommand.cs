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
			// If the fire team ally is the leader, set the destination of the leader's
			// fire team to the move target.
			fireTeamAlly.fireTeam.destination = mMoveTarget;
		}
	}
}

