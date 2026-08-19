using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class ModifierEditorController : MonoBehaviour
{
    [Header("Formularz Modyfikatora")]
    public TMP_InputField inputDisplayName;
    public TMP_InputField inputAttackBonus;
    public TMP_InputField inputDefenceBonus;
    public TMP_InputField inputRangedAttackBonus;
    public TMP_InputField inputRangedDefenceBonus;
    public TMP_InputField inputMobilityBonus;
    public TMP_InputField inputMoraleBonus; // Float
    public TMP_InputField inputRangeBonus;

    [Header("Lista Modyfikatorów")]
    public Transform modifierListContent;
    public GameObject modifierListItemPrefab;

    [Header("Przyciski Nawigacji")]
    public Button btnSaveModifier;
    public Button btnDiscardChanges;
    public Button btnCreateNewModifier;
    public Button btnDeleteModifier;

    [Header("Stan Edytora (Logika)")]
    private bool isEditingMode = false;
    private StatModifier currentEditedModifier = null;

    private void Awake()
    {
        TemplateManager.Instance.LoadTemplates();
    }
    void Start()
    {
        btnSaveModifier.onClick.AddListener(SaveModifier);
        btnDiscardChanges.onClick.AddListener(ClearForm);

        btnCreateNewModifier.onClick.AddListener(() =>
        {
            isEditingMode = false;
            currentEditedModifier = null;
            ClearForm();
        });

        btnDeleteModifier.onClick.AddListener(DeleteCurrentModifier);

        RefreshModifierList();
        ClearForm();
    }

    private void ClearForm()
    {
        inputDisplayName.text = "";
        inputAttackBonus.text = "0";
        inputDefenceBonus.text = "0";
        inputRangedAttackBonus.text = "0";
        inputRangedDefenceBonus.text = "0";
        inputMobilityBonus.text = "0";
        inputMoraleBonus.text = "0";
        inputRangeBonus.text = "0";
    }

    private void RefreshModifierList()
    {
        foreach (Transform child in modifierListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var modifier in TemplateManager.Instance.ModifierTemplates.Values)
        {
            GameObject newItem = Instantiate(modifierListItemPrefab, modifierListContent);

            TMP_Text buttonText = newItem.GetComponentInChildren<TMP_Text>();
            if (buttonText != null )
            {
                buttonText.text = string.IsNullOrEmpty(modifier.DisplayName) ? "Nowy Modyfikator" : modifier.DisplayName;
            }

            Button itemButton = newItem.GetComponent<Button>();
            if (itemButton != null)
            {
                StatModifier modToLoad = modifier;
                itemButton.onClick.AddListener(() => { LoadModifierToEditor(modToLoad); });
            }
        }
    }

    public void LoadModifierToEditor(StatModifier modifier)
    {
        isEditingMode = true;
        currentEditedModifier = modifier;

        inputDisplayName.text = modifier.DisplayName;
        inputAttackBonus.text = modifier.AttackBonus.ToString();
        inputDefenceBonus.text = modifier.DefenceBonus.ToString();
        inputRangedAttackBonus.text = modifier.RangedAttackBonus.ToString();
        inputRangedDefenceBonus.text = modifier.RangedDefenceBonus.ToString();
        inputMobilityBonus.text = modifier.MobilityBonus.ToString();
        inputMoraleBonus.text = modifier.MoraleBonus.ToString("F1");
        inputRangeBonus.text = modifier.RangeBonus.ToString();
    }

    public void SaveModifier()
    {
        if (!isEditingMode || currentEditedModifier == null)
        {
            currentEditedModifier = new StatModifier
            {
                Id = System.Guid.NewGuid().ToString()
            };
        }

        currentEditedModifier.DisplayName = inputDisplayName.text;

        currentEditedModifier.AttackBonus = int.TryParse(inputAttackBonus.text, out int att) ? att : 0;
        currentEditedModifier.DefenceBonus = int.TryParse(inputDefenceBonus.text, out int def) ? def : 0;
        currentEditedModifier.RangedAttackBonus = int.TryParse(inputRangedAttackBonus.text, out int rAtt) ? rAtt : 0;
        currentEditedModifier.RangedDefenceBonus = int.TryParse(inputRangedDefenceBonus.text, out int rDef) ? rDef : 0;
        currentEditedModifier.MobilityBonus = int.TryParse(inputMobilityBonus.text, out int mob) ? mob : 0;

        currentEditedModifier.MoraleBonus = float.TryParse(inputMoraleBonus.text, out float mor) ? mor : 0f;

        currentEditedModifier.RangeBonus = int.TryParse(inputRangeBonus.text, out int rBuff) ? rBuff : 0;

        TemplateManager.Instance.ModifierTemplates[currentEditedModifier.Id] = currentEditedModifier;

        TemplateManager.Instance.SaveTemplates();

        if (isEditingMode)
        {
            Debug.Log($"Zaktualizowano modyfikator: {currentEditedModifier.DisplayName}");
        }
        else
        {
            Debug.Log($"Utworzono nowy modyfikator: {currentEditedModifier.DisplayName}");
        }

        RefreshModifierList();
    }

    private void DeleteCurrentModifier()
    {
        if (isEditingMode && currentEditedModifier != null)
        {
            TemplateManager.Instance.ModifierTemplates.Remove(currentEditedModifier.Id);
            TemplateManager.Instance.SaveTemplates();

            Debug.Log($"Usunięto modyfikator: {currentEditedModifier.DisplayName}");

            RefreshModifierList();
            ClearForm();
            isEditingMode = false;
            currentEditedModifier = null;
        }
    }
}
