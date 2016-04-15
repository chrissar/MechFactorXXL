using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    public bool topDownView = false;
    public int numTicketsPerTeam = 10;
    public event Action leftTopDownView;
    private SpawnMenu mSpawnMenu;
    private static GameController msInstance;
	private TeamList mTeamList;

    public static GameController Instance
    {
        get
        {
            if (msInstance == null)
            {
                msInstance = FindObjectOfType<GameController>();

                if (msInstance == null)
                {
                    GameObject controllerPrefab = (GameObject)Resources.Load("GameController");
                    GameObject controllerObject = (GameObject)Instantiate(controllerPrefab, controllerPrefab.transform.position, controllerPrefab.transform.rotation);
                    controllerObject.name = controllerPrefab.name;
                    msInstance = controllerObject.GetComponent<GameController>();
                }
            }
            return msInstance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        mSpawnMenu = SpawnMenu.Instance;
        if (mSpawnMenu != null)
        {
            mSpawnMenu.enabled = false;
        }

		// Get the team list.
		mTeamList = GameObject.Find ("TeamList").GetComponent<TeamList>() as TeamList;
		// Have the teams populate their list of enemies.
		if (mTeamList != null) 
		{
			mTeamList.SetEnemiesForAllTeams ();
		}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCamera();
            if (mSpawnMenu != null)
            {
                mSpawnMenu.Toggle();
            }
        }
    }

    private void ToggleCamera()
    {
        topDownView = !topDownView;
        if(!topDownView && leftTopDownView != null)
        {
            Debug.Log("Launching Left Top Down VIew Event!");
            leftTopDownView();
        }
    }
}
