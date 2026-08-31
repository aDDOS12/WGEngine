using System;
using System.Collections.Generic;
using UnityEngine;

public class TokenGraphicsManager : MonoBehaviour
{
    public static TokenGraphicsManager Instance { get; private set;  }

    [Header("Globalna Baza Grafik Plakietek")]
    public Sprite[] categorySprites;
    public Sprite[] typeSprites;
    public Sprite[] frameSprites;
    public Sprite[] qualitySprites;
    public Sprite[] iconSprites;

    [Header("Ustawienia Renderowania")]
    public int textureWidth = 400;
    public int textureHeight = 160;
    public float pixelsPerUnit = 400f;

    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Sprite GetBakedSprite(UnitVisualData visualData, Color factionColor)
    {
        if (visualData == null) return null;

        string cacheKey = $"{visualData.CategoryIndex}_{visualData.TypeIndex}_{visualData.FrameIndex}_{visualData.QualityIndex}_{visualData.IconIndex}_{ColorUtility.ToHtmlStringRGB(factionColor)}";

        if (_spriteCache.TryGetValue(cacheKey, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Sprite newSprite = BakeSprite(visualData, factionColor);
        if (newSprite != null)
        {
            _spriteCache[cacheKey] = newSprite;
        }

        return newSprite;
    }

    private Sprite BakeSprite(UnitVisualData data, Color color)
    {
        RenderTexture rt = RenderTexture.GetTemporary(textureWidth, textureHeight, 16);

        GameObject camObj = new GameObject("TempBakeCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;
        cam.orthographic = true;

        cam.orthographicSize = (textureHeight / pixelsPerUnit) / 2f;

        GameObject holder = new GameObject("TempBakeHolder");
        holder.transform.position = Vector3.zero;
        cam.transform.position = new Vector3(0, 0, -10f);

        CreateSpriteRenderer(holder.transform, GetSprite(categorySprites, data.CategoryIndex), color, 0);
        CreateSpriteRenderer(holder.transform, GetSprite(typeSprites, data.TypeIndex), color, 1);
        CreateSpriteRenderer(holder.transform, GetSprite(frameSprites, data.FrameIndex), Color.white, 2);
        CreateSpriteRenderer(holder.transform, GetSprite(qualitySprites, data.QualityIndex), Color.white, 3);
        CreateSpriteRenderer(holder.transform, GetSprite(iconSprites, data.IconIndex), Color.white, 4);

        cam.Render();

        RenderTexture.active = rt;
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        Sprite bakedSprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit);

        DestroyImmediate(camObj);
        DestroyImmediate(holder);
        RenderTexture.ReleaseTemporary(rt);

        return bakedSprite;
    }

    private Sprite GetSprite(Sprite[] sprites, int index)
    {
        if (sprites != null && index >= 0 && index < sprites.Length)
            return sprites[index];
        return null;
    }

    private void CreateSpriteRenderer(Transform parent, Sprite sprite, Color tint, int sortingOrder)
    {
        if (sprite == null) return;
        GameObject child = new GameObject($"Layer_{sortingOrder}");
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = child.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = tint;
        sr.sortingOrder = sortingOrder;
    }
}
