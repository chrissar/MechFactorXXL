using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamList : MonoBehaviour
{
	private Dictionary<int, FireTeam> mTeamList;

	public void Awake()
	{
		mTeamList = new Dictionary<int, FireTeam> ();
	}

	public void AddTeamToListWithNumber(FireTeam teamToAdd, int teamNumber)
	{
		if (teamToAdd != null && teamNumber >= 0) {
			mTeamList.Add (teamNumber, teamToAdd);
		} 
	}

	public FireTeam getTeamWithNumber(int teamNumber)
	{
		FireTeam fireTeam = null;
		mTeamList.TryGetValue (teamNumber, out fireTeam);
		return fireTeam;
	}

	public List<FireTeam> GetTeamsWithGivenAlignment(FireTeam.Side teamSide)
	{
		List<FireTeam> fireTeamsOfGivenAlignment = new List<FireTeam>();
		foreach(KeyValuePair<int, FireTeam> teamEntry in mTeamList){
			if (teamEntry.Value.TeamSide == teamSide) {
				fireTeamsOfGivenAlignment.Add (teamEntry.Value);
			}
		}
		return fireTeamsOfGivenAlignment;
	}

	public void AddTeamsWithSameAlignmentToTeam(FireTeam fireTeam)
	{
		foreach(KeyValuePair<int, FireTeam> teamEntry in mTeamList){
			if (teamEntry.Value.TeamSide == fireTeam.TeamSide) {
				fireTeam.alliedFireTeams.Add (teamEntry.Value);
			}
		}
	}
}

