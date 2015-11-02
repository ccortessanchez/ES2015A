﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.AI;

public abstract class BasePlayer : Utils.SingletonMono<BasePlayer> {

    protected BasePlayer() { }

    /// <summary>
    /// The race of the player
    /// </summary>
    protected Storage.Races _selfRace;
    public Storage.Races race { get { return _selfRace; } }

    /// <summary>
    /// The resources manager
    /// </summary>
    protected Managers.ResourcesManager _resources = new Managers.ResourcesManager();
    public Managers.IResourcesManager resources { get { return _resources; } }

    /// <summary>
    /// The buildings manager
    /// </summary>
    protected Managers.BuildingsManager _buildings;
    public Managers.BuildingsManager buildings { get { return _buildings; } }



    /// <summary>
    /// The selection Manager
    /// </summary>
    protected Managers.SelectionManager _selection = new Managers.SelectionManager();
    public Managers.SelectionManager selection { get { return _selection; } }
     



    protected static GameInformation _info = null;
    protected static BasePlayer _player = null;
    protected static BasePlayer _ia = null;

    public static GameInformation info { get { return _info; } }
    public static Player player { get { return (Player)_player; } }
    public static AIController ia { get { return (AIController)_ia; } }


    public virtual void Start ()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        GameObject gameInformationObject = GameObject.Find("GameInformationObject");

        _info = gameInformationObject.GetComponent<GameInformation>();
        _player = gameController.GetComponent<Player>();
        _ia = gameController.GetComponent<AIController>();
        
    }

    public abstract void removeEntity(IGameEntity entity);
    public abstract void addEntity(IGameEntity newEntity);

    public abstract void addGameObject(GameObject go);
    public abstract void removeGameObject(GameObject go);

    public static BasePlayer getOwner(IGameEntity entity)
    {
        if (entity.info.race == info.GetPlayerRace())
        {
            return player;
        }

        return ia;
    }

    void Update () {}
}
