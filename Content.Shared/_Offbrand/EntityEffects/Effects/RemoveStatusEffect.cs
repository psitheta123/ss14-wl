using Content.Shared.EntityEffects;
using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class RemoveStatusEffectEntityEffectSystem : EntityEffectSystem<StatusEffectContainerComponent, RemoveStatusEffect>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<StatusEffectContainerComponent> entity, ref EntityEffectEvent<RemoveStatusEffect> args)
    {
        _entityManager.System<StatusEffectsSystem>()
            .TryRemoveStatusEffect(entity, args.Effect.EffectProto);
    }
}

public sealed partial class RemoveStatusEffect : EntityEffectBase<RemoveStatusEffect>
{
    [DataField(required: true)]
    public EntProtoId EffectProto;

    /// <inheritdoc />
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString(
            "reagent-effect-guidebook-status-effect-remove",
            ("chance", Probability),
            ("key", prototype.Index(EffectProto).Name));
    }
}
