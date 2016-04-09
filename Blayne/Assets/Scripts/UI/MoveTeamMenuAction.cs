using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace MenuActions
{
    public class MoveTeamMenuAction : MenuAction
    {
        public override SpawnMenu.ActionTarget GetSelectedType()
        {
            return SpawnMenu.ActionTarget.FriendGroup;
        }

        public override SpawnMenu.ActionTarget GetTargetType()
        {
            return SpawnMenu.ActionTarget.Anything;
        }

        protected override void Execute()
        {
            Debug.Log(selectedObject.name);
            FireTeam fireTeam = selectedObject.GetComponent<FireTeam>();
			// Create movement command to issue to the fire team.
			MoveFireTeamCommand movementCommand = new MoveFireTeamCommand (targetLocation);
			fireTeam.executeCommand(movementCommand);
        }
    }
}
