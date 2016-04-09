using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamList
{
	public Dictionary<int, FireTeam> teamList;

	public TeamList ()
	{
		teamList = new Dictionary<int, FireTeam> ();
	}

	public void AddTeamToListWithNumber(FireTeam teamToAdd, int teamNumber)
	{
		if (teamToAdd != null && teamNumber >= 0) {
			teamList.Add (teamNumber, teamToAdd);
		} 
	}

	public FireTeam getTeamWithNumber(int teamNumber)
	{
		FireTeam fireTeam = null;
		teamList.TryGetValue (teamNumber, out fireTeam);
		return fireTeam;
	}
}

