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
            // Set the destination of the ally's fire team.
            fireTeam.SetDestination(targetLocation);
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
