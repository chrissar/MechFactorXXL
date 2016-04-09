using System;
using UnityEngine;

public class ChangeFireTeamFormationCommand : Command
{
	public FireTeamFormation mFireTeamFormation;

	public  ChangeFireTeamFormationCommand ()
	{
		mFireTeamFormation = FireTeamFormation.WEDGE;
	}

	public  ChangeFireTeamFormationCommand (FireTeamFormation formation)
	{
		mFireTeamFormation = formation;
	}

	public override void execute(Ally ally)
	{
		if (ally != null && ally is FireTeam) 
		{
			FireTeam fireTeam = ally as FireTeam;

			// Set the formation of the ally's fire team to the wedge formation.
			fireTeam.SetFormation(mFireTeamFormation);
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

