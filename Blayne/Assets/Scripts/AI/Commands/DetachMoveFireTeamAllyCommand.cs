using System;
using UnityEngine;

public class DetachMoveFireTeamAllyCommand : Command
{
	private Vector3 mDetachMoveTarget;

	public DetachMoveFireTeamAllyCommand ()
	{
		mDetachMoveTarget = Vector3.zero;
	}

	public DetachMoveFireTeamAllyCommand(Vector3 detachMoveTarget)
	{
		mDetachMoveTarget = detachMoveTarget;
	}

	public override void execute(Ally ally)
	{
		if (ally != null && ally is FireTeamAlly) 
		{
			FireTeamAlly fireTeamAlly = ally as FireTeamAlly;
			// Set the destination of the ally's fire team.
			fireTeamAlly.SetDetachDestination(mDetachMoveTarget);
		}
	}
}

