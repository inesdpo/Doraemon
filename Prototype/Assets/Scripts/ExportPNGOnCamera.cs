using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Result
{
    public string best_match;
}

public class ExportPNGOnCamera : MonoBehaviour
{

    public RenderTexture rt;

    public int i = 0;

    public RawImage result;


    // Start is called before the first frame update
    void Start()
    {

        //RenderTexture rt = Selection.activeObject as RenderTexture;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }



    public void SendPredict()
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();

        i++;
        //string path = Application.dataPath + i + ".png";
        string path = Application.persistentDataPath + i + ".png";
        ScreenCapture.CaptureScreenshot(path);
        // System.IO.File.WriteAllBytes(path, bytes);
        //AssetDatabase.ImportAsset(path);
        Debug.Log("Saved to " + path);

        
        ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);
        AsyncGPUReadback.Request(rt, 0, TextureFormat.RGBA32);


        result.texture = tex;

        //StartCoroutine(UploadMultipleFiles(path));
    }
}