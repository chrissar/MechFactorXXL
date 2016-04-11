using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamBase : MonoBehaviour
{
	public FireTeam.Side teamSide;
	public Color teamColor;
	public FireTeamAlly allyPrefab;
	private const float mkRefillRadius = 5.0f;
	private const float mkRefillTeamTimerLimit = 10.0f;

	private List<FireTeam> mFireTeamList;
	private bool mIsAbleToRefill;
	private float mRefillTeamTimerCount;

	public void Start()
	{
		mFireTeamList = new List<FireTeam> ();
		mIsAbleToRefill = true;
		mRefillTeamTimerCount = 0;
	}

	public void Update()
	{
		if (mIsAbleToRefill) {
			// If any member of the friendly fire teams is within vicinity, refill any
			// missign members of the fire team.
			foreach (FireTeam fireTeam in mFireTeamList) {
				// Only try to fill incomplete fire teams.
				int missingTeamMembers = FireTeam.kMaxFireTeamMembers - fireTeam.MemberCount;
				if (missingTeamMembers > 0) {
					for (int i = 0; i < fireTeam.MemberCount; ++i) {
						FireTeamAlly ally = fireTeam.GetAllyAtSlotPosition (i);
						if (Vector3.Distance (ally.Position, transform.position) < mkRefillRadius) {
							print ("Refilling team");
							// Refill the fire team to full.
							for (int j = 0; j < missingTeamMembers; ++j) {
								CreateNewFireTeamAllyForTeam (fireTeam);
							}
							// Don't try to refill more than one fire team at a time.
							mIsAbleToRefill = false;
							break;
						}
					}
				}
			}
		} else {
			mRefillTeamTimerCount += Time.deltaTime;
			if (mRefillTeamTimerCount >= mkRefillTeamTimerLimit) {
				// Allow refills and reset refill counter.
				mIsAbleToRefill = true;
				mRefillTeamTimerCount = 0.0f;
			}
		}
	}

	public void AddAlliedFireTeamToListOfFireTeams(FireTeam fireTeam)
	{
		if (fireTeam != null && fireTeam.TeamSide == teamSide) {
			mFireTeamList.Add (fireTeam);
		}
	}

	private void CreateNewFireTeamAllyForTeam(FireTeam fireTeam)
	{
		GameObject allyObj = Instantiate(allyPrefab.gameObject);
		FireTeamAlly ally = allyObj.GetComponent<FireTeamAlly>();
		fireTeam.AddFireTeamAlly(ally);
		allyObj.transform.position = fireTeam.GetSlotPosition(ally.slotPosition);
		ally.StateMachine.currentMovementState.ToMoving();
		SetTeamColorOfFireTeamAlly (ally);
	}

	private void SetTeamColorOfFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		Renderer renderer = fireTeamAlly.GetComponent<Renderer> ();
		if (renderer != null) {
			renderer.material.color = teamColor;
		}
	}
}
