using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class SpawnPoint : MonoBehaviour
{
    public FireTeam fireTeamPrefab;
    public FireTeamAlly allyPrefab;
    public FireTeam.Side teamSide;
    public int teamNumber;
    public void Awake()
    {
        if (!fireTeamPrefab) throw new UnityException("Fire Team Prefab is null at a Spawn Point");
        if (!allyPrefab) throw new UnityException("Ally Prefab is null at a Spawn Point");

        GameObject fireTeamObject = Instantiate(fireTeamPrefab.gameObject);
        fireTeamObject.transform.position = gameObject.transform.position;
        FireTeam team = fireTeamObject.GetComponent<FireTeam>();
        team.TeamSide = teamSide;
        team.teamNumber = teamNumber;

        for(int i=0;i<FireTeam.kMaxFireTeamMembers;i++)
        {
            GameObject allyObj = Instantiate(allyPrefab.gameObject);
            allyObj.transform.position = team.GetSlotPosition(i);
            FireTeamAlly ally = allyObj.GetComponent<FireTeamAlly>();
			if (team.GetAllyAtSlotPosition (0) == null) {
				ally.fireTeamRole = FireTeamRole.LEADER;
			}
            team.AddFireTeamAlly(ally);
            ally.StateMachine.currentMovementState.ToMoving();
        }

		team.SetFormation(FireTeamFormation.WEDGE);
		team.SetDestination (fireTeamObject.transform.position);
    }
}

