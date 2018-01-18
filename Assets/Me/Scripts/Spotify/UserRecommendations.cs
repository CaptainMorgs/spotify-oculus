﻿using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class UserRecommendations : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    public GameObject topTracksGameObject, topArtistsGameObject;
    private TopTracksScript topTracksScript;
    private TopArtistsScript topArtistsScript;
    private Paging<FullTrack> usersTopTracks;
    private Paging<FullArtist> usersTopArtists;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = thisGameObject.GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();

        topTracksScript = topTracksGameObject.GetComponent<TopTracksScript>();

        topArtistsScript = topArtistsGameObject.GetComponent<TopArtistsScript>();

        StartCoroutine(LoadUserRecommendations());
    }

    public IEnumerator LoadUserRecommendations()
    {
        //TODO subscribe to spotify manager event of authorization being complete and top tracks being complete
        yield return new WaitForSeconds(10);

        usersTopTracks = topTracksScript.usersTopTracks;
        usersTopArtists = topArtistsScript.usersTopArtists;

        Recommendations recommendations = spotifyManagerScript.GetUserRecommendations(usersTopTracks, usersTopArtists);
        if (recommendations.HasError())
        {
            Debug.LogError(recommendations.Error.Status + " " + recommendations.Error.Message);

        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                FullTrack fullTrack = spotifyManagerScript.GetTrack(recommendations.Tracks[i].Id);

                string recommendationsImageURL = fullTrack.Album.Images[0].Url;

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();
                //  playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);

                WWW imageURLWWW = new WWW(recommendationsImageURL);

                yield return imageURLWWW;

                meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                playlistScript.setPlaylistName(fullTrack.Name);
                playlistScript.setPlaylistURI(fullTrack.Uri);
            }
        }
    }
}