using UnityEngine;

public class CombatOrder
{
    public UnitToken SourceUnit;
    public UnitToken TargetUnit; // może być nullem przy zwykłym rozkazie ruchu
    public Vector3 TargetPosition;
    public bool IsRangedAttack;
    public float Distance;
}
