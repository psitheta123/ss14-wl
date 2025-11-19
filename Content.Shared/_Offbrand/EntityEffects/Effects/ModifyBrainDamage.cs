using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ModifyBrainDamageEffectSystem : EntityEffectSystem<BrainDamageComponent, ModifyBrainDamage>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<BrainDamageComponent> entity, ref EntityEffectEvent<ModifyBrainDamage> args)
    {
        var nullable = new Entity<BrainDamageComponent?>(entity, entity.Comp);
        _entityManager.System<BrainDamageSystem>()
            .TryChangeBrainDamage(nullable, args.Effect.Amount * args.Scale);
    }
}

public sealed partial class ModifyBrainDamage : EntityEffectBase<ModifyBrainDamage>
{
    [DataField(required: true)]
    public FixedPoint2 Amount;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        if (Amount < FixedPoint2.Zero)
            return Loc.GetString("reagent-effect-guidebook-modify-brain-damage-heals", ("chance", Probability), ("amount", -Amount));
        else
            return Loc.GetString("reagent-effect-guidebook-modify-brain-damage-deals", ("chance", Probability), ("amount", Amount));
    }
}
