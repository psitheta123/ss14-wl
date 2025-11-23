using Content.Shared.EntityConditions;
using Content.Shared.EntityEffects;
using Content.Shared.Mobs.Components;
using Content.Shared.Zombies;
using Robust.Shared.Prototypes;

namespace Content.Shared._Offbrand.EntityEffects;

/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class IsZombieConditionSystem : EntityConditionSystem<MobStateComponent, IsZombieCondition>
{
    protected override void Condition(Entity<MobStateComponent> entity, ref EntityConditionEvent<IsZombieCondition> args)
    {
        args.Result = HasComp<ZombieComponent>(entity) ^ args.Condition.Invert;
    }
}

public sealed partial class IsZombieCondition : EntityConditionBase<IsZombieCondition>
{
    [DataField]
    public bool Invert = false;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        return Loc.GetString("reagent-effect-condition-guidebook-is-zombie", ("invert", Invert));
    }
}
