using UnityEngine;
using System.Collections;
using System.IO;

public class TerrainSplatReader : MonoBehaviour {

    public Texture2D splat;
    public Terrain terrain;

	// Use this for initialization
	void Start () {
        Texture2D splatTexture;
        splatTexture = splat;

        terrain = GetComponent<Terrain>();
        Debug.Log("Width of terrain: " + terrain.terrainData.size.x);
        Debug.Log("Length of terrain: " + terrain.terrainData.size.z);

        try
        {
            
            if (splat == null)
            {
                Debug.Log("Apparently your selection is either void, of a format that can't be cast as texture2D, or who knows what. Aborting...");
                return;
            }
            
            /*
            if (splat.width != splat.width || splat.height != splat.height)
            {
                Debug.Log("The splat texture, and the texture to splatify, differs in size! Aborting...");
                return;

            }*/
            /*
            Color[] theseColors = splat.GetPixels(0, 0, splat.width, splat.height);
            Color[] theseColorsSplat = splatTexture.GetPixels(0, 0, splatTexture.width, splatTexture.height);

            for (int i = 0; i < theseColors.Length; i++)
            {
                float alphaComponent = 1 - (theseColors[i].r + theseColors[i].g + theseColors[i].b);
                theseColors[i].a = alphaComponent;

                theseColorsSplat[i] = theseColors[i];
            }

            splatTexture.SetPixels(theseColorsSplat);
            splatTexture.Apply();
            */


            Texture2D tex = new Texture2D(splat.width, splat.height, TextureFormat.ARGB32, false);

            for (int i = 0; i < splat.width; i++)
            {
                for (int j = 0; j < splat.height; j++)
                {

                    tex.SetPixel(i, 
                        j, 
                        new Vector4(splat.GetPixel(i, j).r, 
                        splat.GetPixel(i, j).g, 
                        splat.GetPixel(i, j).b, 
                        1));
                }
            }

            tex.Apply();

            Debug.Log("Creating splatmap.png...");
            byte[] bytes = tex.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + "/splatmap.png", bytes);
        }
        catch
        {
            Debug.Log("Save error");
            Debug.Break();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
