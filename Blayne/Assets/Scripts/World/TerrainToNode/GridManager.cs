using UnityEngine;
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

public class GridManager : MonoBehaviour {
    public Texture2D heightMap;

    public Terrain terrain;

    public float[,] cover_influence_map;
    public float[,] security_influence_map;
    public float[,] proximity_influence_map;
    public float[,] control_influence_map;
    public float[,] visibility_influence_map;

    int width = 0;
    int height = 0;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool inBounds(int x, int y)
    {
        // return x >= 0 && x < width && y >= 0 && y < height;
        if ((x < width) && (x >= 0) && (y < height) && (y >= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void InitInfluenceMaps()
    {
        width = terrain.terrainData.alphamapTextures[0].width;
        height = terrain.terrainData.alphamapTextures[0].height;

        cover_influence_map = new float[width, height];
        security_influence_map = new float[width, height];
        proximity_influence_map = new float[width, height];
        control_influence_map = new float[width, height];
        visibility_influence_map = new float[width, height];
    }

    /// <summary>
    /// Initializes the visilibility grid based on the
    /// height map, from a scale of 0.0f to 1.0f.
    /// </summary>
    public void setVisibilityMap()
    {

        for (int i=0; i < heightMap.width; i++)
        {
            for (int j=0; j < heightMap.height; j++)
            {
                // scale value to be between 0 and 1.
                float height = heightMap.GetPixel(i, j).r / 255;
                visibility_influence_map[i, j] = height;
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
                        cover_influence_map[new_x, new_y] = 1.0f;
                    }
                }
            }
            // The cover point itself should not be considered under any circumstance.
            cover_influence_map[(int)coord.x, (int)coord.y] = -1.0f;
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
        return new Vector3(-width / 2 + .5f + x, 2, -height / 2 + .5f + y);
    }

    Vector3 CoordToWorldPoint(Vector2 coord)
    {
        return new Vector3(-width / 2 + .5f + coord.x, 2, -height / 2 + .5f + coord.y);
    }

    public Vector2 CoordFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + width / 2) / width;
        float percentY = (worldPosition.z + height / 2) / height;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((width - 1) * percentX);
        int y = Mathf.RoundToInt((height - 1) * percentY);

        return new Vector2(x, y); 
    }
}
