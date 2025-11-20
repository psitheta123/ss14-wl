using Content.Server.Body.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.EntityEffects;
using Content.Shared.EntityConditions;
using Content.Shared.FixedPoint;
using Content.Shared._Offbrand.EntityEffects;

namespace Content.Server._Offbrand.EntityEffects.Effects;

/// <summary>
/// This effect adjusts a respirator's saturation value.
/// The saturation adjustment is modified by scale.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class MetaboliteConditionEntitySystem : EntityConditionSystem<MetabolizerComponent, MetaboliteCondition>
{
    protected override void Condition(Entity<MetabolizerComponent> entity, ref EntityConditionEvent<MetaboliteCondition> args)
    {
        var reagent = args.Condition.Reagent;
        if (reagent == null)
            return;

        if (reagent is not { } metaboliteReagent)
            return;

        if (!TryComp<SolutionComponent>(entity, out var solution))
            return;

        var metabolites = entity.Comp.Metabolites;

        var quant = FixedPoint2.Zero;
        metabolites.TryGetValue(metaboliteReagent, out quant);

        if (args.Condition.IncludeBloodstream && solution.Solution != null)
        {
            quant += solution.Solution.GetTotalPrototypeQuantity(metaboliteReagent);
        }

        args.Result = quant >= args.Condition.Min && quant <= args.Condition.Max;

        Logger.Debug("STEET");
        Logger.Debug(args.Result.ToString());
    }
}
