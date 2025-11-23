using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class StartHeartEntityEffectSystem : EntityEffectSystem<HeartrateComponent, StartHeart>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<HeartrateComponent> entity, ref EntityEffectEvent<StartHeart> args)
    {
        var nullable = new Entity<HeartrateComponent?>(entity, entity.Comp);
        _entityManager.System<HeartSystem>()
            .TryRestartHeart(nullable);
    }
}

public sealed partial class StartHeart : EntityEffectBase<StartHeart>
{
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-start-heart", ("chance", Probability));
    }
}
