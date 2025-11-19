using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.EntityConditions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

public sealed partial class HeartDamageConditionSystem : EntityConditionSystem<HeartrateComponent, HeartDamageCondition>
{
    protected override void Condition(Entity<HeartrateComponent> entity, ref EntityConditionEvent<HeartDamageCondition> args)
    {
        var damage = entity.Comp.Damage;

        args.Result = damage >= args.Condition.Min && damage <= args.Condition.Max;
    }
}

public sealed partial class HeartDamageCondition : EntityConditionBase<HeartDamageCondition>
{
    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        return Loc.GetString("reagent-effect-condition-guidebook-heart-damage",
            ("max", Max == FixedPoint2.MaxValue ? (float) int.MaxValue : Max.Float()),
            ("min", Min.Float()));
    }
}
