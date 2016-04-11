using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamDisengageCommand : DisengageCommand
{
    public override void execute(Ally ally)
    {
        if (ally && ally is FireTeam)
        {
            var team = ally as FireTeam;
            var allies = team.GetAllMembers();
            foreach(FireTeamAlly nextAlly in allies)
            {
                base.execute(nextAlly);
            }
        }
    }
}
