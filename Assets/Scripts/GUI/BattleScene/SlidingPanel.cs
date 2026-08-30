using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlidingPanel : MonoBehaviour
{
    [Header("Referencje")]
    public RectTransform panelRect;
    public Button toggleButton;

    [Header("Ustawienia Pozycji")]
    [Tooltip("Pozycja AnchoredPosition, gdy panel jest owarty (np. X = 0)")]
    public Vector2 shownPosition;
    [Tooltip("Pozycja AnchoredPosition, gdy panel jest schowany (np. X = -300)")]
    public Vector2 hiddenPosition;

    [Header("Animacja")]
    public float slideDuration = 0.3f;

    private bool isShown = true;
    private Coroutine slideCoroutine;

    void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePanel);
        }

        panelRect.anchoredPosition = isShown ? shownPosition : hiddenPosition;
    }

    public void TogglePanel()
    {
        isShown = !isShown;

        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(Slide(isShown ? shownPosition : hiddenPosition));
    }

    private IEnumerator Slide(Vector2 targetPos)
    {
        Vector2 startPos = panelRect.anchoredPosition;
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;

            t = t * t * (3f - 2f * t);

            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        panelRect.anchoredPosition = targetPos;
    }
}
