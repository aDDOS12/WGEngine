using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadgeVisualController : MonoBehaviour
{
    [Header("Podgląd Plakietki")]
    public Image layerCategory;
    public Image layerType;
    public Image layerQuality;
    public Image layerBorder;
    public Image layerIcon;

    [Header("Edytor Wizualny (Listy)")]
    public TMP_Dropdown dropdownCategory;
    public TMP_Dropdown dropdownType;
    public TMP_Dropdown dropdownQuality;
    public TMP_Dropdown dropdownIcon;

    [Header("Edytor Wizualny (Kolor)")]
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public TMP_InputField inputHex;
    public TMP_InputField inputR;
    public TMP_InputField inputG;
    public TMP_InputField inputB;

    [Header("Baza Grafik do List Rozwijalnych")]
    public Sprite[] categorySprites;
    public Sprite[] typeSprites;
    public Sprite[] qualitySprites;
    public Sprite[] iconSprites;
    void Start()
    {
        sliderR.onValueChanged.AddListener(delegate { UpdateColorFromSliders(); });
        sliderG.onValueChanged.AddListener(delegate { UpdateColorFromSliders(); });
        sliderB.onValueChanged.AddListener(delegate { UpdateColorFromSliders(); });

        inputHex.onEndEdit.AddListener(delegate { UpdateColorFromHex(); });

        inputR.onEndEdit.AddListener(delegate { UpdateColorFromRGBInput(); });
        inputG.onEndEdit.AddListener(delegate { UpdateColorFromRGBInput(); });
        inputB.onEndEdit.AddListener(delegate { UpdateColorFromRGBInput(); });

        dropdownCategory.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });
        dropdownType.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });
        dropdownQuality.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });
        dropdownIcon.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });

        sliderR.value = 255;
        sliderG.value = 255;
        sliderB.value = 255;

        UpdateColorFromSliders();
        UpdateBadgeGraphics();
    }

    // Public API
    public void LoadVisualData(UnitVisualData visualData)
    {
        if (visualData != null)
        {
            dropdownCategory.value = visualData.CategoryIndex;
            dropdownType.value = visualData.TypeIndex;
            dropdownQuality.value = visualData.QualityIndex;
            dropdownIcon.value = visualData.IconIndex;
        }
        else
        {
            ClearDropdown();
        }
        UpdateBadgeGraphics();
    }

    public UnitVisualData GetVisualData(string unitId)
    {
        return new UnitVisualData
        {
            UnitId = unitId,
            CategoryIndex = dropdownCategory.value,
            TypeIndex = dropdownType.value,
            QualityIndex = dropdownQuality.value,
            IconIndex = dropdownIcon.value
        };
    }

    public void LoadColor(string hexCode)
    {
        inputHex.text = hexCode;
        UpdateColorFromHex();
    }
    
    public string GetCurrentHexCode()
    {
        return inputHex.text;
    }

    public void ClearVisuals(bool resetColorToWhite)
    {
        ClearDropdown();

        if (resetColorToWhite)
        {
            sliderR.value = 255;
            sliderG.value = 255;
            sliderB.value = 255;
            UpdateColorFromSliders();
        }

        UpdateBadgeGraphics();
    }

    private void ClearDropdown()
    {
        dropdownCategory.value = 0;
        dropdownType.value = 0;
        dropdownQuality.value = 0;
        dropdownIcon.value = 0;
    }

    // Visual logic

    private void UpdateColorFromSliders()
    {
        byte r = (byte)sliderR.value;
        byte g = (byte)sliderG.value;
        byte b = (byte)sliderB.value;

        Color32 newColor = new Color32(r, g, b, 255);

        layerCategory.color = newColor;
        layerType.color = newColor;

        inputHex.text = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        inputR.SetTextWithoutNotify(r.ToString());
        inputG.SetTextWithoutNotify(g.ToString());
        inputB.SetTextWithoutNotify(b.ToString());
    }

    private void UpdateColorFromHex()
    {
        string hexCode = inputHex.text;

        if (UnityEngine.ColorUtility.TryParseHtmlString(hexCode, out Color parsedColor))
        {
            sliderR.SetValueWithoutNotify(parsedColor.r * 255f);
            sliderG.SetValueWithoutNotify(parsedColor.g * 255f);
            sliderB.SetValueWithoutNotify(parsedColor.b * 255f);

            layerCategory.color = parsedColor;
            layerType.color = parsedColor;
        }
        else
        {
            Debug.LogWarning("Wpisano niepoprawny kod HEX!");
            UpdateColorFromSliders();
        }
    }

    private void UpdateColorFromRGBInput()
    {
        int r = int.TryParse(inputR.text, out int parsedR) ? parsedR : 0;
        int g = int.TryParse(inputG.text, out int parsedG) ? parsedG : 0;
        int b = int.TryParse(inputB.text, out int parsedB) ? parsedB : 0;

        r = Mathf.Clamp(r, 0, 255);
        g = Mathf.Clamp(g, 0, 255);
        b = Mathf.Clamp(b, 0, 255);

        sliderR.SetValueWithoutNotify(r);
        sliderG.SetValueWithoutNotify(g);
        sliderB.SetValueWithoutNotify(b);

        UpdateColorFromSliders();
    }

    private void UpdateBadgeGraphics()
    {
        SetLayerGraphic(layerCategory, dropdownCategory.value, categorySprites);
        SetLayerGraphic(layerType, dropdownType.value, typeSprites);
        SetLayerGraphic(layerQuality, dropdownQuality.value, qualitySprites);
        SetLayerGraphic(layerIcon, dropdownIcon.value, iconSprites);
    }

    private void SetLayerGraphic(Image layer, int dropdownIndex, Sprite[] availableSprites)
    {
        if (availableSprites != null && dropdownIndex >= 0 && dropdownIndex < availableSprites.Length && availableSprites[dropdownIndex] != null)
        {
            layer.sprite = availableSprites[dropdownIndex];
            layer.enabled = true;
        }
        else
        {
            layer.sprite = null;
            layer.enabled = false;
        }
    }
}
