using Content.Shared.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._Offbrand.EntityEffects;

public sealed partial class AdjustReagentGaussianEntityEffectSystem : EntityEffectSystem<SolutionComponent, AdjustReagentGaussian>
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<SolutionComponent> entity, ref EntityEffectEvent<AdjustReagentGaussian> args)
    {
        var timing = IoCManager.Resolve<IGameTiming>();

        var seed = SharedRandomExtensions.HashCodeCombine((int)timing.CurTick.Value, _entityManager.GetNetEntity(entity).Id);
        var rand = new System.Random(seed);

        var amount = rand.NextGaussian(args.Effect.μ, args.Effect.σ);
        amount *= (double)args.Scale;

        var reagent = args.Effect.Reagent;

        if (amount > 0)
            _solutionContainer.TryAddReagent(entity, reagent, amount);
        else
            _solutionContainer.RemoveReagent(entity, reagent, -amount);
    }
}

public sealed partial class AdjustReagentGaussian : EntityEffectBase<AdjustReagentGaussian>
{
    /// <summary>
    ///     The reagent ID to add or remove.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ReagentPrototype> Reagent;

    [DataField(required: true)]
    public double μ;

    [DataField(required: true)]
    public double σ;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        var proto = prototype.Index(Reagent);
        return Loc.GetString("reagent-effect-guidebook-adjust-reagent-gaussian",
            ("chance", Probability),
            ("deltasign", Math.Sign(μ)),
            ("reagent", proto.LocalizedName),
            ("mu", Math.Abs(μ)),
            ("sigma", Math.Abs(σ)));
    }
}
