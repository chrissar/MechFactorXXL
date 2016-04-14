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
            Debug.Log("Adding squad: " + squadObj.name);
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
            else if(IsSquadRetreating(squad))
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
        return squad.MemberCount > 2;
    }
    private bool IsSquadRetreating(FireTeam squad)
    {
        return squad.MemberCount <= 2;
    }
    private bool IsSquadEngaged(FireTeam squad)
    {
        return squad.EngagedEnemyTeams.Count > 0;
    }
    private bool IsSquadInactive(FireTeam squad)
    {
        return !squad.EnemyTeamToPursue;
    }
    private bool IsSquadDead(FireTeam squad)
    {
        return squad.MemberCount == 0;
    }
    private void Attack(FireTeam friend, FireTeam enemy)
    {
        if (friend && enemy)
        {
            
            new TeamAttackEnemyCommand(enemy).execute(friend);
        }
    }
    private void Approach(FireTeam friend, FireTeam enemy)
    {
        if (friend && enemy)
        {
            new MoveFireTeamCommand(enemy.transform.position).execute(friend);
        }
    }
    private void RequestHelp(FireTeam squad, List<FireTeam> team)
    {
        foreach(FireTeam friendlySquad in team)
        {
            if(IsSquadStrong(friendlySquad))
            {
                Attack(friendlySquad, GetAttackingSquad(squad)); 
            }
        }
    }
    private FireTeam GetAttackingSquad(FireTeam squad)
    {
        FireTeam result = null;
        List<FireTeam> enemyTeam = GetEnemyTeam(squad);
        foreach(FireTeam enemySquad in enemyTeam)
        {
            if(enemySquad.EnemyTeamToPursue == squad)
            {
                result = enemySquad;
                break;
            }
        }
        //Debug.Log("Requesting help for attacking squad: " + result.gameObject.name);
        return result;
    }
    private FireTeam GetEngagedEnemy(FireTeam squad)
    {
        return squad.EngagedEnemyTeams[0];
    }
    private List<FireTeam> GetEnemyTeam(List<FireTeam> team)
    {
        return team == mFriendlySquads ? mEnemySquads : mFriendlySquads;
    }
    private List<FireTeam> GetEnemyTeam(FireTeam squad)
    {
        return squad.TeamSide == FireTeam.Side.Friend ? mFriendlySquads : mEnemySquads;
    }
    private FireTeam GetRandomEnemySquad(List<FireTeam> team)
    {
        List<FireTeam> enemyTeam = GetEnemyTeam(team);
        int squadId = ((int)(UnityEngine.Random.value * 100.0f)) % enemyTeam.Count;
        return enemyTeam[squadId];
    }
}
