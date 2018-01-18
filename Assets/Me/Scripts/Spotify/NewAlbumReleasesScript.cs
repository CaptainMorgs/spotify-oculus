﻿using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class NewAlbumReleasesScript : MonoBehaviour
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

        StartCoroutine(loadNewAlbumReleases());
    }

    public IEnumerator loadNewAlbumReleases()
    {
        //TODO subscribe to spotify manager event of authorization being complete
        yield return new WaitForSeconds(2);
        NewAlbumReleases newAlbumReleases = spotifyManagerScript.GetNewAlbumReleases();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            string featuredPlaylistImageURL = newAlbumReleases.Albums.Items[i].Images[0].Url;

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();
            //  playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);

            WWW imageURLWWW = new WWW(featuredPlaylistImageURL);

            yield return imageURLWWW;

            meshRenderers[i].material.mainTexture = imageURLWWW.texture;

            playlistScript.setPlaylistName(newAlbumReleases.Albums.Items[i].Name);
            playlistScript.setPlaylistURI(newAlbumReleases.Albums.Items[i].Uri);
            playlistScript.simpleAlbum = newAlbumReleases.Albums.Items[i];
            playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
        }
    }
    //TODO fix code duplication 
    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteToUse;
    }
}