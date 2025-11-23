using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ModifyBrainOxygenEffectSystem : EntityEffectSystem<BrainDamageComponent, ModifyBrainOxygen>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<BrainDamageComponent> entity, ref EntityEffectEvent<ModifyBrainOxygen> args)
    {
        var nullable = new Entity<BrainDamageComponent?>(entity, entity.Comp);
        _entityManager.System<BrainDamageSystem>()
            .TryChangeBrainOxygenation(nullable, args.Effect.Amount * args.Scale);
    }
}

public sealed partial class ModifyBrainOxygen : EntityEffectBase<ModifyBrainOxygen>
{
    [DataField(required: true)]
    public FixedPoint2 Amount;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        if (Amount < FixedPoint2.Zero)
            return Loc.GetString("reagent-effect-guidebook-modify-brain-oxygen-heals", ("chance", Probability), ("amount", Amount));
        else
            return Loc.GetString("reagent-effect-guidebook-modify-brain-oxygen-deals", ("chance", Probability), ("amount", -Amount));
    }
}

