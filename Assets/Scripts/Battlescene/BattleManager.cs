using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Silnik Walki")]
    public List<CombatOrder> PendingOrders { get; private set; } = new List<CombatOrder>();
    private CombatEngine combatEngine = new CombatEngine();

    [Header("Ustawienia Animacji")]
    public float movementDuration = 1.0f;

    private List<UnitToken> activeUnitsOnBoard = new List<UnitToken>();

    public event Action<UnitToken> OnUnitSelected;
    public event Action OnUnitDeselected;
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
                Debug.Log("Mechaniki Deploymentu zostały zablokowane");
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
            tokenScript.SetTextDirection(Vector2.up);

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

        // Faza 1: Rozpoczęcie ruchu dla wszystkich jednostek z rozkazami
        foreach (var order in PendingOrders)
        {
            Vector2 direction = (order.TargetPosition - order.SourceUnit.transform.position).normalized;
            if (direction != Vector2.zero) order.SourceUnit.SetTextDirection(direction);

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

        OnTurnEnded?.Invoke();
    }

    private void RefreshAllUnitsVisuals()
    {
        UnitToken[] allTokens = FindObjectsOfType<UnitToken>();

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
}
