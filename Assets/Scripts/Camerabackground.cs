using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerabackground : MonoBehaviour
{
    public Camera camera;
    [Range(0.01F, 1.0F)]
    public float renderScale = 1.0F;
    public FilterMode filterMode = FilterMode.Bilinear;

    private Rect originalRect;
    private Rect scaledRect;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void OnDestroy()
    {
        camera.rect = originalRect;
    }

    void OnPreRender()
    {
        originalRect = camera.rect;
        scaledRect.Set(originalRect.x, originalRect.y, originalRect.width * renderScale, originalRect.height * renderScale);
        camera.rect = scaledRect;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        camera.rect = originalRect;
        src.filterMode = filterMode;
        Graphics.Blit(src, dest);
    }
}
