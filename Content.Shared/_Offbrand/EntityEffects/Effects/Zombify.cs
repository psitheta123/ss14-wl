using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ZombidyEntityEffectSystem : EntityEffectSystem<MobStateComponent, Zombify>
{
    protected override void Effect(Entity<MobStateComponent> entity, ref EntityEffectEvent<Zombify> args)
    { }
}

public sealed partial class Zombify : EntityEffectBase<Zombify>
{
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-zombify", ("chance", Probability));
    }
}
