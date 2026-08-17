using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditorController : MonoBehaviour
{

    [Header("Zarządzania Frakcją i Listą)")]
    public TMP_Dropdown dropdownFactionSelect; // faction select dropdown
    // variables needed to generate unit list
    public Transform unitListContentContainer; // "Content" object inside scroll view
    public GameObject unitListItemPrefab; // scroll view button prefab

    [Header("Panele stats/badge")]
    public GameObject statsSection;
    public GameObject badgeEditorSection;

    [Header("Formularz Statystyk")]
    public TMP_InputField inputUnitName;
    public TMP_InputField inputBaseAttack;
    public TMP_InputField inputBaseDefence;
    public TMP_InputField inputRangedAttack;
    public TMP_InputField inputRangedDefence;
    public TMP_InputField inputRange;
    public TMP_InputField inputMobility;
    public TMP_InputField inputDurability;
    public TMP_InputField inputMorale;
    public TMP_InputField inputSoldierCount;

    [Header("Podgląd Plakietki")]
    public Image layerCategory;
    public Image layerType;
    public Image layerQuality;
    public Image layerBorder;
    public Image layerIcon;

    [Header("Edytor Wizualny")]
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

    [Header("Przyciski (Nawigacja)")]
    public Button btnEditBadge;
    public Button btnConfirmBadge;

    [Header("Stan Edytora (Logika)")]
    private bool isEditingMode = false;
    private Unit currentEditedUnit = null;

    [Header("Baza Grafik do List Rozwijalnych")]
    public Sprite[] categorySprites;
    public Sprite[] typeSprites;
    public Sprite[] qualitySprites;
    public Sprite[] iconSprites;
    void Start()
    {
        ShowStatsSection(); // show statsSection as default

        // TODO: Add listeners
        // Navigation Buttons
        btnEditBadge.onClick.AddListener(ShowBadgeEditor);
        btnConfirmBadge.onClick.AddListener(ShowStatsSection);


        sliderR.value = 255;
        sliderG.value = 255;
        sliderB.value = 255;

        sliderR.onValueChanged.AddListener(delegate { UpdateColorFromSliders(); });
        sliderG.onValueChanged.AddListener(delegate { UpdateColorFromSliders(); });
        sliderB.onValueChanged.AddListener(delegate { UpdateColorFromSliders(); });

        inputHex.onEndEdit.AddListener(delegate { UpdateColorFromHex(); });

        inputR.onEndEdit.AddListener(delegate { UpdateColorFromRGBInput(); });
        inputG.onEndEdit.AddListener(delegate { UpdateColorFromRGBInput(); });
        inputB.onEndEdit.AddListener(delegate { UpdateColorFromRGBInput(); });

        UpdateColorFromSliders();

        // dropdown list events
        dropdownCategory.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });
        dropdownType.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });
        dropdownQuality.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });
        dropdownIcon.onValueChanged.AddListener((int index) => { UpdateBadgeGraphics(); });

        UpdateBadgeGraphics();
    }

    // button methods

    public void ShowBadgeEditor()
    {
        statsSection.SetActive(false);
        badgeEditorSection.SetActive(true);
    }

    public void ShowStatsSection()
    {
        badgeEditorSection.SetActive(false);
        statsSection.SetActive(true);
    }

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

    public void LoadUnitToEditor(Unit unitToEdit)
    {
        isEditingMode = true;
        currentEditedUnit = unitToEdit;

        inputUnitName.text = unitToEdit.UnitName;

        // Combat stats
        inputBaseAttack.text = unitToEdit.BaseAttack.ToString();
        inputBaseDefence.text = unitToEdit.BaseDefence.ToString();
        inputRangedAttack.text = unitToEdit.RangedAttack.ToString();
        inputRangedDefence.text = unitToEdit.RangedDefence.ToString();
        inputRange.text = unitToEdit.Range.ToString();

        // Parameters
        inputMorale.text = unitToEdit.Morale.ToString();
        inputDurability.text = unitToEdit.Durability.ToString();
        inputMobility.text = unitToEdit.Mobility.ToString();
        inputSoldierCount.text = unitToEdit.SoldierCount.ToString();

        // TODO: wizualizacja i kolor

        // TODO: ustawienia koloru w oparciu o obiekt z backendu

        UpdateBadgeGraphics();
        UpdateColorFromSliders();

        ShowStatsSection();
    }

    public void SaveUnitFromEditor()
    {
        if (!isEditingMode || currentEditedUnit == null)
        {
            currentEditedUnit = new Unit();
        }

        currentEditedUnit.UnitName = inputUnitName.text;

        // combat stats parse
        currentEditedUnit.BaseAttack = int.TryParse(inputBaseAttack.text, out int att) ? att : 0;
        currentEditedUnit.BaseDefence = int.TryParse(inputBaseDefence.text, out int def) ? def : 0;
        currentEditedUnit.RangedAttack = int.TryParse(inputRangedAttack.text, out int rAtt) ? rAtt : 0;
        currentEditedUnit.RangedDefence = int.TryParse(inputRangedDefence.text, out int rDef) ? rDef : 0;
        currentEditedUnit.Range = int.TryParse(inputRange.text, out int rng) ? rng : 0;

        // parameters parse
        currentEditedUnit.BaseMorale = int.TryParse(inputMorale.text, out int mor) ? mor : 0;
        currentEditedUnit.Durability = int.TryParse(inputDurability.text, out int dur) ? dur : 0;
        currentEditedUnit.Mobility = int.TryParse(inputMobility.text, out int mob) ? mob : 0;
        currentEditedUnit.SoldierCount = int.TryParse(inputSoldierCount.text, out int count) ? count : 0;

        // TODO: wizualizacja i kolory

        if (isEditingMode)
        {
            Debug.Log($"Zakutalizowano istniejącą chorągiew: {inputUnitName.text}");
            // TODO: podmiana danych w liście frakcji
        }
        else
        {
            Debug.Log($"Utworzono nową chorągiew: {inputUnitName.text}");
        }
        // TODO: wyczyszczenie formularza albo powrót do głownego widoku
    }
}
