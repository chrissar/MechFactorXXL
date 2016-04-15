using UnityEngine;
using System.Collections;

public class IsClickable : MonoBehaviour {
    public int mapWidth;
    public int mapHeight;

    public Terrain terrain;
	// Use this for initialization
	void Start () {
        mapWidth = terrain.terrainData.heightmapWidth;
        mapHeight = terrain.terrainData.heightmapHeight;
	}
	
	// Update is called once per frame
	void Update () {    

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // x,y world coordinates.
            float x = 0; 
            float y = 0;
            if (Input.mousePosition.x < (mapWidth / 2))
                x = (Input.mousePosition.x - (mapWidth / 2)) + mapWidth / 2;

            if (Input.mousePosition.y < (mapHeight / 2))
                y = (Input.mousePosition.y - (mapHeight / 2));

            if (Input.mousePosition.x > (mapWidth / 2))
                x = ((mapWidth / 2) - Input.mousePosition.x);

            if (Input.mousePosition.y > (mapHeight / 2))
                y = (Input.mousePosition.y - (mapHeight / 2));

            Vector3 worldCoord = new Vector3(x, y, 0);
            //Debug.Log("Clicked: " + Input.mousePosition);
            //Debug.Log(worldCoord);
            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow, 1000);

        }         
	}
}
