using UnityEngine;
using System.Collections;

public static class FireTeamHelper
{
    public static FireTeamAlly GetClosestTeamMemberInFireTeam(FireTeamAlly fireTeam, FireTeam enemyTeam)
    {
        FireTeamAlly closestTeamMember = null;
        float closestDistance = -1.0f;
        // Find the member of the fire team that is closest to the state machine's fire team ally.
        for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i)
        {
            FireTeamAlly teamMember = enemyTeam.GetAllyAtSlotPosition(i);
            if (teamMember != null)
            {
                float distance = Vector3.Distance(fireTeam.Position, teamMember.Position);
                if (closestDistance < 0.0f || distance < closestDistance)
                {
                    // Set the member as the closest team member found so far.
                    closestTeamMember = teamMember;
                    closestDistance = distance;
                }
            }
        }
        return closestTeamMember;
    }

    public static void RotateToFaceTarget(FireTeamAlly fireTeam, Vector3 targetPoint)
    {
        Vector3 targetDirection = targetPoint - fireTeam.Position;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
        // Use Interpolated rotation, but restrict rotation to y axis.
        Quaternion newOrientation =
            Quaternion.Slerp(fireTeam.transform.rotation, targetRotation, Time.deltaTime * 5);
        newOrientation = Quaternion.Euler(new Vector3(0, newOrientation.eulerAngles.y, 0));

        fireTeam.transform.rotation = newOrientation;
    }
}
