using System;
using UnityEngine;

public class FireTeamLeaderActionHelper
{
	private FireTeamAlly mFireTeamLeader;

	public FireTeamLeaderActionHelper (FireTeamAlly fireTeamLeader)
	{
		mFireTeamLeader = fireTeamLeader;
	}

	public void MoveTeamToCoverForEnemy(){
		// Find the most appropriate cover point.
		GameObject coverPoint = GetBestCoverPoint();
		// If an appropriate cover point was found, set the destination of the team 
		// to the cover point and the set cover slot positions for the team.
		mFireTeamLeader.fireTeam.SetDestination(coverPoint.transform.position);
		mFireTeamLeader.fireTeam.SetFormation (FireTeamFormation.COVER);

		// Have the team assume a suppressed status.
		for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
			FireTeamAlly allyAtSlot = mFireTeamLeader.fireTeam.GetAllyAtSlotPosition (i);
			if (allyAtSlot != null) {
				allyAtSlot.StateMachine.currentStatusState.ToSuppressed ();
			}
		}
	}

	// This should be modified to return a cover point using more sophisticated methods.
	private GameObject GetBestCoverPoint(){
		return GameObject.Find("Cover0");
	}
}

