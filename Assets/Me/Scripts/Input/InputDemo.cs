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
    void Update()
    {
        if (Input.GetKey("up") || OVRInput.Get(OVRInput.Button.One))
        {
   //         spotifyScript.resumePlayback();
        }
       else if (OVRInput.Get(OVRInput.Button.Two))
        {
   //         spotifyScript.pausePlayback();
        }
    }

    void OnMouseDown() {
     //   Debug.Log("OnMouseDown called");
    }
}
