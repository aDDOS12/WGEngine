public interface IModifiable
{
    void AddModifier(StatModifier modifier);
    void RemoveModifier(string modifierId);
    void ClearAllModifiers();
}
