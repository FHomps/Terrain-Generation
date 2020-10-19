using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageRenderer : MonoBehaviour
{
    public RenderTexture renderTexture = null;

    public bool renderImage = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (renderImage && renderTexture != null) {
            var oldRT = RenderTexture.active;

            var tex = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            File.WriteAllBytes("test.png", tex.EncodeToPNG());
            RenderTexture.active = oldRT;
            renderImage = false;
        }
    }
}
