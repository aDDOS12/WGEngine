using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Specialized;

public class SelectionManager : MonoBehaviour
{
    [Header("Reference UI")]
    public RectTransform selectionBox;

    private Vector2 startScreenPos;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (selectionBox != null) selectionBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (BattleManager.Instance.CurrentPhase != BattlePhase.Deployment) return;

        HandleShortcuts();

        if (TokenInteraction.IsDraggingToken) return;

        HandleBoxSelection();
    }

    private void HandleBoxSelection()
    {
        if (Mouse.current == null || selectionBox == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;

            if (Physics2D.OverlapPoint(worldPos) == null)
            {
                startScreenPos = mousePos;
                selectionBox.gameObject.SetActive(true);
                UpdateVisualBox(mousePos);
            }
        }

        if (Mouse.current.leftButton.isPressed && selectionBox.gameObject.activeInHierarchy)
        {
            UpdateVisualBox(mousePos);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && selectionBox.gameObject.activeInHierarchy)
        {
            selectionBox.gameObject.SetActive(false);
            SelectUnitsInBox(startScreenPos, mousePos);
        }
    }

    private void UpdateVisualBox(Vector2 currentMousePos)
    {
        float width = Mathf.Abs(currentMousePos.x - startScreenPos.x);
        float height = Mathf.Abs(currentMousePos.y - startScreenPos.y);

        selectionBox.sizeDelta = new Vector2(width, height);

        Vector2 center = (startScreenPos + currentMousePos) / 2f;
        selectionBox.position = center;
    }

    private void SelectUnitsInBox(Vector2 start, Vector2 end)
    {
        Vector2 worldStart = mainCamera.ScreenToWorldPoint(start);
        Vector2 worldEnd = mainCamera.ScreenToWorldPoint(end);

        Vector2 min = Vector2.Min(worldStart, worldEnd);
        Vector2 max = Vector2.Max(worldStart, worldEnd);

        Collider2D[] hits = Physics2D.OverlapAreaAll(min, max);

        BattleManager.Instance.ClearSelection(notify: false);

        Debug.Log($"Znaleziono {hits.Length} obiektów w ramce");

        var config = DataManager.Instance.CurrentBattleConfig;
        int allowedSide = 0;

        foreach (var hit in hits)
        {
            UnitToken token = hit.GetComponent<UnitToken>();
            if (token != null)
            {
                string factionId = token.UnitData.Faction;
                bool isAttacker = config.AttackingFactionIds.Contains(factionId);
                bool isDefender = config.DefendingFactionIds.Contains(factionId);

                if (allowedSide == 0)
                {
                    if (isAttacker) allowedSide = 1;
                    else if (isDefender) allowedSide = 2;
                    else continue;
                }

                if ((allowedSide == 1 && isAttacker) || (allowedSide == 2 && isDefender))
                {
                    BattleManager.Instance.SelectUnit(token, false);
                }

                Debug.Log($"- Wybrano z ramki: {token.UnitData.UnitName}");
            }
        }

        BattleManager.Instance.NotifySelectionChanged();
    }

    private void HandleShortcuts()
    {
        if (Keyboard.current == null) return;

        bool isCtrlPressed = Keyboard.current.ctrlKey.isPressed;

        if (isCtrlPressed)
        {
            // Ctrl + N -> Szereg poziomy
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                BattleManager.Instance.AlignSelectedToHorizontalRow();
            }

            // Opcjonalnie dorzucamy Ctrl + M -> Kolumna pionowa
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                BattleManager.Instance.AlignSelectedToVerticalColumn();
            }
        }
    }
}
