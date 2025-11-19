using Content.Shared.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.EntityConditions;

namespace Content.Shared._Offbrand.EntityEffects;

public sealed partial class MetaboliteConditionSystem : EntityConditionSystem<SolutionComponent, MetaboliteCondition>
{
    protected override void Condition(Entity<SolutionComponent> entity, ref EntityConditionEvent<MetaboliteCondition> args)
    { }
}

public sealed partial class MetaboliteCondition : EntityConditionBase<MetaboliteCondition>
{
    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField]
    public ProtoId<ReagentPrototype>? Reagent;

    [DataField]
    public bool IncludeBloodstream = true;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        ReagentPrototype? reagentProto = null;
        if (Reagent is { } reagent)
            prototype.TryIndex(reagent, out reagentProto);

        if (IncludeBloodstream)
        {
            return Loc.GetString("reagent-effect-condition-guidebook-total-dosage-threshold",
                ("reagent", reagentProto?.LocalizedName ?? Loc.GetString("reagent-effect-condition-guidebook-this-reagent")),
                ("max", Max == FixedPoint2.MaxValue ? (float) int.MaxValue : Max.Float()),
                ("min", Min.Float()));
        }
        else
        {
            return Loc.GetString("reagent-effect-condition-guidebook-metabolite-threshold",
                ("reagent", reagentProto?.LocalizedName ?? Loc.GetString("reagent-effect-condition-guidebook-this-metabolite")),
                ("max", Max == FixedPoint2.MaxValue ? (float) int.MaxValue : Max.Float()),
                ("min", Min.Float()));
        }
    }
}
