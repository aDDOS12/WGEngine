using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionArrowController : MonoBehaviour
{
    //private LineRenderer lineRenderer;
    private Camera mainCamera;
    private AnimationCurve arrowCurve;

    [Header("Kolory Wektora")]
    public Color moveColor = Color.green; // ruch
    public Color attackColor = Color.red; // atak
    public Color invalidColor = Color.gray;

    [Header("Referencje")]
    [Tooltip("Przypisz tu standardowy materiał Sprites-Default")]
    public Material lineMaterial;

    private Dictionary<UnitToken, LineRenderer> unitLines = new Dictionary<UnitToken, LineRenderer>();

    private void Awake()
    {
        mainCamera = Camera.main;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 0.1f);   // Trzon strzałki
        curve.AddKey(0.85f, 0.1f);
        curve.AddKey(0.85f, 0.3f); // Podstawa grotu
        curve.AddKey(1f, 0f);      // Czubek grotu
    }

    private void Update()
    {
        if (BattleManager.Instance.CurrentPhase != BattlePhase.Combat)
        {
            ClearAllLines();
            return;
        }

        HandleInput();
        UpdateLinesVisibility();
    }

    private void HandleInput()
    {
        if (Mouse.current == null || Keyboard.current == null) return;

        var selectedUnits = BattleManager.Instance.SelectedUnits;

        if (selectedUnits.Count > 0 && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            targetWorldPos.z = 0f;

            bool isShiftPressed = Keyboard.current.shiftKey.isPressed;

            Collider2D hit = Physics2D.OverlapPoint(targetWorldPos);
            UnitToken targetUnit = hit != null ? hit.GetComponent<UnitToken>() : null;

            // Rozdajemy rozkaz każdej zaznaczonej jednostce
            foreach (var unit in selectedUnits)
            {
                DrawActionVector(unit, targetWorldPos, targetUnit, isShiftPressed);
            }
        }
    }

    private void DrawActionVector(UnitToken sourceUnit, Vector3 targetPos, UnitToken targetUnit, bool isRangedIntent)
    {
        LineRenderer lr = GetOrCreateLineForUnit(sourceUnit);
        
        Vector3 startPos = sourceUnit.transform.position;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, targetPos);

        float distance = Vector3.Distance(startPos, targetPos);
        bool isEnemy = targetUnit != null && targetUnit.UnitData.Faction != sourceUnit.UnitData.Faction;

        float unitMobility = sourceUnit.UnitData.Mobility;
        float unitRange = sourceUnit.UnitData.Range;

        if (isRangedIntent)
        {
            // --- ATAK DYSTANSOWY (Shift + PPM) ---
            if (isEnemy && distance <= unitRange)
            {
                SetLineColor(lr, attackColor);
                Debug.Log($"[Wektor] Atak dystansowy na {targetUnit.UnitData.UnitName}. Dystans: {distance:F2}/{unitRange:F2}");
            }
            else
            {
                SetLineColor(lr, invalidColor);
                if (!isEnemy) Debug.Log("[Wektor] Błąd: Atak dystansowy wymaga wskazania wrogiej jednostki.");
                else Debug.Log($"[Wektor] Błąd: Cel poza zasięgiem strzału ({distance:F2} > {unitRange:F2}).");
            }
        }
        else
        {
            // --- RUCH LUB ZWARCIE (PPM) ---
            if (isEnemy)
            {
                if (distance <= 1.1f)
                {
                    SetLineColor(lr, attackColor);
                    Debug.Log($"[Wektor] Walka w zwarciu z {targetUnit.UnitData.UnitName} zatwierdzona!");
                }
                else if (distance <= unitMobility)
                {
                    SetLineColor(lr, moveColor);
                    Debug.Log($"[Wektor] Ruch w stronę wroga. Zbyt daleko na bezpośrednie zwarcie ({distance:F2} > 1.1).");
                }
                else
                {
                    SetLineColor(lr, invalidColor);
                    Debug.Log($"[Wektor] Błąd: Wróg poza zasięgiem ruchu ({distance:F2} > {unitMobility:F2}).");
                }
            }
            else
            {
                if (distance <= unitMobility)
                {
                    SetLineColor(lr, moveColor);
                    Debug.Log($"[Wektor] Rozkaz ruchu. Dystans: {distance:F2}/{unitMobility:F2}");
                }
                else
                {
                    SetLineColor(lr, invalidColor);
                    Debug.Log($"[Wektor] Błąd: Punkt poza zasięgiem ruchu ({distance:F2} > {unitMobility:F2}).");
                }
            }
        }
    }

    private LineRenderer GetOrCreateLineForUnit(UnitToken unit)
    {
        if (unitLines.ContainsKey(unit)) return unitLines[unit];

        GameObject lineObj = new GameObject($"OrderLine_{unit.UnitData.UnitName}");
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.widthCurve = arrowCurve;
        lr.material = lineMaterial;
        lr.sortingOrder = 5;

        unitLines.Add(unit, lr);
        return lr;
    }

    private void SetLineColor(LineRenderer lr, Color color)
    {
        lr.startColor = color;
        lr.endColor = color;
    }

    private void UpdateLinesVisibility()
    {
        var selectedUnits = BattleManager.Instance.SelectedUnits;

        foreach (var kvp in unitLines)
        {
            UnitToken unit = kvp.Key;
            LineRenderer lr = kvp.Value;

            if (unit == null) continue;

            lr.enabled = selectedUnits.Contains(unit);
        }
    }

    private void ClearAllLines()
    {
        foreach (var lr in unitLines.Values)
        {
            if (lr != null) Destroy(lr.gameObject);
        }
        unitLines.Clear();
    }
}
