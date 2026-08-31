using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RosterButton : MonoBehaviour
{
    [Header("Referencje UI")]
    public Image unitIconImage;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitDetailsText;

    public string TemplaeId { get; private set; }

    public void Setup(Unit template, Color factionColor)
    {
        TemplaeId = template.Id;

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
            unitDetailsText.text = $"{template.Faction} [{template.StartingSoldierCount}/{template.SoldierCount}]";
        }
    }
}
