using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class SideAI : MonoBehaviour
{
    private List<FireTeam> mFriendlySquads;
    private List<FireTeam> mEnemySquads;

    public void Start()
    {
        mFriendlySquads = new List<FireTeam>();
        mEnemySquads = new List<FireTeam>();
        GameObject[] squadObjs = GameObject.FindGameObjectsWithTag("squad");

        foreach(GameObject squadObj in squadObjs)
        {
            FireTeam squad = squadObj.GetComponent<FireTeam>();
            if(squad.TeamSide == FireTeam.Side.Friend)
            {
                mFriendlySquads.Add(squad);
            }
            else
            {
                mEnemySquads.Add(squad);
            }
        }
    }
    public void Update()
    {
        ProcessTeamAI(mFriendlySquads);
        ProcessTeamAI(mEnemySquads);
    }
    private void ProcessTeamAI(List<FireTeam> team)
    {
        List<FireTeam> deadSquads = new List<FireTeam>();
        foreach(FireTeam squad in team)
        {
            if(IsSquadDead(squad))
            {
                deadSquads.Add(squad);
            }
            else if(IsSquadRetreating(squad)
                && IsSquadEngaged(squad))
            {
                RequestHelp(squad, team);
            }
            else if(IsSquadInactive(squad))
            {
                Attack(squad, GetRandomEnemySquad(team));
            }
        }
    }

    private bool IsSquadStrong(FireTeam squad)
    {
        return squad.NonLeaderMemberCount > 2;
    }
    private bool IsSquadRetreating(FireTeam squad)
    {
        return squad.NonLeaderMemberCount <= 2;
    }
    private bool IsSquadEngaged(FireTeam squad)
    {
        return squad.EngagedEnemyTeams.Count > 0;
    }
    private bool IsSquadInactive(FireTeam squad)
    {
        return squad.EnemyTeamToAttack;
    }
    private bool IsSquadDead(FireTeam squad)
    {
        return squad.NonLeaderMemberCount == 0;
    }
    private void Attack(FireTeam friend, FireTeam enemy)
    {
        if (friend && enemy)
        {
            new TeamAttackEnemyCommand(enemy).execute(friend);
        }
    }
    private void RequestHelp(FireTeam squad, List<FireTeam> team)
    {
        foreach(FireTeam friendlySquad in team)
        {
            if(IsSquadStrong(friendlySquad))
            {
                Attack(friendlySquad, GetEngagedEnemy(squad)); 
            }
        }
    }
    private FireTeam GetEngagedEnemy(FireTeam squad)
    {
        return squad.EngagedEnemyTeams[0];
    }
    private List<FireTeam> GetEnemyTeam(List<FireTeam> team)
    {
        return team == mFriendlySquads ? mEnemySquads : mFriendlySquads;
    }
    private FireTeam GetRandomEnemySquad(List<FireTeam> team)
    {
        List<FireTeam> enemyTeam = GetEnemyTeam(team);
        int squadId = ((int)(UnityEngine.Random.value * 100.0f)) % enemyTeam.Count;
        return enemyTeam[squadId];
    }
}
