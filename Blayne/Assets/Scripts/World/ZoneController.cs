using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Zone Contoller class manages a "Zone" which is a 2D grid of nodes wherein each node has cell information.
/// A cell's information is derived as the following information at it's coordinate in the following maps:
/// Visibility, Security, Proximity, and Control.
/// </summary>
public class ZoneController : MonoBehaviour {

    /*
        A 2D grid of "nodes" that possess information
        about a particular 1 by 1 square meter area 
        of the map.

        Security.
        Control.
        Proximity.
        Visibility.
    */
    public node[,] influenceMap;

    // These mappings are updated asynchronously
    // and the influence map is updated by summing
    // these maps into one map which is translated
    // to the nodes.
    public float[,] securityMap;
    public float[,] visibilityMap;
    public float[,] proximityMap;
    public float[,] controlMap;

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

        securityMap = new float[mapWidth, mapHeight];
        visibilityMap = new float[mapWidth, mapHeight];
        proximityMap = new float[mapWidth, mapHeight];
        controlMap = new float[mapWidth, mapHeight];

        //ExportHeightMapToPNG();
    }

    void createPNG()
    {
        byte[] bytes = influenceTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/splatmap.png", bytes);
    }

    /// <summary>
    /// Creates a PNG texture. Usually as a graphical representation 
    /// of our 2D maps.
    /// </summary>
    /// <param name="output">The filename of our texture.</param>
    void createPNG(string output)
    {
        byte[] bytes = influenceTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + output, bytes);
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

    // 
    void setNodeNeighbours()
    {
        Vector2 north;
        Vector2 northeast;
        Vector2 east;
        Vector2 southeast;
        Vector2 south;
        Vector2 southwest;
        Vector2 west;
        Vector2 northwest;

        for (int i=0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                north = new Vector2(i, j - 1);
                northeast = new Vector2(i + 1, j - 1);
                east = new Vector2(i + 1, j);
                southeast = new Vector2(i + 1, j + 1);
                south = new Vector2(i, j + 1);
                southwest = new Vector2(i - 1, j + 1);
                west = new Vector2(i - 1, j);
                northwest = new Vector2(i - 1, j - 1);

                // North West
                if (inBounds(northwest))
                {
                    influenceMap[i, j].northwest = getNodeInDirection(northeast);
                    influenceMap[i, j].northwest.southeast = influenceMap[i, j];
                }

                // North
                if (inBounds(north))
                {
                    influenceMap[i, j].north = getNodeInDirection(north);
                    influenceMap[i, j].north.south = influenceMap[i, j];
                }

                // North East
                if (inBounds(northeast))
                {
                    influenceMap[i, j].northeast = getNodeInDirection(northeast);
                    influenceMap[i, j].northeast.southwest = influenceMap[i, j];
                }

                // East
                if (inBounds(east))
                {
                    influenceMap[i, j].northeast = getNodeInDirection(east);
                    influenceMap[i, j].northeast.southwest = influenceMap[i, j];
                }

                // South East
                if (inBounds(southeast))
                {
                    influenceMap[i, j].southeast = getNodeInDirection(southeast);
                    influenceMap[i, j].southeast.northwest = influenceMap[i, j];
                }

                // South
                if (inBounds(south))
                {
                    influenceMap[i, j].south = getNodeInDirection(south);
                    influenceMap[i, j].south.north = influenceMap[i, j];
                }

                // South West
                if (inBounds(southwest))
                {
                    influenceMap[i, j].southwest = getNodeInDirection(southwest);
                    influenceMap[i, j].southwest.northeast = influenceMap[i, j];
                }

                // West
                if (inBounds(west))
                {
                    influenceMap[i, j].west = getNodeInDirection(west);
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

    node getNodeInDirection(Vector2 direction)
    {
        return influenceMap[(int)direction.x, (int)direction.y];
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

    bool inBounds(Vector2 direction)
    {
        // return x >= 0 && x < width && y >= 0 && y < height;
        if ((direction.x < mapWidth) && (direction.x >= 0) && (direction.y < mapHeight) && (direction.y >= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ExportHeightMapToPNG()
    {
        TerrainData terraindata = terrain.terrainData;
        //// get the terrain heights into an array and apply them to a texture2D
        byte[] myBytes;
        float[,] rawHeights = new float[0,0];
        Texture2D duplicateHeightMap = new Texture2D(terraindata.heightmapWidth, terraindata.heightmapHeight, TextureFormat.ARGB32, false);
        rawHeights = terraindata.GetHeights(0, 0, terraindata.heightmapWidth, terraindata.heightmapHeight);

        /// run through the array row by row
        for (int y = 0; y < duplicateHeightMap.height; ++y)
        {
            for (int x = 0; x < duplicateHeightMap.width; ++x)
            {
                /// for wach pixel set RGB to the same so it's gray
                Vector4 color = new Vector4(rawHeights[y, x], rawHeights[y, x], rawHeights[y, x], 1.0f);
                duplicateHeightMap.SetPixel(x, y, color);
                //myIndex++;
            }
        }
        // Apply all SetPixel calls
        duplicateHeightMap.Apply();

        /// make it a PNG and save it to the Assets folder
        myBytes = duplicateHeightMap.EncodeToPNG();
        string filename = "DupeHeightMap.png";
        File.WriteAllBytes(Application.dataPath + "/" + filename, myBytes);
        Debug.Log("Heightmap Duplicated " + "Saved as PNG in Assets/ as: " + filename);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
