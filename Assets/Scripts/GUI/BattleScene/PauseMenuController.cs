using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Referencje UI")]
    public GameObject pausePanel;
    public Button btnSaveBattle;
    public Button btnEndBattle;
    public Button btnExitToMenu;

    private bool isPaused = false;

    void Start()
    {
        // Domyślnie ukrywamy panel na starcie
        if (pausePanel != null) pausePanel.SetActive(false);

        // Podpięcie logiki pod przyciski
        if (btnSaveBattle != null) btnSaveBattle.onClick.AddListener(SaveGame);
        if (btnEndBattle != null) btnEndBattle.onClick.AddListener(EndBattle);
        if (btnExitToMenu != null) btnExitToMenu.onClick.AddListener(ExitToMenu);
    }

    void Update()
    {
        // Nasłuchiwanie klawisza ESC z nowego Input Systemu
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        // Opcjonalnie: Zatrzymanie upływu czasu (jeśli masz jakieś animacje lub fizykę)
        // Time.timeScale = isPaused ? 0f : 1f;
    }

    private void SaveGame()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SaveCurrentBattle(false);
            Debug.Log("[PauseMenu] Bitwa została ręcznie zapisana.");

            // Po zapisie możemy ukryć menu, by gracz mógł grać dalej
            TogglePauseMenu();
        }
    }

    private void EndBattle()
    {
        if (BattleManager.Instance != null)
        {
            // Metoda ta wygeneruje raport i sama przerzuci gracza do MenuScene
            BattleManager.Instance.EndBattleAndGenerateReport();
        }
    }

    private void ExitToMenu()
    {
        // Upewniamy się, że czas wraca do normy przed wyjściem (jeśli użyliśmy Time.timeScale)
        // Time.timeScale = 1f;

        Debug.Log("[PauseMenu] Wychodzenie do menu bez zapisywania...");
        SceneManager.LoadScene("MenuScene");
    }
}
