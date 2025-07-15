using UnityEngine;
using System.IO;
public class RenderTextureExporter : MonoBehaviour
{
    public Camera captureCamera;
    public string screenshotName;
    public int width = 3840;
    public int height = 2160;

    private void Start()
    {
        ExportRenderTexture();
    }
    public void ExportRenderTexture()
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        captureCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Screenshots/" + screenshotName+".png", bytes);

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }
}
