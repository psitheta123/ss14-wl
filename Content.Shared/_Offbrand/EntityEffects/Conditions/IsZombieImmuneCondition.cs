using Content.Shared.EntityConditions;
using Content.Shared.EntityEffects;
using Content.Shared.Mobs.Components;
using Content.Shared.Zombies;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class IsZombieImmuneConditionSystem : EntityConditionSystem<MobStateComponent, IsZombieImmuneCondition>
{
    protected override void Condition(Entity<MobStateComponent> entity, ref EntityConditionEvent<IsZombieImmuneCondition> args)
    {
        args.Result = HasComp<ZombieImmuneComponent>(entity) ^ args.Condition.Invert;
    }
}

public sealed partial class IsZombieImmuneCondition : EntityConditionBase<IsZombieImmuneCondition>
{
    [DataField]
    public bool Invert = false;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        return Loc.GetString("reagent-effect-condition-guidebook-is-zombie", ("invert", Invert));
    }
}

