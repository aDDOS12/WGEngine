using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionArrowController : MonoBehaviour
{
    //private LineRenderer lineRenderer;
    private Camera mainCamera;

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
                if (unit.UnitData.IsBroken)
                {
                    Debug.Log($"[Morale] {unit.UnitData.UnitName} panikuje! Odmawia wykonania rozkazu.");
                    continue;
                }

                DrawActionVector(unit, targetWorldPos, targetUnit, isShiftPressed);
            }
        }
    }

    private void DrawActionVector(UnitToken sourceUnit, Vector3 targetPos, UnitToken targetUnit, bool isRangedIntent)
    {
        LineRenderer lr = GetOrCreateLineForUnit(sourceUnit);
        
        Vector3 startPos = sourceUnit.transform.position;
        Vector3 dir = (targetPos - startPos).normalized; 

        float distance = Vector3.Distance(startPos, targetPos);
        float fixedHeadLength = 0.8f;
        if (distance < fixedHeadLength) fixedHeadLength = distance * 0.5f;
        float headRatio = fixedHeadLength / distance;
        float headStart = 1f - headRatio;

        Vector3 headBasePos = targetPos - (dir * fixedHeadLength);

        lr.positionCount = 4;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, headBasePos);
        lr.SetPosition(2, headBasePos + (dir * 0.001f)); // Minimalne przesunięcie dla ostrego kąta
        lr.SetPosition(3, targetPos);

        float bodyWidth = 0.25f;
        float headWidth = 0.8f;

        AnimationCurve dynamicCurve = new AnimationCurve();
        dynamicCurve.AddKey(new Keyframe(0f, bodyWidth, 0f, 0f));                   // Początek trzonu
        dynamicCurve.AddKey(new Keyframe(headStart, bodyWidth, 0f, 0f));       // Koniec trzonu
        dynamicCurve.AddKey(new Keyframe(headStart + 0.001f, headWidth, 0f, 0f));       // Podstawa grotu (szeroka)
        dynamicCurve.AddKey(new Keyframe(1f, 0f, 0f, 0f));                      // Czubek grotu

        lr.widthCurve = dynamicCurve;
        lr.widthMultiplier = 1f;

        bool isEnemy = targetUnit != null && targetUnit.UnitData.Faction != sourceUnit.UnitData.Faction;
        float unitMobility = sourceUnit.UnitData.MobilityValue;
        float unitRange = sourceUnit.UnitData.Range;

        if (isRangedIntent)
        {
            // --- ATAK DYSTANSOWY (Shift + PPM) ---
            if (isEnemy && distance <= unitRange)
            {
                SetLineColor(lr, attackColor);
                Debug.Log($"[Wektor] Atak dystansowy na {targetUnit.UnitData.UnitName}. Dystans: {distance:F2}/{unitRange:F2}");

                BattleManager.Instance.RegisterOrder(new CombatOrder
                {
                    SourceUnit = sourceUnit,
                    TargetUnit = targetUnit,
                    TargetPosition = sourceUnit.transform.position,
                    IsRangedAttack = true,
                    Distance = distance
                });
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
                if (distance <= unitMobility)
                {
                    SetLineColor(lr, attackColor);
                    Debug.Log($"[Wektor] Szarża/Zwarcie z {targetUnit.UnitData.UnitName} zatwierdzona! Dystans: {distance:F2}");

                    float engagementOffset = 1.1f;

                    Vector3 combatDestination = sourceUnit.transform.position;

                    if (distance > engagementOffset)
                    {
                        Vector3 dirToTarget = (targetPos - sourceUnit.transform.position).normalized;
                        combatDestination = targetPos - (dirToTarget * engagementOffset);
                    }

                    BattleManager.Instance.RegisterOrder(new CombatOrder
                    {
                        SourceUnit = sourceUnit,
                        TargetUnit = targetUnit,
                        TargetPosition = combatDestination,
                        IsRangedAttack = false,
                        Distance = distance
                    });
                }
                else
                {
                    SetLineColor(lr, invalidColor);
                    Debug.Log($"[Wektor] Błąd: Wróg poza zasięgiem szarży ({distance:F2} > {unitMobility:F2}).");
                }
            }
            else
            {
                if (distance <= unitMobility)
                {
                    SetLineColor(lr, moveColor);
                    Debug.Log($"[Wektor] Rozkaz ruchu. Dystans: {distance:F2}/{unitMobility:F2}");

                    BattleManager.Instance.RegisterOrder(new CombatOrder
                    {
                        SourceUnit = sourceUnit,
                        TargetUnit = null,
                        TargetPosition = targetPos,
                        IsRangedAttack = false,
                        Distance = distance
                    });
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
        lr.material = lineMaterial;
        lr.sortingLayerName = "UI";
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
