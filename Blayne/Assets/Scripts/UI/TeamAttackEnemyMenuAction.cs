using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuActions
{
    public class TeamAttackEnemyMenuAction : MenuAction
    {
        public override SpawnMenu.ActionTarget GetSelectedType()
        {
            return SpawnMenu.ActionTarget.FriendGroup;
        }

        public override SpawnMenu.ActionTarget GetTargetType()
        {
            return SpawnMenu.ActionTarget.EnemyGroup;
        }

        protected override void Execute()
        {
            new TeamAttackEnemyCommand(targetObject.GetComponent<FireTeam>()).execute(selectedObject.GetComponent<FireTeam>());
        }
    }
}
