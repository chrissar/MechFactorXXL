using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ZoneController : MonoBehaviour {

    node[,] influenceMap;

    public Terrain terrain;

    Texture2D splatMap;
    Texture2D influenceTexture;

    int mapWidth;
    int mapHeight;

    public List<node> allNodes;

	// Use this for initialization
	void Start () {
        allNodes = new List<node>();
        splatMap = terrain.terrainData.alphamapTextures[0];
        mapWidth = 0;
        mapHeight = 0;

       
    }

    void createPNG()
    {
        byte[] bytes = influenceTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/splatmap.png", bytes);
    }

    void createTexture()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                influenceTexture.SetPixel(i, j, influenceMap[i, j].influence);
            }
        }

        influenceTexture.Apply();
    }

    void setInfluenceMap()
    {
        mapWidth = splatMap.width;
        mapHeight = splatMap.height;
        influenceMap = new node[mapWidth, mapHeight];
        influenceTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32, false);
        setNodeNeighbours();
    }

    void setNodeNeighbours()
    {
        for (int i=0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                // North West
                if (inBounds(i-1, j-1))
                {
                    influenceMap[i, j].northwest = influenceMap[i - 1, j - 1];
                    influenceMap[i, j].northwest.southeast = influenceMap[i, j];
                }

                // North
                if (inBounds(i, j - 1))
                {
                    influenceMap[i, j].north = influenceMap[i, j - 1];
                    influenceMap[i, j].north.south = influenceMap[i, j];
                }

                // North East
                if (inBounds(i + 1, j - 1))
                {
                    influenceMap[i, j].northeast = influenceMap[i + 1, j - 1];
                    influenceMap[i, j].northeast.southwest = influenceMap[i, j];
                }

                // East
                if (inBounds(i + 1, j))
                {
                    influenceMap[i, j].northeast = influenceMap[i + 1, j];
                    influenceMap[i, j].northeast.southwest = influenceMap[i, j];
                }

                // South East
                if (inBounds(i + 1, j + 1))
                {
                    influenceMap[i, j].southeast = influenceMap[i + 1, j + 1];
                    influenceMap[i, j].southeast.northwest = influenceMap[i, j];
                }

                // South
                if (inBounds(i, j + 1))
                {
                    influenceMap[i, j].south = influenceMap[i, j + 1];
                    influenceMap[i, j].south.north = influenceMap[i, j];
                }

                // South West
                if (inBounds(i - 1, j + 1))
                {
                    influenceMap[i, j].southwest = influenceMap[i - 1, j + 1];
                    influenceMap[i, j].southwest.northeast = influenceMap[i, j];
                }

                // West
                if (inBounds(i - 1, j))
                {
                    influenceMap[i, j].west = influenceMap[i - 1, j];
                    influenceMap[i, j].west.east = influenceMap[i, j];
                }

                allNodes.Add(influenceMap[i, j]);
            }
        }

        foreach (node nodeToProcess in allNodes)
        {
            nodeToProcess.SetNeighbours();
        }
    }

    bool inBounds (int x, int y)
    {
        // return x >= 0 && x < width && y >= 0 && y < height;
        if ((x < mapWidth) && (x >= 0) && (y < mapHeight) && (y >= 0))
        {
            return true;
        }
        else
        {
            return false;
        }      
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
