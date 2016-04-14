using UnityEngine;
using System.Collections;

public class DrawMap : MonoBehaviour {
    public int resWidth = 500;
    public int resHeight = 500;
    public Camera cam;
    private bool takeHiResShot = false;
    public string dirPath = "Textures";

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    public static string ScreenShotName(int width, int height, string path)
    {
        
        return string.Format("{0}/" + path + "/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void LateUpdate()
    {
        float aspect = cam.aspect;
        float pH = (int)resWidth / aspect;

        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            cam.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            cam.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            cam.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight, dirPath);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    }
}
