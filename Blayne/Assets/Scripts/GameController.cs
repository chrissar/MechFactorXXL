using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public bool topDownView = false;
    public int numTicketsPerTeam = 10;

    private SpawnMenu mSpawnMenu;
    private static GameController msInstance;

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
        mSpawnMenu.enabled = false;
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
        topDownView = (!topDownView);
    }
}
