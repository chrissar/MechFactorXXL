using System;
using System.Collections.Generic;
using UnityEngine;

public class FireTeamLeaderActionHelper
{
	private const float mkMaxCoverPointDistance = 20.0f;
	private const float mkCoverMinimumDistanceFromEnemy = 15.0f;

	private FireTeamAlly mFireTeamLeader;

	public FireTeamLeaderActionHelper (FireTeamAlly fireTeamLeader)
	{
		mFireTeamLeader = fireTeamLeader;
	}

	public void AddTeamOfEnemyToEngagedList(FireTeamAlly enemy)
	{
		mFireTeamLeader.fireTeam.EngagedEnemyTeams.Add(enemy.fireTeam);
	}

	public void MoveTeamToCoverForEnemy()
	{
		// Find the most appropriate cover point.
		GameObject coverPoint = GetBestCoverPoint();
		if (coverPoint != null) {
			// If an appropriate cover point was found, set the destination of the team 
			// to the cover point and the set cover slot positions for the team.
			mFireTeamLeader.fireTeam.SetDestination (coverPoint.transform.position);
			mFireTeamLeader.fireTeam.SetFormation (FireTeamFormation.COVER);
		} else {
			// Until attack states are implemented, set the destination of the team
			// to the fire team leader's position.
			mFireTeamLeader.fireTeam.SetDestination (mFireTeamLeader.Position);
		}

		// Have the team assume a suppressed status.
		for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
			FireTeamAlly allyAtSlot = mFireTeamLeader.fireTeam.GetAllyAtSlotPosition (i);
			if (allyAtSlot != null) {
				allyAtSlot.StateMachine.currentStatusState.ToSuppressed ();
			}
		}
	}

	// This should be modified to return a cover point using more sophisticated methods.
	private GameObject GetBestCoverPoint()
	{
		// Get candidate cover points, which are the cover points sufficiently close to
		// the team (a cover map can be used here).
		List<GameObject> candidateCoverPoints = getCandidateCoverPoints ();

		Vector3 teamPosition = mFireTeamLeader.fireTeam.CurrentAnchorPosition;
		GameObject closestValidCoverPoint = null;
		float closestDistance = -1.0f;
		// Of the candidate cover points, find the one that is closest to the 
		// fire team that is also sufficiently far from the engaged enemies.
		foreach (GameObject candidateCoverPoint in candidateCoverPoints) {
			// Check if the cover point is sufficiently far from the
			// engaged enemies.
			Vector3 coverPointPosition = candidateCoverPoint.transform.position; 
			float distanceFromFireTeam = Vector3.Distance (teamPosition, coverPointPosition);
			if (IsCoverPointFarEnoughFromEnemies (coverPointPosition, distanceFromFireTeam)) {
				// Check if cover point is the closest cover point 
				// to the team found so far.
				if (closestDistance < 0 || distanceFromFireTeam < closestDistance) {
					// Set the cover point as the current best cover point.
					closestValidCoverPoint = candidateCoverPoint;
					closestDistance = distanceFromFireTeam;
				}
			}
		}
		return closestValidCoverPoint;
	}

	private List<GameObject> getCandidateCoverPoints()
	{
		GameObject[] coverPoints = GameObject.FindGameObjectsWithTag("Cover");
		List<GameObject> candidateCoverPoints = new List<GameObject> ();
		foreach (GameObject coverPoint in coverPoints) {
			// If the cover point is close enough to the fire team, add it to
			// the list of candidates.
			if (Vector3.Distance (coverPoint.transform.position, 
				mFireTeamLeader.fireTeam.CurrentAnchorPosition) < mkMaxCoverPointDistance) {
				candidateCoverPoints.Add (coverPoint);
			}
		}
		return candidateCoverPoints;
	}

	private bool IsCoverPointFarEnoughFromEnemies(Vector3 coverPointPosition, 
		float distanceFromLeaderFireTeam)
	{
		foreach (FireTeam enemyFireTeam in mFireTeamLeader.fireTeam.EngagedEnemyTeams) {
			// The cover point is considered too close to enemies if below the minumum
			// distance from the enemy or if the enemy fire team is closer to the cover
			// point than the fire team of the leader.
			float distanceFromEnemy =
				Vector3.Distance (coverPointPosition, enemyFireTeam.CurrentAnchorPosition);
			if(distanceFromEnemy < mkCoverMinimumDistanceFromEnemy || 
				distanceFromEnemy < distanceFromLeaderFireTeam)
			{
				return false;
			}
		}
		return true;
	}

}

