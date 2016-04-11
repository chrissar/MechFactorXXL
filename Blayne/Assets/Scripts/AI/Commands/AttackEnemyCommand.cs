using System;

public class AttackEnemyCommand : Command
{
	private FireTeam mFireTeamToAttack;

	public AttackEnemyCommand (FireTeam fireTeamToAttack)
	{
		mFireTeamToAttack = fireTeamToAttack;
	}

	public override void execute(Ally ally)
	{
		if (ally != null && ally is FireTeamAlly) 
		{
			FireTeamAlly fireTeamAlly = ally as FireTeamAlly;

			// Set the target fire team of the fire team ally.
			fireTeamAlly.targetEnemyTeam = mFireTeamToAttack;
		}
	}
}

