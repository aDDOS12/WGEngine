using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIController : MonoBehaviour
{
    [Header("Elementy UI")]
    public Button nextTurnButton;

    private void OnEnable()
    {
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(OnNextTurnClicked);
        }

        // Subskrypcja zdarzeń blokujących UI (zapobiega spamowaniu przycisku)
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnTurnStarted += LockUI;
            BattleManager.Instance.OnTurnEnded += UnlockUI;
        }
    }

    private void OnDisable()
    {
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.RemoveListener(OnNextTurnClicked);
        }

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnTurnStarted -= LockUI;
            BattleManager.Instance.OnTurnEnded -= UnlockUI;
        }
    }

    private void OnNextTurnClicked()
    {
        // UI tylko deleguje żądanie, nie sprawdza logiki gry
        BattleManager.Instance.ResolveCurrentTurn();
    }

    private void LockUI()
    {
        if (nextTurnButton != null) nextTurnButton.interactable = false;
    }

    private void UnlockUI()
    {
        if (nextTurnButton != null) nextTurnButton.interactable = true;
    }
}
