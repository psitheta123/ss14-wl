using Content.Server.Body.Components;
using Content.Shared._Offbrand.EntityEffects;
using Content.Shared.Chemistry.Components;
using Content.Shared.EntityEffects;
using Content.Shared.EntityConditions;
using Content.Shared.FixedPoint;

namespace Content.Server._Offbrand.EntityEffects;

public sealed class MetaboliteThresholdSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MetabolizerComponent, EntityConditionEvent<MetaboliteCondition>>(OnCheckMetaboliteThreshold);
    }

    private void OnCheckMetaboliteThreshold(Entity<MetabolizerComponent> entity, ref EntityConditionEvent<MetaboliteCondition> args)
    {
        var reagent = args.Condition.Reagent;
        if (reagent == null)
            return;

        if (reagent is not { } metaboliteReagent)
            return;

        if (!TryComp<MetabolizerComponent>(entity, out var metabolizer))
            return;

        if (!TryComp<SolutionComponent>(entity, out var solution))
            return;

        var metabolites = metabolizer.Metabolites;

        var quant = FixedPoint2.Zero;
        metabolites.TryGetValue(metaboliteReagent, out quant);

        if (args.Condition.IncludeBloodstream && solution.Solution != null)
        {
            quant += solution.Solution.GetTotalPrototypeQuantity(metaboliteReagent);
        }

        args.Result = quant >= args.Condition.Min && quant <= args.Condition.Max;
    }
}
