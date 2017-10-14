using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDemo : MonoBehaviour {

    private GameObject spotifyManager;

    private Spotify spotifyScript;

    // Use this for initialization
    void Start () {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyScript = spotifyManager.GetComponent<Spotify>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey("up")) {
            // spotifyScript.playPlaylist("spotify: track:4iV5W9uYEdYUVa79Axb7Rh");
          //  spotifyScript.playPlaylist("spotify:user:spotify:playlist:37i9dQZF1DXdsy92d7BLpC");
    }

    }

    void OnMouseDown() {
     //   Debug.Log("OnMouseDown called");
    }
}
