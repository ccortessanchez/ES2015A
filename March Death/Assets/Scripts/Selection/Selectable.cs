﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Selectable : MonoBehaviour
{

    private Rect selectedRect = new Rect();
    private Texture2D selectedBox;
    bool currentlySelected { get; set; }
    private float healthRatio = 1f;
    private bool updateHealthRatio = true;
    private bool entityMoving = true;

    //Pendiente
    //IGameEntity gameEntity;

    protected virtual void Start()
    {
        //Pendiente
        //gameEntity = this.GetComponent<IGameEntity>();
        selectedBox = SelectionOverlay.CreateTexture();
        currentlySelected = false;

    }

    protected virtual void Update() { }

    protected virtual void LateUpdate()
    {
        bool updateSomething = false;

        // the GameEntity is moving
        if(entityMoving)
        {
            // calculates the box
            selectedRect = SelectionOverlay.CalculateBox(GetComponent<Collider>());
            updateSomething = true;
        }

        if (updateHealthRatio)
        {
            //Pendiente
            //healthRatio = gameEntity.healthPercentage() / 100f;
            updateSomething = true;
            // doesn't update until gets the callback
            updateHealthRatio = false;
        }


        if(updateSomething) SelectionOverlay.UpdateTexture(selectedBox, healthRatio);
    }

    protected virtual void OnGUI()
    {
        if (currentlySelected)
        {
            DrawSelection();
        }
    }

    public virtual void Select(Player player)
    {
        //only handle input if currently selected

        Selectable oldObject = player.SelectedObject;

        if ( !this.Equals(oldObject))
        {
            // old object selection is now false (if exists)
            if (oldObject) oldObject.currentlySelected = false;
            // player selected object is now this current selectable object
            player.SelectedObject = this;
            this.currentlySelected = true;
            //Debug pursposes
            //Pendiente
            //Debug.Log(gameEntity.info.name);
            registerEntityCallbacks();

			updateActorInformation();

        }
    }

	private void updateActorInformation() 
	{
		Transform information = getHUDInformationComponent ();
		if (information != null) 
		{
			Transform txtActorName = information.transform.FindChild ("ActorName");
			Transform txtActorHealth = information.transform.FindChild ("ActorHealth");
			Transform txtActorImage = information.transform.FindChild ("ActorImage");
					
			IGameEntity entity = gameObject.GetComponent<IGameEntity> ();
			txtActorName.gameObject.GetComponent<Text> ().text = entity.info.name;
			txtActorName.gameObject.GetComponent<Text>().enabled = true;
			txtActorHealth.gameObject.GetComponent<Text> ().text = entity.healthPercentage.ToString ();
			txtActorHealth.gameObject.GetComponent<Text>().enabled = true;

			txtActorImage.gameObject.GetComponent<Image> ().color = new Color(0, 0, 1, 1);
			txtActorImage.gameObject.GetComponent<Image>().enabled = true;
		}
	}

	private void hideActorInformation() 
	{
		Transform information = getHUDInformationComponent ();
		if (information != null) 
		{
			Transform txtActorName = information.transform.FindChild("ActorName");
			Transform txtActorHealth = information.transform.FindChild("ActorHealth");
			Transform txtActorImage = information.transform.FindChild ("ActorImage");

			txtActorName.gameObject.GetComponent<Text>().enabled = false;
			txtActorHealth.gameObject.GetComponent<Text>().enabled = false;
			txtActorImage.gameObject.GetComponent<Image>().enabled = false;

		}
	}

	private Transform getHUDInformationComponent() 
	{
		GameObject hud = GameObject.Find ("HUD");	
		if (hud != null) {
			Transform information = hud.transform.FindChild ("Information");
			if (information != null) {
				return information;
			}
		}

		return null;
	}

    private void registerEntityCallbacks()
    {
        //TODO
    }
    private void unregisterEntityCallbacks()
    {
        //TODO
    }
    public virtual void Deselect()
    {
        currentlySelected = false;
		hideActorInformation ();
    }

    private void DrawSelection()
    {
        GUI.DrawTexture(selectedRect, selectedBox);
    }
}
