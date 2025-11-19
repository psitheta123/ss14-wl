using Content.Shared._Offbrand.Wounds;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.EntityConditions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

public sealed partial class TotalGroupDamageConditionSystem : EntityConditionSystem<DamageableComponent, TotalGroupDamageCondition>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Condition(Entity<DamageableComponent> entity, ref EntityConditionEvent<TotalGroupDamageCondition> args)
    {
        var prototype = IoCManager.Resolve<IPrototypeManager>();
        var group = prototype.Index(args.Condition.Group);

        var total = FixedPoint2.Zero;
        entity.Comp.Damage.TryGetDamageInGroup(group, out total);
        args.Result = total >= args.Condition.Min && total <= args.Condition.Max;
    }
}

public sealed partial class TotalGroupDamageCondition : EntityConditionBase<TotalGroupDamageCondition>
{
    [DataField(required: true)]
    public ProtoId<DamageGroupPrototype> Group;

    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;


    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        return Loc.GetString("reagent-effect-condition-guidebook-total-group-damage",
            ("max", Max == FixedPoint2.MaxValue ? (float) int.MaxValue : Max.Float()),
            ("min", Min.Float()),
            ("name", prototype.Index(Group).LocalizedName));
    }
}
