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
			FireTeam fireTeam = fireTeamAlly.fireTeam;
			if (fireTeam != null) {
				// Set the destination of the ally's fire team.
				fireTeam.SetDestination (mMoveTarget);

				// Set the fire team members to the move state.
				for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
					FireTeamAlly allyAtSlot = fireTeam.GetAllyAtSlotPosition (i);
					if (allyAtSlot != null) {
						allyAtSlot.currentMovementState = allyAtSlot.movingState;
						allyAtSlot.OnEnterMovementState ();
					}
				}
			}
		}
	}
}

