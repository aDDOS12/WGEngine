using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangeVisualizer : MonoBehaviour
{
    [Header("Kolory Okręgów")]
    public Color moveColor = new Color(0f, 1f, 0f, 0.3f);   // Półprzezroczysty zielony
    public Color rangeColor = new Color(1f, 0f, 0f, 0.3f);  // Półprzezroczysty czerwony

    [Header("Konfiguracja Renderera")]
    [Tooltip("Przypisz standardowy materiał Sprites-Default")]
    public Material circleMaterial;

    [Tooltip("Im więcej segmentów, tym gładsze koło")]
    [Range(20, 100)] public int segments = 50;
    public float lineWidth = 0.05f;

    // Klasa pomocnicza przechowująca oba okręgi dla danej jednostki
    private class UnitCircles
    {
        public LineRenderer MoveCircle;
        public LineRenderer RangeCircle;
    }

    private Dictionary<UnitToken, UnitCircles> unitCircles = new Dictionary<UnitToken, UnitCircles>();

    private void Update()
    {
        if (BattleManager.Instance.CurrentPhase != BattlePhase.Combat)
        {
            ClearAllCircles();
            return;
        }

        UpdateCirclesVisibility();
    }

    private void UpdateCirclesVisibility()
    {
        var selectedUnits = BattleManager.Instance.SelectedUnits;
        bool isShiftPressed = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        // Najpierw ukrywamy wszystkie okręgi w słowniku
        foreach (var kvp in unitCircles)
        {
            if (kvp.Value.MoveCircle != null) kvp.Value.MoveCircle.enabled = false;
            if (kvp.Value.RangeCircle != null) kvp.Value.RangeCircle.enabled = false;
        }

        // Rysujemy i aktywujemy tylko dla zaznaczonych jednostek
        foreach (var unit in selectedUnits)
        {
            if (unit == null) continue;

            UnitCircles circles = GetOrCreateCircles(unit);
            Vector3 centerPos = unit.transform.position;

            if (isShiftPressed)
            {
                // Tryb ostrzału (Shift) - upewniamy się, że jednostka ma jakikolwiek zasięg
                if (unit.UnitData.Range > 0)
                {
                    DrawCircle(circles.RangeCircle, centerPos, unit.UnitData.Range);
                    circles.RangeCircle.enabled = true;
                }
            }
            else
            {
                // Tryb ruchu (Domyślny)
                DrawCircle(circles.MoveCircle, centerPos, unit.UnitData.MobilityValue);
                circles.MoveCircle.enabled = true;
            }
        }
    }

    private void DrawCircle(LineRenderer lr, Vector3 center, float radius)
    {
        lr.positionCount = segments + 1; // +1 aby domknąć koło

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * 360f * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;

            lr.SetPosition(i, center + new Vector3(x, y, 0f));
        }
    }

    private UnitCircles GetOrCreateCircles(UnitToken unit)
    {
        if (unitCircles.ContainsKey(unit)) return unitCircles[unit];

        UnitCircles circles = new UnitCircles();

        // Generowanie okręgu ruchu
        GameObject moveObj = new GameObject($"MoveCircle_{unit.UnitData.UnitName}");
        moveObj.transform.SetParent(transform);
        circles.MoveCircle = SetupLineRenderer(moveObj, moveColor);

        // Generowanie okręgu ataku
        GameObject rangeObj = new GameObject($"RangeCircle_{unit.UnitData.UnitName}");
        rangeObj.transform.SetParent(transform);
        circles.RangeCircle = SetupLineRenderer(rangeObj, rangeColor);

        unitCircles.Add(unit, circles);
        return circles;
    }

    private LineRenderer SetupLineRenderer(GameObject obj, Color color)
    {
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = circleMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.sortingOrder = 4; // Pod strzałką rozkazu (która ma 5)
        return lr;
    }

    private void ClearAllCircles()
    {
        foreach (var circles in unitCircles.Values)
        {
            if (circles.MoveCircle != null) Destroy(circles.MoveCircle.gameObject);
            if (circles.RangeCircle != null) Destroy(circles.RangeCircle.gameObject);
        }
        unitCircles.Clear();
    }
}
