using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;
using UnityEditor;

public class TopTracksScript : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = thisGameObject.GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();

        StartCoroutine(loadTopTracks());
    }

    public IEnumerator loadTopTracks()
    {
        //TODO subscribe to spotify manager event of authorization being complete
        yield return new WaitForSeconds(2);
        Paging<FullTrack> usersTopTracks = spotifyManagerScript.GetUsersTopTracks();
        if (usersTopTracks == null)
        {
            Debug.LogError("usersTopTracks is null");

        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string featuredPlaylistImageURL = usersTopTracks.Items[i].Album.Images[0].Url;

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();
                //  playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);

                WWW imageURLWWW = new WWW(featuredPlaylistImageURL);

                yield return imageURLWWW;

                meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                playlistScript.setPlaylistName(usersTopTracks.Items[i].Name);
                playlistScript.setPlaylistURI(usersTopTracks.Items[i].Uri);
            }
        }
    }
}
