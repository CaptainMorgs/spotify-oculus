using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models;

public class PlaylistScript : MonoBehaviour {

	private string playlistName, playlistURI;
	private GameObject spotifyManager;

	private Spotify script;
    public GameObject  playlistNameObject, recordPlayer;
	private RecordPlayer recordPlayerScript;
    private UnityEngine.UI.Text playlistNameText;
   // private float a;
   // private Color c;
    private MeshRenderer meshRenderer;
    public GameObject spriteGameObject;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
    //    a = meshRenderer.material.color.a;
    //    c = meshRenderer.material.GetColor("_TintColor");

        spotifyManager = GameObject.Find ("SpotifyManager");
		script = spotifyManager.GetComponent<Spotify>();
        playlistNameText = playlistNameObject.GetComponent<UnityEngine.UI.Text>();
        spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();
		recordPlayerScript = recordPlayer.GetComponent<RecordPlayer> ();
    }
	
	public void setPlaylistURI (string playlistURI) {
		this.playlistURI = playlistURI;
        
	}

	public void setPlaylistName (string playlistName) {
		this.playlistName = playlistName;
        playlistNameText.text = playlistName;

    }

	public void playPlaylist() {
        Debug.Log(playlistURI);
		script.playPlaylist (playlistURI);
		recordPlayerScript.recordPlayerActive = true;
	}

    public void TogglePlayButton() {
        if (spriteRenderer.enabled == true)
        {
            spriteRenderer.enabled = false;
        }
        else {
            spriteRenderer.enabled = true;
        }
    }
}
