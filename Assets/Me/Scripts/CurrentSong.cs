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
    private GameObject text, audioAnalysis;
    private UnityEngine.UI.Text artistNameText, audioAnalysisText;

    //TODO display song name and artist name as well as image of artist

    // Use this for initialization
    void Start()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyScript = spotifyManager.GetComponent<Spotify>();
        meshRenderer = GetComponent<MeshRenderer>();
        text = GameObject.Find("ArtistName");
        audioAnalysis = GameObject.Find("AudioAnalysis");
        artistNameText = text.GetComponent<UnityEngine.UI.Text>();
        audioAnalysisText = audioAnalysis.GetComponent<UnityEngine.UI.Text>();
    }

    public void updateCurrentlyPlaying(string artistID, string artistName, AudioAnalysis audioAnalysis)
    {
        this.artistID = artistID;
        artistNameText.text = artistName;
      //  audioAnalysis.Beats.ForEach(a => audioAnalysisText.text += ("Start: "+ a.Start) + ("Duration: " + a.Duration) + ("Confidence: " + a.Confidence));
      //  audioAnalysisText.text += audioAnalysis.Meta.ToString();
      //  audioAnalysis.Sections.ForEach(s => audioAnalysisText.text += s.ToString());
        audioAnalysisText.text += (" TimeSignature: " + audioAnalysis.Track.TimeSignature + "\n") + ("Tempo: " + audioAnalysis.Track.Tempo + "\n") + ("Mode: " + audioAnalysis.Track.Mode + "\n") + ("Key: " + audioAnalysis.Track.Key + "\n") + ("Duration: " + audioAnalysis.Track.Duration + "\n");
        //audioAnalysisText.text = audioAnalysis.Meta.ToString() + "/n" + ;
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
