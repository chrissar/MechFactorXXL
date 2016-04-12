using System;
using UnityEngine;

public class DisengageCommand : Command
{
	public override void execute(Ally ally)
	{
		if (ally != null && ally is FireTeamAlly) 
		{
			FireTeamAlly fireTeamAlly = ally as FireTeamAlly;

			// Set the target fire team of the fire team ally.
			fireTeamAlly.targetEnemyTeam = null;
			fireTeamAlly.StateMachine.firingState.ToIdling ();
		}
	}
}

