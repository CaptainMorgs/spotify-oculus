﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandUI : MonoBehaviour {

    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private string gameObjectName;
    public GameObject RaycastGameObject;
    private Raycast raycast;

    // Use this for initialization
    void Start () {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        raycast = RaycastGameObject.GetComponent<Raycast>();
        gameObjectName = transform.name;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnRayCastHit()
    {
        gameObjectName = transform.name;

        if (gameObjectName.Equals("Play"))
        {
            spotifyManagerScript.resumePlayback();
        }
        else if (gameObjectName.Equals("Pause"))
        {
            spotifyManagerScript.pausePlayback();
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
        else if (gameObjectName.Equals("Toggle"))
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
        else {
            Debug.LogError("Cannot find UI Gameobject by name");
        }
    }
}
