using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class TeamAttackEnemyCommand : AttackEnemyCommand
{
    public TeamAttackEnemyCommand(FireTeam enemyTeam)
        : base(enemyTeam)
    {

    }
    public override void execute(Ally ally)
    {
        if(ally && ally is FireTeam)
        {
            
            FireTeam team = ally as FireTeam;
            var allies = team.GetAllMembers();
            team.EnemyTeamToPursue = mFireTeamToAttack;
            foreach(FireTeamAlly nextAlly in  allies)
            {
                base.execute(nextAlly);
            }
        }
    }
}
