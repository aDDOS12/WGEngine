using UnityEngine;

public interface IModifiable
{
    void AddModifier(StatModifier modifier);
    void RemoveModifier(string modifierId);
    void ClearAllModifiers();
    public void ApplyCustomModifier(string customName, int customAttack, float customMorale);
}
