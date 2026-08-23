using System;
using TMPro;
using UnityEngine;

public class UnitToken : MonoBehaviour
{
    [Header("Warstwy Wizualne")]
    public SpriteRenderer layerCategory;
    public SpriteRenderer layerType;
    public SpriteRenderer layerFrame;
    public SpriteRenderer layerQuality;
    public SpriteRenderer layerIcon;

    [Header("Baza Grafik")]
    public Sprite[] categorySprites;
    public Sprite[] typeSprites;
    public Sprite[] frameSprites;
    public Sprite[] qualitySprites;
    public Sprite[] iconSprites;

    [Header("Pływające UI Pionka")]
    public TMP_Text textUnitName;
    public TMP_Text textSoldierCount;

    [Header("Interakcja")]
    public GameObject selectionHighlight;

    public Unit UnitData { get; private set; }

    public void InitializeUnit(Unit unitData, UnitVisualData visualData, Color factionColor)
    {
        SetSelected(false);
        UnitData = unitData;

        if (visualData != null)
        {
            SetLayerSprite(layerCategory, visualData.CategoryIndex, categorySprites);
            SetLayerSprite(layerType, visualData.TypeIndex, typeSprites);
            SetLayerSprite(layerFrame, visualData.FrameIndex, frameSprites);
            SetLayerSprite(layerQuality, visualData.QualityIndex, qualitySprites);
            SetLayerSprite(layerIcon, visualData.IconIndex, iconSprites);
        }

        if (layerCategory != null) layerCategory.color = factionColor;
        if (layerType != null) layerType.color = factionColor;

        UpdateVisual();
    }

    private void SetLayerSprite(SpriteRenderer layer, int index, Sprite[] sprites)
    {
        if (layer != null && sprites != null && index >= 0 && index < sprites.Length && sprites[index] != null)
        {
            layer.sprite = sprites[index];
            layer.enabled = true;
        }
        else if (layer != null)
        {
            layer.sprite = null;
            layer.enabled = false;
        }
    }

    private void UpdateVisual()
    {
        if (UnitData == null) return;

        if (textUnitName != null) textUnitName.text = UnitData.UnitName;

        if (textSoldierCount != null)
        {
            textSoldierCount.text = $"{UnitData.SoldierCount} / {UnitData.StartingSoldierCount}";

            if (UnitData.IsBroken)
            {
                textSoldierCount.color = Color.white;
            }
        }
    }
    private void OnMouseDown()
    {
        if (UnitData == null) return;
        Debug.Log($"Kliknięto choragiew: {UnitData.UnitName} (Frakcja: {UnitData.Faction})");
        BattleManager.Instance.SelectUnit(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(isSelected);
        }
    }
}
