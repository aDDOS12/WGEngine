using TMPro;
using Unity.AI.Navigation.LowLevel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RosterButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencje UI")]
    public Image unitIconImage;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitDetailsText;

    public string TemplateId { get; private set; }

    private GameObject ghostIcon;
    private RectTransform ghostRect;
    private Canvas mainCanvas;
    private Unit unitTemplate;
    private Color factionColor;

    public void Setup(Unit template, Color color)
    {
        unitTemplate = template;
        factionColor = color;
        TemplateId = template.Id;
        mainCanvas = GetComponentInParent<Canvas>();

        if (unitIconImage != null && template.VisualData != null) 
        {
            unitIconImage.sprite = TokenGraphicsManager.Instance.GetBakedSprite(template.VisualData, factionColor);
            unitIconImage.preserveAspect = true;
        }

        if (unitNameText != null)
        {
            unitNameText.text = template.UnitName;
        }

        if (unitDetailsText != null)
        {
            unitDetailsText.text = template.Faction;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ghostIcon = new GameObject("Ghost_UnitIcon");
        ghostIcon.transform.SetParent(mainCanvas.transform, false);
        ghostIcon.transform.SetAsLastSibling();

        Image ghostImage = ghostIcon.AddComponent<Image>();
        ghostImage.sprite = unitIconImage.sprite;
        ghostImage.preserveAspect = true;
        ghostImage.raycastTarget = false;

        ghostRect = ghostIcon.GetComponent<RectTransform>();
        ghostRect.sizeDelta = new Vector2(100f, 40f);

        Color ghostColor = ghostImage.color;
        ghostColor.a = 0.7f;
        ghostImage.color = ghostColor;

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIcon != null)
        {
            UpdateGhostPosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostIcon != null)
        {
            Destroy(ghostIcon);
            ghostIcon = null;
        }

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPoint.z = 0f;

        BattleManager.Instance.SpawnUnitOnBoard(unitTemplate, factionColor, worldPoint);
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition);

        ghostRect.localPosition = localPointerPosition;
    }
}
