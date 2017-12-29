using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models;
using System;

public class PlaylistScript : MonoBehaviour {

	private string playlistName, playlistURI;
	private GameObject spotifyManager;
	private Spotify script;
    public GameObject  playlistNameObject, recordPlayer;
	private RecordPlayer recordPlayerScript;
    private UnityEngine.UI.Text playlistNameText;
    private MeshRenderer meshRenderer;
    public GameObject spriteGameObject;
    private SpriteRenderer spriteRenderer;
    private SimplePlaylist simplePlaylist;
    private FullTrack fullTrack;
    public SimpleAlbum simpleAlbum;
    public FullArtist fullArtist;


    // Use this for initialization
    void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
        spotifyManager = GameObject.Find ("SpotifyManager");
		script = spotifyManager.GetComponent<Spotify>();
        playlistNameText = playlistNameObject.GetComponent<UnityEngine.UI.Text>();
        spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();
		recordPlayerScript = recordPlayer.GetComponent<RecordPlayer> ();

    }
	
	public void setPlaylistURI (string playlistURI) {
		this.playlistURI = playlistURI;
    }

    public void setSimplePlaylist(SimplePlaylist simplePlaylist)
    {
        this.simplePlaylist = simplePlaylist;
    }

    public void setPlaylistName (string playlistName) {
		this.playlistName = playlistName;
        playlistNameText.text = playlistName;

    }

    public void setFullTrack(FullTrack fullTrack)
    {
        this.fullTrack = fullTrack;
        Debug.Log("Setting full track");
    }

    public FullTrack getFullTrack()
    {
        return fullTrack;

    }

    public SimplePlaylist getSimplePlaylist()
    {
        return simplePlaylist;

    }
        public string getPlaylistURI()
        {
            return playlistURI;
        }

        public string getPlaylistName()
    {
       return playlistName;
    }

    public async System.Threading.Tasks.Task playSomethingAsync() {
        if (transform.tag == "song")
        {
            playSong();
        }
        else if (transform.tag == "artist") {
          await  playArtistAsync();

        }
        else
        {
            playPlaylist();
        }
    }

    private async System.Threading.Tasks.Task playArtistAsync()
    {
        //just plays the artists top song
        SeveralTracks artistTopTracks = await script.GetArtistTopTracksAsync(fullArtist.Id);
        script.playSongURI(artistTopTracks.Tracks[0].Uri);
    }

    private void playPlaylist() {
		script.playURI (playlistURI);
	//	recordPlayerScript.recordPlayerActive = true;
	}

    private void playSong()
    {
        script.playSongURI(playlistURI);
   //     recordPlayerScript.recordPlayerActive = true;
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

    void OnMouseDown() {
        playSomethingAsync();
    }
}
