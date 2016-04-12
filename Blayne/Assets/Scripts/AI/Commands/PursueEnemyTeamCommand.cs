using System;

public class PursueEnemyTeamCommand : Command
{
	private FireTeam mFireTeamToPursue;

	public PursueEnemyTeamCommand (FireTeam fireTeamToPursue)
	{
		mFireTeamToPursue = fireTeamToPursue;
	}
		
	public override void execute(Ally ally)
	{
		if (ally != null && ally is FireTeam) 
		{
			FireTeam fireTeam = ally as FireTeam;

			// Set the enemy team to pursue.
			fireTeam.EnemyTeamToPursue = mFireTeamToPursue;
		}
	}
}
