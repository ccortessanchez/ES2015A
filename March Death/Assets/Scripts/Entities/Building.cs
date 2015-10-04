﻿using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Storage;


/// <summary>
/// Building base class. Extends actor (which in turn extends MonoBehaviour) to
/// handle basic API operations
/// </summary>
public class Building : Utils.Actor<Building.Actions>, IGameEntity
{
    public enum Actions { DAMAGED, DESTROYED };

    public Building() { }

    /// <summary>
    /// Edit this on the Prefab to set Units of certain races/types
    /// </summary>
    public Races race = Races.MEN;
    public BuildingTypes type = BuildingTypes.FORTRESS;


    /// <summary>
    /// List of ability objects of this building
    /// </summary>
    private List<IBuildingAbility> _abilities;

    /// <summary>
    /// Contains all static information of the Building.
    /// That means: max health, damage, defense, etc.
    /// </summary>
    private BuildingInfo _info;
    private BuildingAttributes _attributes;
    public EntityInfo info
    {
        get
        {
            return _info;
        }
    }

    /// <summary>
    /// Returns current status of the Building
    /// </summary>
    private EntityStatus _status;
    public EntityStatus status
    {
        get
        {
            return _status;
        }
    }

    /// <summary>
    /// Resource building might need this to acount how many workers can pull resources from it.
    /// </summary>
    public int usedCapacity { get; set; }

    /// <summary>
    /// Returns the number of wounds a building received
    /// </summary>
    private float _woundsReceived;
    public float wounds
    {
        get
        {
            return _woundsReceived;
        }
    }

    /// <summary>
    /// Returns percentual value of health (100% meaning all life)
    /// </summary>
    public float healthPercentage
    {
        get
        {
            return (_attributes.wounds - _woundsReceived) * 100f / _attributes.wounds;
        }
    }

    /// <summary>
    /// Returns percentual value of damage (100% meaning 0% life)
    /// </summary>
    public float damagePercentage
    {
        get
        {
            return 100f - healthPercentage;
        }
    }


    /// <summary>
    /// Returns true in case an attack will land on this unit
    /// </summary>
    /// <param name="from">Unit which attacked</param>
    /// <param name="isRanged">Set to true in case the attack is range, false if melee</param>
    /// <returns>True if it hits, false otherwise</returns>
    private bool willAttackLand(Unit from, bool isRanged = false)
    {
        int dice = Utils.D6.get.rollSpecial();

        if (isRanged)
        {
            // TODO: Specil units (ie gigants) and distance!
            return dice > 1 && (((UnitAttributes)from.info.attributes).projectileAbility + dice >= 7);
        }

        return true;
    }

    /// <summary>
    /// Retuns true if an attack will cause wounds to this unit
    /// </summary>
    /// <param name="from">Attacker</param>
    /// <returns>True if causes wounds, false otherwise</returns>
    private bool willAttackCauseWounds(Unit from)
    {
        int dice = Utils.D6.get.rollOnce();
        return HitTables.wounds[((UnitAttributes)from.info.attributes).strength, _attributes.resistance] <= dice;
    }

    /// <summary>
    /// Automatically calculates if an attack will hit, and in case it
    /// does it updates the current state.
    /// </summary>
    /// <param name="from">Attacker</param>
    /// <param name="isRanged">True if the attack is ranged, false if melee</param>
    public void receiveAttack(Unit from, bool isRanged)
    {
        // Do not attack dead targets
        if (_status == EntityStatus.DESTROYED)
        {
            throw new InvalidOperationException("Can not receive damage while not alive");
        }

        // If it hits and produces damage, update wounds
        if (willAttackLand(from, isRanged) && willAttackCauseWounds(from))
        {
            _woundsReceived += 1;
            fire(Actions.DAMAGED);
        }

        // Check if we are dead
        if (_woundsReceived == _attributes.wounds)
        {
            _status = EntityStatus.DESTROYED;

            fire(Actions.DESTROYED);
        }
    }

    /// <summary>
    /// Iterates all abilities on the
    /// </summary>
    private void setupAbilities()
    {
        _abilities = new List<IBuildingAbility>();

        foreach (BuildingAbility ability in _info.actions)
        {
            // Try to get class with this name
            string abilityName = ability.name.Replace(" ", "");

            try
            {
                var constructor = Type.GetType(abilityName).
                    GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(BuildingAbility), typeof(GameObject) }, null);
                if (constructor == null)
                {
                    // No such constructor, construct default
                    _abilities.Add(new GenericBuildingAbility(ability));
                }
                else
                {
                    // Class found, use that!
                    _abilities.Add((IBuildingAbility)constructor.Invoke(new object[2] { ability, gameObject }));
                }
            }
            catch (Exception)
            {
                // No such class, use the GenericAbility class
                _abilities.Add(new GenericBuildingAbility(ability));
            }
        }
    }

    /// <summary>
    /// Returns an action given a name
    /// </summary>
    /// <param name="name">Name of the action</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown when no action with the given name is found
    /// </exception>
    /// <returns>Always returns a valid IBuildingAbility (IAction)</returns>
    public IAction getAction(string name)
    {
        foreach (IBuildingAbility ability in _abilities)
        {
            if (ability.info.name.Equals(name))
            {
                return ability;
            }
        }

        throw new ArgumentException("Invalid action " + name + "requested");
    }


    /// <summary>
    /// Object initialization
    /// </summary>
    void Start()
    {
        _status = EntityStatus.IDLE;
        _info = Info.get.of(race, type);
        _attributes = (BuildingAttributes)_info.attributes;
        setupAbilities();
    }

    /// <summary>
    /// Called once a frame to update the object
    /// </summary>
    void Update()
    {
    }

    /// <summary>
    /// Called every fixed physics frame
    /// </summary>
    void FixedUpdate()
    {
    }

    /// <summary>    
    /// Returns NULL as this cannot be converted to Unit
	/// </summary>
    /// <returns>Object casted to Unit</returns>
    public Unit toUnit() { return null;  }

    /// <summary>
    /// Casts this IGameEntity to Unity (pointless if already building)
    /// </summary>
    /// <returns>Always null</returns>
    public Building toBuilding() { return this; }

    public Resource toResource() { return null; }

}
