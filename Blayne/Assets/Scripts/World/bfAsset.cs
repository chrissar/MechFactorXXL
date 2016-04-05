using UnityEngine;
using System.Collections;

/// <summary>
/// bfAsset class defines properties of units or structures.
/// </summary>
public class bfAsset : MonoBehaviour {
    public float power = 0;
    public int maxRadius = 0;

    public float[,] localProximityMap;

    public void setNewProximityMap(int width, int height)
    {
        localProximityMap = new float[width, height];
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
