using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.EntityConditions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

public sealed partial class LungDamageConditionSystem : EntityConditionSystem<LungDamageComponent, LungDamageCondition>
{
    protected override void Condition(Entity<LungDamageComponent> entity, ref EntityConditionEvent<LungDamageCondition> args)
    {
        var damage = entity.Comp.Damage;

        args.Result = damage >= args.Condition.Min && damage <= args.Condition.Max;
    }
}

public sealed partial class LungDamageCondition : EntityConditionBase<LungDamageCondition>
{
    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        return Loc.GetString("reagent-effect-condition-guidebook-lung-damage",
            ("max", Max == FixedPoint2.MaxValue ? (float) int.MaxValue : Max.Float()),
            ("min", Min.Float()));
    }
}

