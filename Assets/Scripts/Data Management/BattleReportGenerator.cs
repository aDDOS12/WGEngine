using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class BattleReportGenerator
{
    public static void GenerateReport(BattleSaveData finalData)
    {
        string directoryPath = Application.persistentDataPath + "/Reports/";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Unikalna nazwa pliku z datą (żeby raporty z tej samej bitwy się nie nadpisywały)
        string fileName = $"{finalData.BattleId}_{DateTime.Now:yyyyMMdd_HHmm}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        StringBuilder sb = new StringBuilder();

        // ==========================================
        // SEKCJA 1: INFORMACJE OGÓLNE
        // ==========================================
        sb.AppendLine("==================================================");
        sb.AppendLine($" RAPORT BITEWNY: {finalData.BattleId}");
        sb.AppendLine($" Data wygenerowania: {DateTime.Now:yyyy-MM-dd HH:mm}");
        sb.AppendLine($" Rozegranych tur: {finalData.CurrentTurn - 1}");
        sb.AppendLine("==================================================\n");

        sb.AppendLine("--- STRONY KONFLIKTU ---");

        // Podział jednostek na frakcje
        var unitsByFaction = finalData.Units.GroupBy(u => u.FactionID)
                                            .ToDictionary(g => g.Key, g => g.ToList());

        int totalAttackers = 0;
        int totalDefenders = 0;

        foreach(var unit in finalData.Units)
        {
            if (finalData.AttackingFactions.Contains(unit.FactionID)) totalAttackers += unit.StartingSoldierCount;
            if (finalData.DefendingFactions.Contains(unit.FactionID)) totalDefenders += unit.StartingSoldierCount;
        }

        sb.AppendLine($"ATAKUJĄCY ({string.Join(", ", finalData.AttackingFactions)}) - Razem żołnierzy: {totalAttackers}");
        sb.AppendLine($"OBROŃCY ({string.Join(", ", finalData.DefendingFactions)}) - Razem żołnierzy: {totalDefenders}\n");
        sb.AppendLine("--- ORDE DE BATAILLE (Stan Początkowy) ---");

        foreach (var factionKvp in unitsByFaction)
        {
            sb.AppendLine($"Frakcja: {factionKvp.Key}");
            foreach (var unit in factionKvp.Value)
            {
                sb.AppendLine($"  - {unit.UnitName}: {unit.StartingSoldierCount} żołnierzy");
            }
        }
        sb.AppendLine();

        // ==========================================
        // SEKCJA 2: HISTORIA TUR
        // ==========================================
        sb.AppendLine("==================================================");
        sb.AppendLine(" PRZEBIEG BITWY (LOGI ZDARZEŃ)");
        sb.AppendLine("==================================================");

        if (finalData.HistoryLogs == null || finalData.HistoryLogs.Count == 0)
        {
            sb.AppendLine("Brak zarejestrowanych starć.");
        }
        else
        {
            foreach (var turn in finalData.HistoryLogs)
            {
                sb.AppendLine($"\n--- TURA {turn.TurnNumber} ---");
                if (turn.CombatEvents.Count == 0)
                {
                    sb.AppendLine(" Brak znaczących zdarzeń.");
                }
                else
                {
                    foreach (var log in turn.CombatEvents)
                    {
                        sb.AppendLine($" {log}");
                    }
                }
            }
        }
        sb.AppendLine();

        // ==========================================
        // SEKCJA 3: PODSUMOWANIE I STRATY
        // ==========================================
        sb.AppendLine("==================================================");
        sb.AppendLine(" PODSUMOWANIE STRAT");
        sb.AppendLine("==================================================");

        int totalGlobalLost = 0;

        foreach (var factionKvp in unitsByFaction)
        {
            string factionName = factionKvp.Key;
            List<UnitSaveData> factionUnits = factionKvp.Value;

            int factionStarting = factionUnits.Sum(u => u.StartingSoldierCount);
            int factionCurrent = factionUnits.Sum(u => u.CurrentSoldierCount);
            int factionTotalLost = factionStarting - factionCurrent;
            totalGlobalLost += factionTotalLost;

            // Wyliczanie szczegółowe strat z zachowaniem ułamków na korzyść lekko rannych
            int factionKIA = factionTotalLost / 3;
            int factionSeverelyWounded = factionTotalLost / 3;
            int factionLightlyWounded = factionTotalLost - factionKIA - factionSeverelyWounded;

            sb.AppendLine($"\n>>> FRAKCJA: {factionName} <<<");
            sb.AppendLine($"Wystawiono: {factionStarting} | Przetrwało w boju: {factionCurrent} | Suma strat: {factionTotalLost}");
            sb.AppendLine($" - Polegli (KIA): {factionKIA}");
            sb.AppendLine($" - Ciężko ranni (Odesłani na tyły): {factionSeverelyWounded}");
            sb.AppendLine($" - Lekko ranni (Zdolni do dalszej walki): {factionLightlyWounded}\n");

            sb.AppendLine(" Szczegóły chorągwi:");
            foreach (var unit in factionUnits)
            {
                int unitLost = unit.StartingSoldierCount - unit.CurrentSoldierCount;
                int unitKIA = unitLost / 3;
                int unitSW = unitLost / 3;
                int unitLW = unitLost - unitKIA - unitSW;

                string moraleStatus = unit.IsBroken ? "[ZŁAMANE MORALE]" : "[W PORZĄDKU]";
                string destroyedStatus = unit.CurrentSoldierCount == 0 ? "[ZNISZCZONA]" : "";

                sb.AppendLine($"  * {unit.UnitName} {destroyedStatus} {moraleStatus}");
                sb.AppendLine($"    Straty: {unitLost} (Zabici: {unitKIA}, Ciężko ranni: {unitSW}, Lekko ranni: {unitLW})");
            }
        }

        sb.AppendLine("==================================================");
        sb.AppendLine($" ŁĄCZNE STRATY OBU STRON: {totalGlobalLost} żołnierzy.");
        sb.AppendLine("==================================================");

        // Zapis do pliku
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[Raport] Wygenerowano raport bitewny: {filePath}");
    }
}
