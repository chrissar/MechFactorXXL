using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public enum influence_map
{
    visibility,
    security,
    control,
    proximity,
    cover
};

/// <summary>
/// The Zone Contoller class manages a "Zone" which is a 2D grid of nodes wherein each node has cell information.
/// A cell's information is derived as the following information at it's coordinate in the following maps:
/// Visibility, Security, Proximity, and Control.
/// </summary>
public class ZoneController : MonoBehaviour {

    bool isDebug = false;
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
    public float[,] coverMap;

    public Terrain terrain;

    Texture2D heightMap;
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

        //ExportHeightMapToPNG();
        TestGetTrees();
    }

    public void InitInfluenceMaps()
    {
        mapWidth = terrain.terrainData.alphamapTextures[0].width;
        mapHeight = terrain.terrainData.alphamapTextures[0].height;

        securityMap = new float[mapWidth, mapHeight];
        visibilityMap = new float[mapWidth, mapHeight];
        proximityMap = new float[mapWidth, mapHeight];
        controlMap = new float[mapWidth, mapHeight];
        coverMap = new float[mapWidth, mapHeight];
    }

    /// <summary>
    /// Initializes the visilibility grid based on the
    /// height map, from a scale of 0.0f to 1.0f.
    /// </summary>
    public void setVisibilityMap()
    {
        for (int i = 0; i < heightMap.width; i++)
        {
            for (int j = 0; j < heightMap.height; j++)
            {
                // scale value to be between 0 and 1.
                float height = heightMap.GetPixel(i, j).r / 255;
                visibilityMap[i, j] = height;
            }
        }
    }


    /// <summary>
    /// Initializes the cover influence map.
    /// </summary>
    void FindAllCoverPoints()
    {
        List<GameObject> covers = new List<GameObject>(GameObject.FindGameObjectsWithTag("cover"));
        List<TreeInstance> trees = new List<TreeInstance>(terrain.terrainData.treeInstances);
        int new_x = 0;
        int new_y = 0;
        foreach (GameObject cover_point in covers)
        {
            Vector2 coord = CoordFromWorldPoint(cover_point.transform.position);
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    new_x = (int)coord.x + x;
                    new_y = (int)coord.y + y;
                    if (inBounds(new_x, new_y))
                    {
                        // make this point in cover grid = to 1.
                        coverMap[new_x, new_y] = 1.0f;
                    }
                }
            }
            // The cover point itself should not be considered under any circumstance.
            coverMap[(int)coord.x, (int)coord.y] = -1.0f;
        }
    }

    void TestGetTrees()
    {
        Debug.Log(terrain.terrainData.treeInstanceCount);
        TreeInstance tree = terrain.terrainData.GetTreeInstance(0);
        Debug.Log(tree.position);        
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

    /// <summary>
    /// Converts an X,Y grid coordinate to its corresponding 
    /// world space coord.
    /// </summary>
    /// <param name="x">A point along the horizontal x-axis.</param>
    /// <param name="y">A point along the vertical y-axis.</param>
    /// <returns>A Vector3 corresponding to World Space coordinate.</returns>
    Vector3 CoordToWorldPoint(int x, int y)
    {
        return new Vector3(-mapWidth / 2 + .5f + x, 2, -mapHeight / 2 + .5f + y);
    }

    /// <summary>
    /// Converts an Vector2 comprising a X, Y grid coordinate to its corresponding 
    /// world space coord.
    /// </summary>
    /// <param name="x">A point along the horizontal x-axis.</param>
    /// <param name="y">A point along the vertical y-axis.</param>
    /// <returns>A Vector3 corresponding to World Space coordinate.</returns>
    Vector3 CoordToWorldPoint(Vector2 coord)
    {
        return new Vector3(-mapWidth / 2 + .5f + coord.x, 2, -mapHeight / 2 + .5f + coord.y);
    }

    public Vector2 CoordFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + mapWidth / 2) / mapWidth;
        float percentY = (worldPosition.z + mapHeight / 2) / mapHeight;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((mapWidth - 1) * percentX);
        int y = Mathf.RoundToInt((mapHeight - 1) * percentY);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Returns the node in a given 2D vector direction relative to a current coordinate.
    /// </summary>
    /// <param name="direction">A Vector2 containing one of eight directions.</param>
    /// <returns>A node at a determined coordinate.</returns>
    node getNodeInDirection(Vector2 direction)
    {
        // A Direction can be from any point from -1,-1 to +1,+1
        // relative to a given coordinate.
        return influenceMap[(int)direction.x, (int)direction.y];
    }

    /// <summary>
    /// A coordinate is in bounds if it isn't outside of our grid.
    /// </summary>
    /// <param name="x">Horizontal x-axis coordinate.</param>
    /// <param name="y">Vertical y-axis coordinate.</param>
    /// <returns>A boolean.</returns>
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

    /// <summary>
    /// A coordinate is in bounds if it isn't outside of our grid.
    /// </summary>
    /// <param name="direction">A Vector2 containing our x,y coordinates.</param>
    /// <returns>A boolean.</returns>
    bool inBounds(Vector2 coordinate)
    {
        // return x >= 0 && x < width && y >= 0 && y < height;
        if ((coordinate.x < mapWidth) && (coordinate.x >= 0) && (coordinate.y < mapHeight) && (coordinate.y >= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// We take our height map and converter it to a 2D Texture for visualization purposes.
    /// </summary>
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
        heightMap = duplicateHeightMap;
        /// make it a PNG and save it to the Assets folder
        myBytes = duplicateHeightMap.EncodeToPNG();
        string filename = "DupeHeightMap.png";
        File.WriteAllBytes(Application.dataPath + "/" + filename, myBytes);

        if (isDebug)
            Debug.Log("Heightmap Duplicated " + "Saved as PNG in Assets/ as: " + filename);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
