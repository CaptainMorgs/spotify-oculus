using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SpotifyAPI.Web.Models;
using System;

public class HoverUI : MonoBehaviour {

    private PlaylistScript playlistScript;

    private GameObject text;
    private TextMeshProUGUI textPro;

    // Use this for initialization
    void Start () {
        //  text = GameObject.Find("TextMeshPro Text");
        text = transform.Find("TextMeshPro Text").gameObject;
        textPro = text.GetComponent<TextMeshProUGUI>();// ?? gameObject.AddComponent<TextMeshProUGUI>();
        Debug.Log(text);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateHoverUI(PlaylistScript playlistScript)
    {
        this.playlistScript = playlistScript;

        FullTrack track = playlistScript.getFullTrack();

        if (track != null)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(playlistScript.getFullTrack().DurationMs);

            string answer = string.Format("{0:D2}m:{1:D2}s",
                        t.Minutes,
                        t.Seconds);

            //   double trackDuration = (((double)playlistScript.getFullTrack().DurationMs / (double)1000)/ (double) 60);

            textPro.SetText(playlistScript.getPlaylistName()
            + "\n" + "Length: " + answer
            );
        }
        else {
            textPro.SetText(playlistScript.getPlaylistName());

        }

    }

    
}
