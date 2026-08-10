using UnityEngine;

public class ModifierTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("<color=cyan>--- TEST MODYFIKATORÓW JEDNOSTKI ---</color>");

        Unit knight = new Unit()
        {
            Id = "knight_test",
            UnitName = "Ciężka Kawaleria",
            BaseAttack = 100,
            BaseDefence = 80,
            BaseMorale = 60
        };

        Debug.Log($"STAN POCZĄTKOWY, Atak: {knight.Attack}, Obrona: {knight.Defence}, Morale: {knight.Morale}");

        StatModifier chargeModifier = new StatModifier
        {
            Id = "mod_cav_charge",
            DisplayName = "Szarża",
            AttackBonus = 50,
            DefenceBonus = -20,
            MoraleBonus = 15
        };

        Debug.Log("<color=orange>--- DODANIE MODYFIKATORA: Szarża ---</color>");
        knight.AddModifier(chargeModifier);

        Debug.Log($"STAN PO ZAAPLIKOWANIA MODYFIKAORA, Atak: {knight.Attack}, Obrona: {knight.Defence}, Morale: {knight.Morale}");
        Debug.Log($"Weryfikacja: Atak: 150, Obrona: 60, Morale: 75");

        Debug.Log("<color=yellow>--- PRÓBA ZDUPLIKOWANIA MODYFIKATORA ---</color>");
        knight.AddModifier(chargeModifier);

        Debug.Log($"Weryfikacja: Atak: {knight.Attack}, Obrona: {knight.Defence}, Morale: {knight.Morale}");
        Debug.Log($"Oczekiwane: Atak: 150, Obrona: 60, Morale: 75");

        Debug.Log("<color=orange>--- USUNIĘCIE MODYFIKATORA ---</color>");
        knight.RemoveModifier("mod_cav_charge");
        Debug.Log($"STAN PO USUNIĘCIU, Atak: {knight.Attack}, Obrona: {knight.Defence}, Morale: {knight.Morale}");

        knight.AddModifier(new StatModifier { Id = "mod_banner", DisplayName = "Magiczny Sztandar", AttackBonus = 10 });
        knight.AddModifier(new StatModifier { Id = "mod_fear", DisplayName = "Strach", MoraleBonus = -20 });

        Debug.Log($"Przed czyszczeniem: Aktywne modyfikatory = {knight.ActiveModifiers.Count}, Atak = {knight.Attack}, Morale = {knight.Morale}");
        Debug.Log("Weryfikacja: Atak: 110, Morale: 40");

        knight.ClearAllModifiers();

        Debug.Log($"Po wywołaniu ClearAllModifiers(): Aktywne modyfikatory = {knight.ActiveModifiers.Count}, Atak = {knight.Attack}, Morale = {knight.Morale}");
    }
}
