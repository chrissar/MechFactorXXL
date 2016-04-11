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
            new MoveFireTeamCommand(targetLocation).execute(selectedObject.GetComponent<FireTeam>());
        }
    }
}
