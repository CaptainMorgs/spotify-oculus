using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftHandUI : MonoBehaviour {

    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private string gameObjectName;
    public GameObject RaycastGameObject, recommendationDeck;
    private RecommenderDeck recommenderDeckScript;
    private Raycast raycast;
    private bool isHovering = false;
    public float idleTransparancy = 0.8f;
    private SaveLoad saveLoad;

    // Use this for initialization
    void Start () {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();
        recommendationDeck = GameObject.Find("RecommenderDeck");
        recommenderDeckScript = recommendationDeck.GetComponent<RecommenderDeck>();
        raycast = RaycastGameObject.GetComponent<Raycast>();
        gameObjectName = transform.name;
        //Ignore collisions between grabble and handUI layer, i.e. hand ui and vinyl (they can collide when holding vinyl)
        Physics.IgnoreLayerCollision(8,11,true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //TODO surely this can be cleaner....
    public void OnRayCastHit()
    {
        Debug.Log(transform.name);
        gameObjectName = transform.name;

        if (gameObjectName.Equals("Play"))
        {
            spotifyManagerScript.ResumePlayback();
        }
        else if (gameObjectName.Equals("Pause"))
        {
            spotifyManagerScript.PausePlayback();
        }
        else if (gameObjectName.Equals("ForwardLeft"))
        {
            spotifyManagerScript.SkipPlaybackToPrevious();
        }
        else if (gameObjectName.Equals("ForwardRight"))
        {
            spotifyManagerScript.SkipPlaybackToNext();
        }
        else if (gameObjectName.Equals("SetShuffle"))
        {
            spotifyManagerScript.SetShuffle();
        }
        else if (gameObjectName.Equals("SetRepeatMode"))
        {
            spotifyManagerScript.SetRepeatMode();
        }
        else if (gameObjectName.Equals("PlayOnClick"))
        {
            UnityEngine.UI.Toggle toggle = gameObject.GetComponent<UnityEngine.UI.Toggle>();
            if (toggle.isOn)
            {
                raycast.playOnClick = false;
                toggle.isOn = false;
                Debug.Log("Play on click set to false");
            }
            else {
                raycast.playOnClick = true;
                toggle.isOn = true;
                Debug.Log("Play on click set to false");
            }
        }
        else if (gameObjectName.Equals("Reload"))
        {
            saveLoad.Reload();
        }
        else if (gameObjectName.Equals("GetRecommendations"))
        {
            Debug.Log("Calling GetRecommendations from lefthandui");
            recommenderDeckScript.GetRecommendations();
        }
        else {
            Debug.LogError("Cannot find UI Gameobject by name");
        }
    }

    public void OnHover()
    {
        if (!isHovering)
        {
            isHovering = true;
            transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            if (!gameObjectName.Equals("PlayOnClick"))
            {
                Color color = GetComponent<Image>().color;
                if (color != null)
                {
                    color.a = 1.0f;
                    GetComponent<Image>().color = color;
              //      Debug.Log("Altering UI Transparancy");

                }
            }
            
        }
    }

    public void OnHoverExit()
    {
        if (isHovering )
        {
            isHovering = false;
            transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
            if (!gameObjectName.Equals("Toggle"))
            {
                Color color = GetComponent<Image>().color;
                if (color != null)
                {
                    color.a = idleTransparancy;
                    GetComponent<Image>().color = color;
                    //   Debug.Log("Altering UI Transparancy");
                }
            }
        }
    }

    
}
