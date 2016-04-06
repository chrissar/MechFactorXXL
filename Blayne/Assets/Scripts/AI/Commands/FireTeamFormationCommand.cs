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
			// Set the formation of the ally's fire team to the wedge formation.
			fireTeamAlly.fireTeam.SetFormation(mFireTeamFormation);
		}
	}
}

