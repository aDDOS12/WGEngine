using System;
using TMPro;
using UnityEngine;

public class UnitToken : MonoBehaviour
{
    [Header("Referencje Wizualne")]
    public SpriteRenderer tokenSprite;
    public TMP_Text textUnitName;
    public TMP_Text textSoldierCount;

    public Unit UnitData { get; private set; }

    public void InitializeUnit(Unit unitData, Color factionColor)
    {
        UnitData = unitData;

        if (tokenSprite != null)
        {
            tokenSprite.color = factionColor;
        }

        UpdateVisual();
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
    }
}
