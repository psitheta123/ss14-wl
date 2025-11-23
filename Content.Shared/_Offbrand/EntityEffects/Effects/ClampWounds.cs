using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ClampWoundEffectSystem : EntityEffectSystem<WoundableComponent, ClampWounds>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<WoundableComponent> entity, ref EntityEffectEvent<ClampWounds> args)
    {
        var nullable = new Entity<WoundableComponent?>(entity, entity.Comp);
        _entityManager.System<WoundableSystem>()
            .ClampWounds(nullable, args.Effect.Chance);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class ClampWounds: EntityEffectBase<ClampWounds>
{
    [DataField(required: true)]
    public float Chance;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-clamp-wounds", ("probability", Probability), ("chance", Chance));
    }
}
