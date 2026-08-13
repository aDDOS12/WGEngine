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
    void Start()
    {
        ShowStatsSection(); // show statsSection as default

        // TODO: Add listeners
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
}
