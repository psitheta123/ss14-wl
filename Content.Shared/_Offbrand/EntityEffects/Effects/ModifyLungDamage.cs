using Content.Shared._Offbrand.Wounds;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ModifyLungDamageEffectSystem : EntityEffectSystem<LungDamageComponent, ModifyLungDamage>
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    protected override void Effect(Entity<LungDamageComponent> entity, ref EntityEffectEvent<ModifyLungDamage> args)
    {
        var nullable = new Entity<LungDamageComponent?>(entity, entity.Comp);
        _entityManager.System<LungDamageSystem>()
            .TryModifyDamage(nullable, args.Effect.Amount * args.Scale);
    }
}

public sealed partial class ModifyLungDamage : EntityEffectBase<ModifyLungDamage>
{
    [DataField(required: true)]
    public FixedPoint2 Amount;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        if (Amount < FixedPoint2.Zero)
            return Loc.GetString("reagent-effect-guidebook-modify-lung-damage-heals", ("chance", Probability), ("amount", -Amount));
        else
            return Loc.GetString("reagent-effect-guidebook-modify-lung-damage-deals", ("chance", Probability), ("amount", Amount));
    }
}

