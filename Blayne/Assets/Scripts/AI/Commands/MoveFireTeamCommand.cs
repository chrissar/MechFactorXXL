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
		if (ally != null && ally is FireTeam) {
			FireTeam fireTeam = ally as FireTeam;
			// Set the destination of the ally's fire team.
			fireTeam.SetDestination (mMoveTarget);
			fireTeam.EnemyTeamToPursue = null; // clear enemy to attack.

			// Set the fire team members to the move state.
			for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
				FireTeamAlly allyAtSlot = fireTeam.GetAllyAtSlotPosition (i);
				if (allyAtSlot != null) {
					allyAtSlot.StateMachine.currentMovementState.ToMoving ();
				} 
			}
		} 
	}
}

