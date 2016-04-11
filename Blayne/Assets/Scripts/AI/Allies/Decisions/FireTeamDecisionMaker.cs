using System;
using System.Collections.Generic;
using UnityEngine;

public class FireTeamDecisionMaker : MonoBehaviour
{
	private const float mkMaxCoverPointDistance = 20.0f;
	private const float mkMaxSupportFireTeamDistance = 30.0f;
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
				// Set the enemy team as the current target to fire at.
				FireTeam engagedEnemyFireTeam = FireAtEnemy ();
				// If enemy fire team is stronger, try to find cover.
				if (IsOverpoweredByEnemy()) {
					// Find the best cover point to take if there is one.
					GameObject coverPoint = GetBestCoverPoint ();
					if (coverPoint != null) {
						// Move to the cover point, assuming cover formation.
						MoveToPointWithFormation (coverPoint.transform.position, 
							FireTeamFormation.COVER);
					} else {
						// Cannot take cover, so attack the enemy team.
						AttackEnemyTeam (engagedEnemyFireTeam);
					}
				} else {
					// Attempt to destroy the enemy team.
					AttackEnemyTeam (engagedEnemyFireTeam);
				}
			} else {
				// Disengage Enemies.
				DisengageEnemy();
				// If the team is in critial condition, move 
				// the team to the closest team base.
				if (IsTeamInCritialCondition ()) {
					Vector3 closestTeamBasePosition = GetClosestTeamBase ();
					if (closestTeamBasePosition != Vector3.zero) {
						// Move to the team base, assuming file formation.
						MoveToPointWithFormation (closestTeamBasePosition,
							FireTeamFormation.WEDGE);
					}
				}	
			}
		}
	}

	private void UpdateEngagedEnemiesList()
	{
		RemoveDestroyedEnemyFireTeams ();
		// Only check for visible enemies again after reaching the appropriate position.
		if(fireTeam.IsFireTeamInPosition()){
			// If none of the members of the fire team see any enemies, clear the 
			// list of engaged enemies. Note that allies will face enemies they have 
			// engaged while within sight range of the enemy.
			FireTeamAlly ally = IsNoEnemyInSight ();
			if (ally == null) {
				fireTeam.EngagedEnemyTeams.Clear ();
			}
		}
	}

	private void RemoveDestroyedEnemyFireTeams()
	{
		for (int i = fireTeam.EngagedEnemyTeams.Count - 1; i >= 0; --i) {
			if (fireTeam.EngagedEnemyTeams [i] == null ||
				fireTeam.EngagedEnemyTeams [i].MemberCount == 0) {
				fireTeam.EngagedEnemyTeams.RemoveAt (i);
			}
		}
	}

	private FireTeamAlly IsNoEnemyInSight()
	{
		// Check if there are any visible enemies around the squad.
		for (int i = 0; i < FireTeam.kMaxFireTeamMembers; ++i) {
			FireTeamAlly ally = fireTeam.GetAllyAtSlotPosition (i);
			if(ally != null){
				if (ally.IsAnyEnemyVisible ()) {
					return ally; 
				}
			}
		}
		return null;
	}

	private void MoveToPointWithFormation(Vector3 point, FireTeamFormation formation)
	{
		// Set the destination of the fire team to the cover point,
		// while setting them to the cover formation.
		ChangeFireTeamFormationCommand formationCommand =
			new ChangeFireTeamFormationCommand (formation);
		fireTeam.executeCommand (formationCommand);
		MoveFireTeamCommand moveCommand = new MoveFireTeamCommand (point);
		fireTeam.executeCommand (moveCommand);
	}

	private void AttackEnemyTeam(FireTeam enemyFireTeam)
	{
		fireTeam.EnemyTeamToAttack = enemyFireTeam;
	}

	private void DisengageEnemy()
	{
		// Order the team to stop engaging any enemies.
		fireTeam.EnemyTeamToAttack = null;
		DisengageCommand disengageCommand = new DisengageCommand ();
		IssueCommandToEntireTeam (disengageCommand);
		// Return to wedge formation.
		new ChangeFireTeamFormationCommand(FireTeamFormation.WEDGE).execute(fireTeam);
	}
		
	private bool IsOverpoweredByEnemy()
	{
		// Count number of allies
		int numberOfSupportAllies = fireTeam.MemberCount; // Count own fire team.
		foreach (FireTeam alliedFireTeam in fireTeam.alliedFireTeams) {
			// Check if the ally fire team is close enough to this fire team 
			// (and that it is not this fire team). If so, consider it a support ally.
			if(alliedFireTeam != fireTeam && 
				Vector3.Distance(alliedFireTeam.CurrentAnchorPosition,
					fireTeam.CurrentAnchorPosition) < mkMaxSupportFireTeamDistance){
				numberOfSupportAllies += alliedFireTeam.MemberCount;
			}
		}
		// Count number of enemeis
		int numberOfEnemies = 0;
		foreach (FireTeam enemyFireTeam in fireTeam.EngagedEnemyTeams) {
			numberOfEnemies += enemyFireTeam.MemberCount;
		}
		// For now, assume the enemy is stronger if the fire team is engaging more enemies 
		// than there are surrounding allies.
		if (numberOfSupportAllies < numberOfEnemies) {
			return true;
		}
		return false;
	}

	private bool IsTeamInCritialCondition()
	{
		// If the number of fire team allies in the fire team is half or less of its
		// maximum team size, consider the team to be in critical condition.
		if (fireTeam.MemberCount <= FireTeam.kMaxFireTeamMembers) {
			return true;
		} 
		return false;
	}

	private void SetFireTeamDestination(Vector3 destination)
	{
		// Set the destination of the team to the cover point 
		// and the set cover slot positions for the team.
		fireTeam.SetDestination (destination);
	}

	// Returns enemy team that was chosen to fire upon.
	private FireTeam FireAtEnemy()
	{
		// Order the team to fire upon the closest enemy team. This sets the target enemy team
		// of the fire team members to the closest enemy team.
		FireTeam fireTeamToAttack = GetClosestEnemyFireTeam();
		if (fireTeamToAttack != null) {
			AttackEnemyCommand attackEnemyCommand = new AttackEnemyCommand (fireTeamToAttack);
			IssueCommandToEntireTeam (attackEnemyCommand);
		}
		return fireTeamToAttack;
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

	private Vector3 GetClosestTeamBase()
	{
		Vector3 closestTeamBasePosition = Vector3.zero;
		float closestTeamBaseDistance = -1.0f;
		foreach (TeamBase teamBase in fireTeam.TeamBases) {
			if (teamBase != null) {
				float distance = Vector3.Distance (teamBase.transform.position,
					fireTeam.CurrentAnchorPosition);
				if (closestTeamBaseDistance < 0 || distance < closestTeamBaseDistance) {
					// Set the team base position as the new closest team base position.
					closestTeamBasePosition = teamBase.transform.position;
					closestTeamBaseDistance = distance;
				}
			}
		}
		return closestTeamBasePosition;
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

	private FireTeam GetClosestEnemyFireTeam()
	{
		Vector3 teamPosition = fireTeam.CurrentAnchorPosition;
		FireTeam closestEnemyFireTeam = null;
		float closestDistance = -1.0f;

		// Find the enemy fire team being engaged that is closest to the leader's fire team.
		foreach (FireTeam enemyFireTeam in fireTeam.EngagedEnemyTeams) {
			if (enemyFireTeam != null) {
				Vector3 enemyFireTeamPosition = enemyFireTeam.CurrentAnchorPosition; 
				float distance = Vector3.Distance (teamPosition, enemyFireTeamPosition);

				if (closestDistance < 0 || distance < closestDistance) {
					// Set the fire team as the current closest fire team
					closestEnemyFireTeam = enemyFireTeam;
					closestDistance = distance;
				}
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

