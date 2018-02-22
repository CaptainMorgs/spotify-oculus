using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SpotifyAPI.Web.Models;

public class RecommenderDeck : MonoBehaviour
{

    public List<GameObject> recommendationSeeds;

    public List<GameObject> activeSeeds;

    public UserRecommendations userRecommendations;

    private GameObject spotifyManager;

    private Spotify spotifyManagerScript;


    // Use this for initialization
    void Start()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetRecommendations()
    {
        Debug.Log("GetRecommendations called");
        if (activeSeeds.Count > 0)
        {

            List<string> seedIds = new List<string>();

            foreach (var seed in activeSeeds)
            {
                PlaylistScript playlistScript = seed.GetComponent<VinylScript>().playlistScript;
                if (playlistScript.trackType == PlaylistScript.TrackType.artist)
                {
                    seedIds.Add(playlistScript.artistId);
                }
                //TODO more elegent solution than using the top track of each playlist? Use genre maybe?
                else if (playlistScript.trackType == PlaylistScript.TrackType.playlist)
                {

                    FullPlaylist fullPlaylist = spotifyManagerScript.GetPlaylist(playlistScript.ownerId, playlistScript.playlistId);
                    seedIds.Add(fullPlaylist.Tracks.Items[0].Track.Artists[0].Id);

                }              
                else
                {
                    Debug.LogError("only supports artists and playlists right now");
                }


            }
            if (seedIds.Count != 0)
            {
                StartCoroutine(userRecommendations.LoadUserRecommendations(seedIds));
            }
            else
            {
                Debug.LogError("Seed Id list is empty");
            }
        }
    }
}
