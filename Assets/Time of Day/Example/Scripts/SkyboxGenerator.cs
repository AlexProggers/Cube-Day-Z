using UnityEngine;
using System.IO;

public class SkyboxGenerator : MonoBehaviour
{
    public int Resolution = 1024;

#if UNITY_EDITOR && UNITY_STANDALONE

    private const string directory = "Assets/Skyboxes";

    private static string[] filenames = new string[] {
        "front.png",
        "right.png",
        "back.png",
        "left.png",
        "up.png",
        "down.png"
    };

    private static Vector3[] rotations = new Vector3[] {
        new Vector3(0, 0, 0),
        new Vector3(0, -90, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 90, 0),
        new Vector3(-90, 0, 0),
        new Vector3(90, 0, 0)
    };

    public void RenderSkybox()
    {
        GameObject go = new GameObject("Skybox Camera", typeof(Camera));

        go.GetComponent<Camera>().backgroundColor = Color.black;
        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        go.GetComponent<Camera>().fieldOfView = 90;
        go.GetComponent<Camera>().aspect = 1;

        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;

        for (int orientation = 0; orientation < rotations.Length ; orientation++)
        {
            RenderSkybox(orientation, go);
        }

        DestroyImmediate(go);
    }

    private void RenderSkybox(int orientation, GameObject go)
    {
        go.transform.eulerAngles = rotations[orientation];

        RenderTexture render_texture = new RenderTexture(Resolution, Resolution, 24);
        go.GetComponent<Camera>().targetTexture = render_texture;

        Texture2D screenshot = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, false);
        go.GetComponent<Camera>().Render();

        RenderTexture.active = render_texture;
        screenshot.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);

        RenderTexture.active = null;
        DestroyImmediate(render_texture);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllBytes(Path.Combine(directory, filenames[orientation]), screenshot.EncodeToPNG());
    }

#else

    public void RenderSkybox()
    {
        Debug.Log("RenderSkybox() is only supported in the Unity editor on desktop platforms.");
    }

#endif

}
