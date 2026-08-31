using System;
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

    private void ShowModifiers()
    {
        unitListPanel.SetActive(false);
        modifierPanel.SetActive(true);

        btnUnits.interactable = true;
        btnModifiers.interactable = false;
    }

    private void ShowUnitList()
    {
        unitListPanel.SetActive(true);
        modifierPanel.SetActive(false);

        btnUnits.interactable = false;
        btnModifiers.interactable = true;
    }
}
