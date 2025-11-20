using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ModifyHeartDamageEffectSystem : EntityEffectSystem<HeartrateComponent, ModifyHeartDamage>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<HeartrateComponent> entity, ref EntityEffectEvent<ModifyHeartDamage> args)
    {
        var nullable = new Entity<HeartrateComponent?>(entity, entity.Comp);
        _entityManager.System<HeartSystem>()
            .ChangeHeartDamage(nullable, args.Effect.Amount * args.Scale);
    }
}

public sealed partial class ModifyHeartDamage : EntityEffectBase<ModifyHeartDamage>
{
    [DataField(required: true)]
    public FixedPoint2 Amount;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        if (Amount < FixedPoint2.Zero)
            return Loc.GetString("reagent-effect-guidebook-modify-heart-damage-heals", ("chance", Probability), ("amount", -Amount));
        else
            return Loc.GetString("reagent-effect-guidebook-modify-heart-damage-deals", ("chance", Probability), ("amount", Amount));
    }
}

