using UnityEngine;
using System.Collections;

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

    public float[,] CreateInfluenceMap(influence_map _map)
    {
        float[,] grid = new float[heightMap.width, heightMap.height];
        switch (_map)
        {
            case influence_map.visibility:
                for (int i=0; i < heightMap.width; i++)
                {
                    for (int j=0; j < heightMap.height; j++)
                    {
                        // scale value to be between 0 and 1.
                        float height = heightMap.GetPixel(i, j).r / 255;
                        grid[i, j] = height;
                    }
                }
                break;
            case influence_map.control:
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                           
                    }
                }
                break;
        }

        return grid;
    }

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
