using System;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Referencje")]
    public Camera camera;

    [Header("Ustawienia Przybliżenia")]
    public float zoomStep = 5f;
    public float minCamSize = 2f;
    public float maxCamSize = 200f;
    [Tooltip("Czas w sekundach do osiągnięcia docelowego przybliżenia. Mniej = szybciej")]
    public float zoomSmoothTime = 0.1f;

    [Header("Ustawienia Przesuwania")]
    public bool enableSmoothPan = true;
    [Tooltip("Czas w sekundach do wyhamowania kamery. Mniej = bardziej responsywnie, więcej = efekt pływania")]
    public float panSmoothTime = 0.05f;

    private Vector3 dragOrigin;
    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 panVelocity = Vector3.zero;
    private float zoomVelocity = 0f;
    private float initialZ;
    void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        targetZoom = camera.orthographicSize;
        targetPosition = camera.transform.position;
        initialZ = camera.transform.position.z;
    }

    void Update()
    {
        if (Mouse.current == null) return;
        HandleZoom();
        HandlePan();
    }

    private void HandlePan()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame)
        {
            dragOrigin = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.isPressed || Mouse.current.middleButton.isPressed)
        {
            Vector2 currentScreen = Mouse.current.position.ReadValue();

            Vector3 worldOrigin = camera.ScreenToWorldPoint(dragOrigin);
            Vector3 worldCurrent = camera.ScreenToWorldPoint(currentScreen);

            Vector3 difference = worldOrigin - worldCurrent;
            difference.z = 0f;

            targetPosition += difference;

            dragOrigin = currentScreen;
        }

        if (enableSmoothPan)
        {
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, targetPosition, ref panVelocity, panSmoothTime);
        }
        else
        {
            camera.transform.position = targetPosition;
        }

        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, initialZ);
    }

    private void HandleZoom()
    {
        if (TokenInteraction.IsDraggingToken) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            float normalizedScroll = scroll / 120f;
            targetZoom -= scroll * zoomStep * (camera.orthographicSize * 0.5f);
            targetZoom = Mathf.Clamp(targetZoom, minCamSize, maxCamSize);
        }

        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime);
    }
}
