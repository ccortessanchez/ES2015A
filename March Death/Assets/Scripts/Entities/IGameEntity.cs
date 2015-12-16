using System;
﻿using Storage;
using Utils;

// Values are set only for clarity purposes on the Animator
// Once DEAD/DESTROYED, you can not go back to any state!
public enum EntityStatus
{
    BUILDING_PHASE_1 = 0,
    BUILDING_PHASE_2 = 1,
    BUILDING_PHASE_3 = 2,
    IDLE = 3,
    MOVING = 4,
    ATTACKING = 5,
    DEAD = 6,
    DESTROYED = 7,
    WORKING = 8
};

public interface IGameEntity : IBaseActor
{
    EntityInfo info { get; }
    EntityStatus status { get; }
    EntityStatus DefaultStatus { get; set; }

    int wounds { get; }
    float damagePercentage { get; }
    float healthPercentage { get; }

    UnityEngine.Transform getTransform();
    UnityEngine.GameObject getGameObject();
    UnityEngine.Vector3 closestPointTo(UnityEngine.Vector3 point);

    Ability getAbility(string name);

    Races getRace();
    E getType<E>() where E : struct, IConvertible;
    void Destroy(bool immediately = false);

    IKeyGetter registerFatalWounds(Action<System.Object> func);
    IKeyGetter unregisterFatalWounds(Action<System.Object> func);
    void setStatus(EntityStatus status);

    void receiveAttack(Unit from, bool isRanged);

    bool doIfUnit(Action<Unit> callIfTrue);
    bool doIfBuilding(Action<IBuilding> callIfTrue);
    bool doIfResource(Action<Resource> callIfTrue);
    bool doIfBarrack(Action<Barrack> callIfTrue);
}
