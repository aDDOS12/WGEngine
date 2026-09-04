using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(UnitToken))]
public class TokenInteraction : MonoBehaviour
{
    private UnitToken unitToken;
    private Camera mainCamera;
    private Vector3 dragOffset;
    private bool isDragging = false;
    private SpriteRenderer[] spriteRenderers;
    public static bool IsDraggingToken { get; private set; }

    private void Awake()
    {
        unitToken = GetComponent<UnitToken>();
        mainCamera = Camera.main;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (BattleManager.Instance.CurrentPhase != BattlePhase.Deployment) return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = GetMouseWorldPosition(mouseScreenPos);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                IsDraggingToken = true;
                dragOffset = transform.position - worldPos;
                SetSortingOrderOffset(10);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            IsDraggingToken = false;
            SetSortingOrderOffset(-10);
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            transform.position = worldPos + dragOffset;
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        if (Mouse.current == null) return;

        float rotationStep = 45f;
        float scrollY = Mouse.current.scroll.ReadValue().y;
;
        if (scrollY > 0f)
        {
            transform.Rotate(0, 0, rotationStep);
        }
        else if (scrollY < 0f)
        {
            transform.Rotate(0, 0, -rotationStep);
        }
    }

    private Vector3 GetMouseWorldPosition(Vector2 mousePos)
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;
        return worldPos;
    }

    private void SetSortingOrderOffset(int offset)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.sortingOrder += offset;
        }
    }
}
