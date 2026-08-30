using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactionRow : MonoBehaviour
{
    [Header("Referencje UI")]
    public TMP_Dropdown defenderDropdown;
    public TMP_Dropdown attackerDropdown;

    public void Initialize(List<string> availableFactions)
    {
        defenderDropdown.ClearOptions();
        attackerDropdown.ClearOptions();

        List<string> options = new List<string> { "Brak" };
        options.AddRange(availableFactions);

        defenderDropdown.AddOptions(options);
        attackerDropdown.AddOptions(options);
    }

    public string GetDefender() => defenderDropdown.options[defenderDropdown.value].text;
    public string GetAttacker() => attackerDropdown.options[attackerDropdown.value].text;
}
