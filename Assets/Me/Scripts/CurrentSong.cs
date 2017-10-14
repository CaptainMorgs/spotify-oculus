using SpotifyAPI.Web.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSong : MonoBehaviour
{

    private GameObject spotifyManager, songNameObject;
    private Spotify spotifyScript;
    private string artistID, songID;
    private MeshRenderer meshRenderer;
    private UnityEngine.UI.Text playlistNameText;

    //TODO display song name and artist name as well as image of artist

    // Use this for initialization
    void Start()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyScript = spotifyManager.GetComponent<Spotify>();
        meshRenderer = GetComponent<MeshRenderer>();
     //   playlistNameText = songNameObject.GetComponent<UnityEngine.UI.Text>();
    }

    public void updateCurrentlyPlaying(string artistID)
    {
        this.artistID = artistID;
        StartCoroutine(DisplayArtist());
    }

    private IEnumerator DisplayArtist()
    {
        FullArtist artist = spotifyScript.GetFullArtist(artistID);
        string artistImageURL = artist.Images[0].Url;
        WWW imageURLWWW = new WWW(artistImageURL);
        yield return imageURLWWW;
        meshRenderer.material.mainTexture = imageURLWWW.texture;
    }
}
