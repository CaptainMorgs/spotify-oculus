using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models;
using System;

public class PlaylistScript : MonoBehaviour
{

    private string playlistName, playlistURI;
    private GameObject spotifyManager;
    private Spotify script;
    public GameObject playlistNameObject, recordPlayer;
    private RecordPlayer recordPlayerScript;
    private GameObject audioVisualizer;
    private AudioVisualizer audioVisualizerScript;
    private UnityEngine.UI.Text playlistNameText;
    private MeshRenderer meshRenderer;
    public GameObject spriteGameObject;
    private SpriteRenderer spriteRenderer;
    private SimplePlaylist simplePlaylist;
    private FullTrack fullTrack;
    public SimpleAlbum simpleAlbum;
    public FullArtist fullArtist;
    public AudioAnalysis audioAnalysis;
    public UnityEngine.UI.Image image;
    public Sprite sprite;

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        spotifyManager = GameObject.Find("SpotifyManager");
        script = spotifyManager.GetComponent<Spotify>();
        playlistNameText = playlistNameObject.GetComponent<UnityEngine.UI.Text>();
        spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();
        recordPlayerScript = recordPlayer.GetComponent<RecordPlayer>();
        audioVisualizer = GameObject.Find("AudioVisualizer");
        audioVisualizerScript = audioVisualizer.GetComponent<AudioVisualizer>();

    }

    public void setPlaylistURI(string playlistURI)
    {
        this.playlistURI = playlistURI;
    }

    public void setSimplePlaylist(SimplePlaylist simplePlaylist)
    {
        this.simplePlaylist = simplePlaylist;
    }

    public void setPlaylistName(string playlistName)
    {
        this.playlistName = playlistName;
        playlistNameText.text = playlistName;

    }

    public void setFullTrack(FullTrack fullTrack)
    {
        this.fullTrack = fullTrack;
        //   Debug.Log("Setting full track");
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

    public void PlaySomething()
    {
        if (transform.tag == "song")
        {
            playSong();

            if (audioAnalysis != null)
            {
                audioVisualizerScript.SendAnalysis(audioAnalysis);
            }
        }
        else if (transform.tag == "artist")
        {
            playArtist();

        }
        else
        {
            playPlaylist();
        }
    }

    private void playArtist()
    {
        //just plays the artists top song
        SeveralTracks artistTopTracks = script.GetArtistsTopTracks(fullArtist.Id);
        script.playSongURI(artistTopTracks.Tracks[0].Uri);
    }

    private void playPlaylist()
    {
        script.playURI(playlistURI);
        //	recordPlayerScript.recordPlayerActive = true;
    }

    private void playSong()
    {
        script.playSongURI(playlistURI);
        //     recordPlayerScript.recordPlayerActive = true;
    }

    public void TogglePlayButton()
    {
        if (spriteRenderer.enabled == true)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }

    void OnMouseDown()
    {
        PlaySomething();
    }
}
