using System;
using System.Collections;
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
    public Transform textContainer;
    public float textOffsetDistance = 0.5f;

    public Vector2 FacingDirection { get; private set; } = Vector2.up;

    [Header("Interakcja")]
    public GameObject selectionHighlight;

    public Unit UnitData { get; private set; }

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        }
    }

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

    public void UpdateVisual()
    {
        if (UnitData == null) return;

        if (textUnitName != null) textUnitName.text = UnitData.UnitName;

        if (textSoldierCount != null)
        {
            textSoldierCount.text = $"{UnitData.SoldierCount} / {UnitData.TemplateSoldierCount}";

            if (UnitData.IsBroken)
            {
                textSoldierCount.color = Color.red;
                if (layerFrame != null) layerFrame.color = new Color(0.3f, 0.3f, 0.3f);
            }
            else
            {
                textSoldierCount.color = Color.white;
                // Tutaj przywracamy domyślny kolor ramki, jeśli zaimplementujesz system zbierania się (Rally)
                if (layerFrame != null) layerFrame.color = Color.white;
            }
        }
    }

    private void HandleSelectionChanged()
    {
        SetSelected(BattleManager.Instance.SelectedUnits.Contains(this));
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(isSelected);
        }
    }

    public void SetFacingDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        FacingDirection = direction.normalized;
        transform.up = FacingDirection;
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}
