using System;
using UnityEngine;

public class FireTeamFormationCommand : Command
{
	public FireTeamFormation mFireTeamFormation;

	public  FireTeamFormationCommand ()
	{
		mFireTeamFormation = FireTeamFormation.WEDGE;
	}

	public  FireTeamFormationCommand (FireTeamFormation formation)
	{
		mFireTeamFormation = formation;
	}

	public override void execute(Ally ally)
	{
		if (ally is FireTeamAlly) 
		{
			FireTeamAlly fireTeamAlly = (FireTeamAlly)ally;
			FireTeam fireTeam = fireTeamAlly.fireTeam;

			// Set the formation of the ally's fire team to the wedge formation.
			fireTeamAlly.fireTeam.SetFormation(mFireTeamFormation);
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

