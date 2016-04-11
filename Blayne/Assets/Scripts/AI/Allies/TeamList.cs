using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamList
{
	private Dictionary<int, FireTeam> mTeamList;

	public TeamList ()
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

	public void addTeamsWithSameAlignmentToTeam(FireTeam fireTeam)
	{
		List<FireTeam> fireTeamsOfGivenAlignment = new List<FireTeam>();
		foreach(KeyValuePair<int, FireTeam> teamEntry in mTeamList){
			if (teamEntry.Value.TeamSide == fireTeam.TeamSide) {
				fireTeam.alliedFireTeams.Add (teamEntry.Value);
			}
		}
	}
}

