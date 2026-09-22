using UnityEngine;
using UnityEngine.UI;

public class DeploymentTabController : MonoBehaviour
{
    [Header("Panele")]
    public GameObject unitListPanel;
    public GameObject modifierPanel;

    [Header("Przyciski")]
    public Button btnUnits;
    public Button btnModifiers;

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPhaseChanged += HandlePhaseChanged;
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
        }
    }

    void Start()
    {
        if (btnUnits != null)
        {
            btnUnits.onClick.AddListener(ShowUnitList);
        }

        if (btnModifiers != null)
        {
            btnModifiers.onClick.AddListener(ShowModifiers);
        }

        ShowUnitList();
    }

    private void HandlePhaseChanged(BattlePhase newPhase)
    {
        if (newPhase == BattlePhase.Combat)
        {
            ShowModifiers();

            if (btnUnits != null)
            {
                btnUnits.interactable = false;
            }
        }
        else if (newPhase == BattlePhase.Initialization || newPhase == BattlePhase.Deployment)
        {
            if (btnUnits != null) btnUnits.interactable = true;
        }
    }

    private void ShowModifiers()
    {
        if (unitListPanel != null) unitListPanel.SetActive(false);
        if (modifierPanel != null) modifierPanel.SetActive(true);

        if (btnUnits != null) btnUnits.interactable = true;
        if (btnModifiers != null) btnModifiers.interactable = false;
    }

    private void ShowUnitList()
    {
        if (unitListPanel != null) unitListPanel.SetActive(true);
        if (modifierPanel != null) modifierPanel.SetActive(false);

        if (btnUnits != null) btnUnits.interactable = false;
        if (btnModifiers != null) btnModifiers.interactable = true;
    }
}
