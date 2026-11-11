
public class PenetrationAbility : SpecialAbility
{
    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);
        unit.HasIgnoreDefenseType = true;
    }
}
