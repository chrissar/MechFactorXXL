using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuActions
{
    public class ChangeFormationMenuAction : MenuAction
    {
        public FireTeamFormation formation = FireTeamFormation.WEDGE;

        public override SpawnMenu.ActionTarget GetSelectedType()
        {
            return SpawnMenu.ActionTarget.Anything;
        }
        public override SpawnMenu.ActionTarget GetTargetType()
        {
            return SpawnMenu.ActionTarget.FriendGroup;
        }
        protected override void Execute()
        {
            FireTeam fireTeam = targetObject.GetComponent<FireTeam>();
			// Create formation command to issue to the fire team.
			ChangeFireTeamFormationCommand formationCommand = new ChangeFireTeamFormationCommand(formation);
			fireTeam.executeCommand(formationCommand);
        }
    }
}
