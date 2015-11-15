﻿using UnityEngine;
using System.Collections;
using Storage;

public class Main_Game : MonoBehaviour {

	private GameInformation info;
	private CameraController cam;
	private Player user;
    Managers.BuildingsManager bm;
    Managers.SoundsManager sounds;
    public Managers.BuildingsManager BuildingsMgr { get { return bm; } }

	Transform strongholdTransform;
	GameObject playerHero;

    // Use this for initialization
    void Start () {
        strongholdTransform = GameObject.Find("PlayerStronghold").transform;
        playerHero = GameObject.Find("PlayerHero");
        if(GameObject.Find("GameInformationObject"))
		    info = (GameInformation) GameObject.Find("GameInformationObject").GetComponent("GameInformation");
        user = GameObject.Find("GameController").GetComponent("Player") as Player;
		cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        bm = GameObject.Find("GameController").GetComponent<Managers.BuildingsManager>();
        sounds = GameObject.Find("GameController").GetComponent<Managers.SoundsManager>();
        if (info) info.LoadHUD();
        LoadPlayerStronghold();
        LoadPlayerUnits();
        StartGame();
    }

	private void LoadPlayerStronghold()
	{
		GameObject playerStronghold;
        ConstructionGrid grid;
        if (info)
        {
            grid = GetComponent<ConstructionGrid>();
            strongholdTransform.position = grid.discretizeMapCoords(strongholdTransform.position);
            playerStronghold = Info.get.createBuilding(info.GetPlayerRace(),
                                                       BuildingTypes.STRONGHOLD,
                                                       strongholdTransform.position,
                                                       strongholdTransform.rotation);
            grid.reservePosition(strongholdTransform.position);
			user.addEntity(playerStronghold.GetComponent<IGameEntity>());
			cam.lookGameObject(playerStronghold);
        }
	}

    private void LoadPlayerUnits()
    {
        if (info)
        {
            // TODO Must be able to load other kinds of units (both civilian and military)
            playerHero = Info.get.createUnit(info.GetPlayerRace(),
                UnitTypes.HERO, playerHero.transform.position, playerHero.transform.rotation);

            user.addEntity(playerHero.GetComponent<IGameEntity>());
        }
    }

    public GameInformation GetGameInformationObject()
    {
		return info;
    }

    public void StartGame()
    {
        switch (info.getGameMode())
        {
            case GameInformation.GameMode.CAMPAIGN:
                LoadCampaign();
                break;
            case GameInformation.GameMode.SKIRMISH:
                break;
        }
    }

    public void LoadCampaign()
    {
        // TODO Create campaign
    }

    void OnDestroy()
    {
        ClearGame();
    }

    public void ClearGame()
    {
        GameObject obj;
        obj = GameObject.Find("GameInformationObject").gameObject;
        Destroy(obj);
    }
}
