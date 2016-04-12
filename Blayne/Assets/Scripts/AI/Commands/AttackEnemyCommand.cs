using System;

public class AttackEnemyCommand : Command
{
	protected FireTeam mFireTeamToAttack;

	public AttackEnemyCommand (FireTeam fireTeamToAttack)
	{
		mFireTeamToAttack = fireTeamToAttack;
	}

	public override void execute(Ally ally)
	{
		if (ally && ally is FireTeamAlly) 
		{
			FireTeamAlly fireTeamAlly = ally as FireTeamAlly;
            // Set the target fire team of the fire team ally.
            fireTeamAlly.targetEnemyTeam = mFireTeamToAttack;
		}
	}
}

