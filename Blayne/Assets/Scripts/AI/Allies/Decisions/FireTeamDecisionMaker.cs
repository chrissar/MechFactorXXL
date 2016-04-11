using System;
using System.Collections.Generic;
using UnityEngine;

public class FireTeamDecisionMaker : MonoBehaviour
{
	private const float mkMaxCoverPointDistance = 20.0f;
	private const float mkCoverMinimumDistanceFromEnemy = 15.0f;

	public FireTeam fireTeam;
	private int mNumberOfEngagedEnemies;

    private GameObject mBestCoverPoint = null;
    public bool CoverExists { get { return mBestCoverPoint != null; } }

	public void Start()
	{
		mNumberOfEngagedEnemies = 0;
	}

	public void Update()
	{
		if (fireTeam == null) {
			return;
		}

		// Update list of engaged enemies.
		UpdateEngagedEnemiesList();

		// Check if the number of engaged enemies has increased.
		if (mNumberOfEngagedEnemies != fireTeam.EngagedEnemyTeams.Count) {
			mNumberOfEngagedEnemies = fireTeam.EngagedEnemyTeams.Count;
			if (mNumberOfEngagedEnemies > 0) {
				// Find the best cover point to take if there is one.
				GameObject coverPoint = GetBestCoverPoint ();
				if (coverPoint != null) {
					// Move to cover point.
					fireTeam.SetFormation (FireTeamFormation.COVER);
					SetFireTeamDestination (coverPoint.transform.position);
				} else {
					// Stand ground by setting the fire team destination to the 
					// fire team leader's position.
					SetFireTeamDestination (fireTeam.GetAllyAtSlotPosition (0).Position);
				}
				// Engage enemies.
				AttackEnemy ();
			} else {
				// Disengage Enemies.
				DisengageEnemy();
			}
		}
	}

	private void UpdateEngagedEnemiesList()
	{
		// If none of the members of the fire team see any enemies, clear the 
		// list of engaged enemies. Note that allies will face enemies they have 
		// engaged while within sight range of the enemy.
		FireTeamAlly ally = IsNoEnemyInSight ();
		if (ally == null) {
			fireTeam.EngagedEnemyTeams.Clear ();
		}
	}

	private FireTeamAlly IsNoEnemyInSight()
	{
		// Check if there are any visible enemies around the squad.
		for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
			FireTeamAlly ally = fireTeam.GetAllyAtSlotPosition (i);
			if(ally != null){
				if (ally.IsEnemyVisible ()) {
					return ally; 
				}
			}
		}
		return null;
	}

	private bool IsOverpoweredByEnemy()
	{
		// This is currently stubbed to always return true.
		return true;
	}

	private void SetFireTeamDestination(Vector3 destination)
	{
		// Set the destination of the team to the cover point 
		// and the set cover slot positions for the team.
		fireTeam.SetDestination (destination);
	}

	private void AttackEnemy()
	{
		// Order the team to attack the closest enemy team.
		FireTeam fireTeamToAttack = GetClosestEnemyFireTeam();
		if (fireTeamToAttack != null) {
			AttackEnemyCommand attackEnemyCommand = new AttackEnemyCommand (fireTeamToAttack);
			IssueCommandToEntireTeam (attackEnemyCommand);
		}
	}

	private void DisengageEnemy()
	{
		// Order the team to attack the closest enemy team.
		DisengageCommand disengageCommand = new DisengageCommand ();
		IssueCommandToEntireTeam (disengageCommand);
	}
		
	// This should be modified to return a cover point using more sophisticated methods.
	private GameObject GetBestCoverPoint()
	{
		// Get candidate cover points, which are the cover points sufficiently close to
		// the team (a cover map can be used here).
		List<GameObject> candidateCoverPoints = GetCandidateCoverPoints ();

		Vector3 teamPosition = fireTeam.CurrentAnchorPosition;
		GameObject validCoverPointFarthestFromEnemies = null;
		float farthestDistance = -1.0f;
		// Of the candidate cover points, find the one that is farthest from 
		// all enemies.
		foreach (GameObject candidateCoverPoint in candidateCoverPoints) {
			Vector3 coverPointPosition  = candidateCoverPoint.transform.position;
			// Check if the cover point is sufficiently far from the engaged enemies.
			if (IsCoverPointFarEnoughFromEnemies (coverPointPosition)) {
				float closestEnemyDistance = closestEnemyDistanceFromPoint (coverPointPosition);
				if (farthestDistance < 0.0f || closestEnemyDistance > farthestDistance) {
					// Set the new farthest distance from enemies cover point.
					validCoverPointFarthestFromEnemies = candidateCoverPoint;
					farthestDistance = closestEnemyDistance;
				}
			}
		}
        mBestCoverPoint = validCoverPointFarthestFromEnemies;
		return validCoverPointFarthestFromEnemies;
	}

	private List<GameObject> GetCandidateCoverPoints()
	{
		GameObject[] coverPoints = GameObject.FindGameObjectsWithTag("Cover");
		List<GameObject> candidateCoverPoints = new List<GameObject> ();
		foreach (GameObject coverPoint in coverPoints) {
			// If the cover point is close enough to the fire team, add it to
			// the list of candidates.
			if (Vector3.Distance (coverPoint.transform.position, 
				fireTeam.CurrentAnchorPosition) < mkMaxCoverPointDistance) {
				candidateCoverPoints.Add (coverPoint);
			}
		}
		return candidateCoverPoints;
	}

	private bool IsCoverPointFarEnoughFromEnemies(Vector3 coverPointPosition)
	{
		foreach (FireTeam enemyFireTeam in fireTeam.EngagedEnemyTeams) {
			// The cover point is considered too close to enemies if below the minumum
			// distance from the enemy or if the enemy fire team is closer to the cover
			// point than the fire team of the leader.
			float distanceFromEnemy =
				Vector3.Distance (coverPointPosition, enemyFireTeam.CurrentAnchorPosition);
			if(distanceFromEnemy < mkCoverMinimumDistanceFromEnemy)
			{
				return false;
			}
		}
		return true;
	}

	private float closestEnemyDistanceFromPoint(Vector3 point){
		float closestEnemyDistance = -1.0f;
		foreach (FireTeam enemyFireTeam in fireTeam.EngagedEnemyTeams) {
			float distance = Vector3.Distance (point, enemyFireTeam.CurrentAnchorPosition);
			if(closestEnemyDistance < 0 || distance <closestEnemyDistance)
			{
				// Set the closest distance found so far.
				closestEnemyDistance = distance;
			}
		}
		return closestEnemyDistance;
	}

	private FireTeam GetClosestEnemyFireTeam(){
		Vector3 teamPosition = fireTeam.CurrentAnchorPosition;
		FireTeam closestEnemyFireTeam = null;
		float closestDistance = -1.0f;

		// Find the enemy fire team being engaged that is closest to the leader's fire team.
		foreach (FireTeam enemyFireTeam in fireTeam.EngagedEnemyTeams) {
			Vector3 enemyFireTeamPosition = enemyFireTeam.CurrentAnchorPosition; 
			float distance = Vector3.Distance (teamPosition, enemyFireTeamPosition);

			if (closestDistance < 0 || distance < closestDistance) {
				// Set the fire team as the current closest fire team
				closestEnemyFireTeam = enemyFireTeam;
				closestDistance = distance;
			}
		}
		return closestEnemyFireTeam;
	}

	private void IssueCommandToEntireTeam(Command command)
	{
		for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
			FireTeamAlly ally = fireTeam.GetAllyAtSlotPosition (i);
			if (ally != null) {
				ally.executeCommand (command);
			}
		}
	}
}

