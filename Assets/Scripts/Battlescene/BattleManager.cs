using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public List<UnitToken> SelectedUnits { get; private set; } = new List<UnitToken>();
    public BattlePhase CurrentPhase { get; private set; }

    [Header("Referencje Planszy")]
    public Transform tokenContainer;

    [Header("Prefaby")]
    public GameObject unitTokenPrefab;

    [Header("Referencje UI")]
    public Button startCombatButton;
    public GameObject startCombatPanel;

    [Header("Silnik Walki")]
    public List<CombatOrder> PendingOrders { get; private set; } = new List<CombatOrder>();
    private CombatEngine combatEngine = new CombatEngine();

    [Header("Ustawienia Animacji")]
    public float movementDuration = 1.0f;

    [Header("Historia Bitwy")]
    public int CurrentTurn { get; private set; } = 1;
    public List<TurnLog> BattleHistory { get; private set; } = new List<TurnLog>();
    private List<string> _currentTurnLogs = new List<string>();

    private List<UnitToken> activeUnitsOnBoard = new List<UnitToken>();

    public event Action OnSelectionChanged;
    public event Action<BattlePhase> OnPhaseChanged;
    public event Action OnTurnStarted;
    public event Action OnTurnEnded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (startCombatButton != null)
        {
            startCombatButton.onClick.AddListener(() => ChangePhase(BattlePhase.Combat));
        }

        ChangePhase(BattlePhase.Initialization);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            LoadBattleFromSave("autosave.json");
        }
#endif

        if (CurrentPhase == BattlePhase.Deployment)
        {
            if (Keyboard.current.xKey.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
            {
                RemoveSelectedUnits();
            }
        }
    }

    public void ChangePhase(BattlePhase newPhase)
    {
        if (CurrentPhase == newPhase) return; 

        CurrentPhase = newPhase;
        Debug.Log($"[BattleManager] Zmiana fazy na {CurrentPhase}");

        switch (CurrentPhase)
        {
            case BattlePhase.Initialization:
                ClearBoard();
                break;
            case BattlePhase.Deployment:
                PrepareDeployment();
                break;
            case BattlePhase.Combat:
                ClearSelection(true);
                if (startCombatPanel != null) startCombatPanel.SetActive(false);
                Debug.Log("Mechaniki Deploymentu zostały zablokowane");
                SaveCurrentBattle(true);
                break;
        }

        OnPhaseChanged?.Invoke(CurrentPhase);
    }

    private void PrepareDeployment()
    {
        var config = DataManager.Instance.CurrentBattleConfig;
        if (config == null)
        {
            Debug.LogError("[BattleManager] Błąd krytyczny: Próba rozpoczęcia Deployment bez konfiguracji bitwy!");
            return;
        }

        // Wypełnienie bocznych paneli jednostkami dostępnymi dla wybranych frakcji
        if (DeploymentUIManager.Instance != null)
        {
            DeploymentUIManager.Instance.PopulateLists(config);
        }
        else
        {
            Debug.LogWarning("[BattleManager] Brak instancji DeploymentUIManager na scenie.");
        }
    }

    private void ClearBoard()
    {
        activeUnitsOnBoard.Clear();
        if (tokenContainer != null)
        {
            foreach (Transform child in tokenContainer) Destroy(child.gameObject);
        }
    }

    public void SelectUnit(UnitToken unit, bool clearPrevious = true)
    {
        if (clearPrevious)
        {
            ClearSelection();
        }

        if (!SelectedUnits.Contains(unit))
        {
            SelectedUnits.Add(unit);
        }

        if (clearPrevious)
        {
            NotifySelectionChanged();
        }
    }

    private void RemoveSelectedUnits()
    {
        if (SelectedUnits.Count == 0) return;

        foreach (var token in SelectedUnits)
        {
            // 1. Usuwamy z lokalnej listy planszy
            activeUnitsOnBoard.Remove(token);

            // 2. Usuwamy z globalnego rejestru DataManagera, by uniknąć NullReferenceException
            if (DataManager.Instance != null && DataManager.Instance.ActiveUnits.Contains(token.UnitData))
            {
                DataManager.Instance.ActiveUnits.Remove(token.UnitData);
            }

            // 3. Fizycznie niszczymy obiekt na scenie
            Destroy(token.gameObject);
        }

        // 4. Czyścimy selekcję i wysyłamy sygnał do odświeżenia UI (np. wyczyszczenia panelu statystyk)
        ClearSelection(true);

        Debug.Log("[BattleManager] Zaznaczone jednostki zostały usunięte z planszy.");
    }

    public void ClearSelection(bool notify = true)
    {
        SelectedUnits.Clear();
        if (notify) NotifySelectionChanged();
    }

    public void NotifySelectionChanged()
    {
        OnSelectionChanged?.Invoke();
    }

    public void SpawnUnitOnBoard(Unit template, Color factionColor, Vector3 position)
    {
        if (unitTokenPrefab == null)
        {
            Debug.LogError("[BattleManager] Brak przypisanego prefabu UnitToken!");
            return;
        }

        Unit newUnit = template.CloneUnit();

        GameObject tokenObj = Instantiate(unitTokenPrefab, position, Quaternion.identity, tokenContainer);
        tokenObj.name = $"Token_{newUnit.UnitName}";

        UnitToken tokenScript = tokenObj.GetComponent<UnitToken>();
        if (tokenScript != null)
        {
            tokenScript.InitializeUnit(newUnit, newUnit.VisualData, factionColor);
            tokenScript.SetFacingDirection(Vector2.up);

            activeUnitsOnBoard.Add(tokenScript);

            SelectUnit(tokenScript);
        }
    }

    public void AlignSelectedToHorizontalRow()
    {
        if (SelectedUnits.Count < 2) return;

        // Bierzemy oś Y od pierwszej zaznaczonej jednostki
        float targetY = SelectedUnits[0].transform.position.y;

        foreach (var token in SelectedUnits)
        {
            Vector3 newPos = token.transform.position;
            newPos.y = targetY;
            token.transform.position = newPos;
        }
    }

    public void AlignSelectedToVerticalColumn()
    {
        if (SelectedUnits.Count < 2) return;

        // Bierzemy oś X od pierwszej zaznaczonej jednostki
        float targetX = SelectedUnits[0].transform.position.x;

        foreach (var token in SelectedUnits)
        {
            Vector3 newPos = token.transform.position;
            newPos.x = targetX;
            token.transform.position = newPos;
        }
    }

    public void RegisterOrder(CombatOrder order)
    {
        // Usuwamy stary rozkaz, jeśli gracz zmienił zdanie co do ruchu tej jednostki
        PendingOrders.RemoveAll(o => o.SourceUnit == order.SourceUnit);
        if (order != null) PendingOrders.Add(order);
    }

    public void RemoveOrder(UnitToken unit)
    {
        PendingOrders.RemoveAll(o => o.SourceUnit == unit);
    }

    public void ResolveCurrentTurn()
    {
        if (CurrentPhase != BattlePhase.Combat) return;

        Debug.Log($"[BattleManager] Przetwarzanie tury. Ilość rozkazów: {PendingOrders.Count}");

        StartCoroutine(ResolveTurnRoutine());
    }

    private IEnumerator ResolveTurnRoutine()
    {
        // Zablokowanie interfejsu
        OnTurnStarted?.Invoke();

        ProcessRoutingUnits();

        // Faza 1: Rozpoczęcie ruchu dla wszystkich jednostek z rozkazami
        foreach (var order in PendingOrders)
        {
            Vector2 direction = (order.TargetPosition - order.SourceUnit.transform.position).normalized;
            if (direction != Vector2.zero) order.SourceUnit.SetFacingDirection(direction);

            // Uruchamiamy korutynę na każdym żetonie
            order.SourceUnit.StartCoroutine(order.SourceUnit.MoveToPosition(order.TargetPosition, movementDuration));
        }

        // Faza 2: Oczekujemy określoną ilość czasu, aż wszystkie żetony dojadą
        yield return new WaitForSeconds(movementDuration);

        // Faza 3: Rozstrzygnięcie walki
        foreach (var order in PendingOrders)
        {
            if (order.TargetUnit != null)
            {
                EngagementType type = order.IsRangedAttack ? EngagementType.Fire : EngagementType.Melee;

                combatEngine.ResolveEngagement(
                    order.SourceUnit.UnitData, // Atakujący
                    order.TargetUnit.UnitData, // Obrońca
                    type,                      // Typ starcia
                    order.Distance             // Dystans
                );
            }
        }

        PendingOrders.Clear();
        ClearSelection(true);

        // TODO: Odświeżenie UI wszystkich jednostek
        Debug.Log("[BattleManager] Tura zakończona. Obrażenia zostały przeliczone.");

        RefreshAllUnitsVisuals();

        BattleHistory.Add(new TurnLog { TurnNumber = this.CurrentTurn, CombatEvents = new List<string>(_currentTurnLogs) });
        _currentTurnLogs.Clear();
        CurrentTurn++; // Zwiększamy numer tury

        SaveCurrentBattle(true);
        OnTurnEnded?.Invoke();
    }

    private void RefreshAllUnitsVisuals()
    {
        UnitToken[] allTokens = FindObjectsByType<UnitToken>(FindObjectsInactive.Exclude);

        foreach (var token in allTokens)
        {
            token.UpdateVisual();

            if (token.UnitData.SoldierCount <= 0)
            {
                Debug.Log($"[Walka] Oddział {token.UnitData.UnitName} został całkowicie zniszczony.");
                // TODO: Zamiast niszczyc jednostke mozna w przyszlosci odpalic animacje niszczenia
            }
        }
    }

    private void ProcessRoutingUnits()
    {
        UnitToken[] allTokens = FindObjectsByType<UnitToken>(FindObjectsInactive.Exclude);

        foreach (var token in allTokens)
        {
            if (token.UnitData.IsBroken)
            {
                // Czyścimy głupie pomysły gracza (gdyby jakoś to obszedł)
                RemoveOrder(token);

                // Bardzo prosty algorytm szukania najbliższego wroga
                UnitToken nearestEnemy = null;
                float closestDist = float.MaxValue;

                foreach (var other in allTokens)
                {
                    if (other.UnitData.Faction != token.UnitData.Faction)
                    {
                        float dist = Vector3.Distance(token.transform.position, other.transform.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            nearestEnemy = other;
                        }
                    }
                }

                // Generowanie wektora ucieczki
                if (nearestEnemy != null)
                {
                    Vector3 fleeDirection = (token.transform.position - nearestEnemy.transform.position).normalized;
                    // Uciekają na pełnej mobilności
                    Vector3 fleeTarget = token.transform.position + (fleeDirection * token.UnitData.MobilityValue);

                    RegisterOrder(new CombatOrder
                    {
                        SourceUnit = token,
                        TargetUnit = null, // Biegnie w puste pole
                        TargetPosition = fleeTarget,
                        IsRangedAttack = false,
                        Distance = token.UnitData.MobilityValue
                    });

                    Debug.Log($"[Morale] {token.UnitData.UnitName} rzuca się do ucieczki!");
                }
            }
        }
    }

    public void SaveCurrentBattle(bool isAutosave = false)
    {
        BattleSaveData saveData = new BattleSaveData();
        saveData.CurrentTurn = this.CurrentTurn;
        saveData.HistoryLogs = new List<TurnLog>(this.BattleHistory);

        UnitToken[] allTokens = FindObjectsByType<UnitToken>(FindObjectsInactive.Exclude);

        foreach (var token in allTokens)
        {
            UnitSaveData uData = new UnitSaveData
            {
                UnitName = token.UnitData.UnitName,
                FactionID = token.UnitData.Faction,
                Position = token.transform.position,
                FacingDirection = token.FacingDirection,
                CurrentSoldierCount = token.UnitData.SoldierCount,
                IsBroken = token.UnitData.IsBroken,
                StartingSoldierCount = token.UnitData.StartingSoldierCount
            };
            saveData.Units.Add(uData);
        }
        var config = DataManager.Instance.CurrentBattleConfig;
        if (config != null)
        {
            saveData.BattleId = config.BattleId;
            saveData.AttackingFactions = config.AttackingFactionIds;
            saveData.DefendingFactions = config.DefendingFactionIds;
        }

        // Pobranie nazwy z konfiguracji (zabezpieczone w razie braku configu)
        string battleName = DataManager.Instance.CurrentBattleConfig?.BattleId ?? "NieznanaBitwa";
        // Decyzja o nazwie pliku
        string fileName = isAutosave ? "autosave.json" : $"{battleName}.json";

        SaveSystem.SaveGame(fileName, saveData);
    }

    public void LoadBattleFromSave(string fileName = "autosave.json")
    {
        BattleSaveData saveData = SaveSystem.LoadGame(fileName);

        if (saveData == null)
        {
            Debug.LogError($"[BattleManager] Błąd wczytywania. Plik {fileName} nie istnieje lub jest uszkodzony.");
            return;
        }

        // ODTWORZENIE KONFIGURACJI BITWY DLA DATA MANAGERA
        DataManager.Instance.CurrentBattleConfig = new BattleConfiguration(
            string.IsNullOrEmpty(saveData.BattleId) ? "WczytanaBitwa" : saveData.BattleId,
            saveData.AttackingFactions ?? new List<string>(),
            saveData.DefendingFactions ?? new List<string>()
        );
        this.CurrentTurn = saveData.CurrentTurn;
        this.BattleHistory = saveData.HistoryLogs ?? new List<TurnLog>();
        this._currentTurnLogs.Clear();

        // 1. Zabezpieczenie przed wyciekami pamięci i błędami referencji
        ClearBattlefield();

        // 2. Odtworzenie metadanych gry
        // Jeśli masz zmienną śledzącą tury (np. CurrentTurn), nadpisz ją tutaj:
        // CurrentTurn = saveData.CurrentTurn; 

        // 3. Rekonstrukcja jednostek na scenie
        foreach (var unitData in saveData.Units)
        {
            ReconstructUnitToken(unitData);
        }

        // 4. Reset stanu UI i zmuszenie gry do odświeżenia widoków
        CurrentPhase = BattlePhase.Combat; // lub odpowiednia faza startowa
        OnTurnEnded?.Invoke(); // Wywołujemy event, żeby UI załapało nowy stan

        Debug.Log($"[BattleManager] Bitwa wczytana pomyślnie. Zrekonstruowano {saveData.Units.Count} jednostek.");
    }

    private void ClearBattlefield()
    {
        SelectedUnits.Clear();
        PendingOrders.Clear();
        DataManager.Instance.ActiveUnits.Clear();

        UnitToken[] existingTokens = FindObjectsByType<UnitToken>(FindObjectsInactive.Exclude);
        for (int i = 0; i < existingTokens.Length; i++)
        {
            Destroy(existingTokens[i].gameObject);
        }
    }

    private void ReconstructUnitToken(UnitSaveData uData)
    {
        // 1. Znalezienie oryginalnego szablonu po nazwie
        Unit baseTemplate = null;
        foreach(var template in TemplateManager.Instance.UnitTemplates.Values)
        {
            if (template.UnitName == uData.UnitName)
            {
                baseTemplate = template;
                break;
            }
        }

        if (baseTemplate == null)
        {
            Debug.LogError($"[BattleManager] Nie znaleziono bazowego szablonu dla: {uData.UnitName}");
            return;
        }

        // 2. Klonowanie jednostki przy użyciu Twojej metody
        Unit runtimeUnit = baseTemplate.CloneUnit();
        runtimeUnit.StartingSoldierCount = uData.StartingSoldierCount;
        DataManager.Instance.ActiveUnits.Add(runtimeUnit);

        // 3. Wstrzykiwanie stanu z zapisu
        runtimeUnit.SoldierCount = uData.CurrentSoldierCount;
        runtimeUnit.Faction = uData.FactionID;
        runtimeUnit.IsBroken = uData.IsBroken;

        // Zabezpieczenie na wypadek, gdyby ktoś wczytał zniszczoną jednostkę
        if (runtimeUnit.SoldierCount < 0) runtimeUnit.SoldierCount = 0;

        // 4. Instancjonowanie żetonu (od razu do odpowiedniego kontenera!)
        GameObject tokenObj = Instantiate(unitTokenPrefab, uData.Position, Quaternion.identity, tokenContainer);
        tokenObj.name = $"Token_{runtimeUnit.UnitName}";
        UnitToken tokenScript = tokenObj.GetComponent<UnitToken>();

        // 5. Pobieranie koloru frakcji na podstawie Hex Code
        Color factionColor = Color.white; // domyślny kolor

        if (TemplateManager.Instance.FactionTemplates.TryGetValue(runtimeUnit.Faction, out FactionData factionData))
        {
            if (ColorUtility.TryParseHtmlString(factionData.ColorHexCode, out Color parsedColor))
            {
                factionColor = parsedColor;
            }
        }

        // 6. Inicjalizacja. (VisualData masz od razu w obiekcie Unit!)
        tokenScript.InitializeUnit(runtimeUnit, runtimeUnit.VisualData, factionColor);
        tokenScript.SetFacingDirection(uData.FacingDirection);
        tokenScript.UpdateVisual();
    }

    public void LogCombatEvent(string message)
    {
        // Dodajemy prosty znacznik czasu, bardzo przydatne w raportach
        _currentTurnLogs.Add($"[{System.DateTime.Now:HH:mm:ss}] {message}");
    }

    public void EndBattleAndGenerateReport()
    {
        // 1. Zbieramy dokładnie taki sam obiekt jak przy zapisywaniu JSON
        BattleSaveData finalData = new BattleSaveData();

        finalData.CurrentTurn = this.CurrentTurn;
        finalData.HistoryLogs = new List<TurnLog>(this.BattleHistory);

        var config = DataManager.Instance.CurrentBattleConfig;
        if (config != null)
        {
            finalData.BattleId = config.BattleId;
            finalData.AttackingFactions = config.AttackingFactionIds;
            finalData.DefendingFactions = config.DefendingFactionIds;
        }

        UnitToken[] allTokens = FindObjectsByType<UnitToken>(FindObjectsInactive.Exclude);
        foreach (var token in allTokens)
        {
            UnitSaveData uData = new UnitSaveData
            {
                UnitName = token.UnitData.UnitName,
                FactionID = token.UnitData.Faction,
                StartingSoldierCount = token.UnitData.StartingSoldierCount,
                CurrentSoldierCount = token.UnitData.SoldierCount,
                IsBroken = token.UnitData.IsBroken
                // Pozycje nas tutaj nie obchodzą, bo to tylko do raportu
            };
            finalData.Units.Add(uData);
        }

        // 2. Przekazujemy zebrane dane do naszego generatora
        BattleReportGenerator.GenerateReport(finalData);

        // 3. Po zrobieniu raportu z bitwy, autozapis jest już niepotrzebny/przestarzały
        DataManager.Instance.DeleteSaveFile("autosave.json");

        Debug.Log("[BattleManager] Zakończono bitwę i wygenerowano logi.");

        SceneManager.LoadScene("MenuScene");
    }
}
