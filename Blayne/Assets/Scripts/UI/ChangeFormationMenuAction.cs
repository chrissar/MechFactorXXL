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

            // Set the formation of the ally's fire team to the wedge formation.
            fireTeam.SetFormation(formation);
            // Set the fire team members to the move state.
            for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i)
            {
                FireTeamAlly allyAtSlot = fireTeam.GetAllyAtSlotPosition(i);
                if (allyAtSlot != null)
                {
                    allyAtSlot.StateMachine.currentMovementState.ToMoving();
                }
            }
        }
    }
}
