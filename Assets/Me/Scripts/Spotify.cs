﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.IO;

public class Spotify : MonoBehaviour {

	public static SpotifyWebAPI _spotify;

	private static string clientId = "4dd553a707024f8bb4f210bb86d73ee1";

	private static string clientSecret = "c02f072189ad4f21a7d04f7370574f7b";

	private static string redirectUriLocal = "http://localhost";

	private string rickAstleyAlbum = "6N9PS4QXF1D0OWPk0Sxtb4";

	public Image albumImage;

	public GameObject FeaturedPlaylistTab, CurrentSongGameObject;

	private FeaturedPlaylistTabScript featuredPlaylistTabScript;

    private CurrentSong currentSongScript;

    public GameObject prefab;

   

	// Use this for initialization
	void Start () {
		ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

		ImplicitGrantAuth();

		featuredPlaylistTabScript = FeaturedPlaylistTab.GetComponent<FeaturedPlaylistTabScript> ();

        currentSongScript = CurrentSongGameObject.GetComponent<CurrentSong>();

        StartCoroutine(featuredPlaylistTabScript.loadStuff ());

        RestCallTest();


    }

	// Update is called once per frame
	void Update () {
	}

    public void RestCallTest()
    {
        string endPoint = @"https://api.spotify.com/v1/me/top/artists";
        var request = (HttpWebRequest)WebRequest.Create(endPoint);

        request.Method = "GET";

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            var responseValue = string.Empty;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }

            // grab the response
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
            }

            var json = responseValue;
            Debug.Log(responseValue);
        }
    }

    public FeaturedPlaylists GetFeaturedPlaylists() {
        FeaturedPlaylists playlists = _spotify.GetFeaturedPlaylists();
        return playlists;
    }

    public FullArtist GetFullArtist(string artistID) {
        FullArtist artist = _spotify.GetArtist(artistID);
        return artist;
    }

	public void playSong(string songID) {
		PlaybackContext context = _spotify.GetPlayback ();	
		ErrorResponse error = _spotify.ResumePlayback(uris: new List<string> {context.Device.Id, "spotify:track:4iV5W9uYEdYUVa79Axb7Rh" });

		if (error.Error != null) {
			Debug.LogError (error.Error.Message);
		}
	}

	public void playPlaylist(string playlistURI) {
		PlaybackContext context = _spotify.GetPlayback ();	
		ErrorResponse error = _spotify.ResumePlayback( context.Device.Id, playlistURI );
		Debug.Log ("PlaylistURI: " + playlistURI);
		if (error.Error != null) {
			Debug.LogError (error.Error.Message);
		}
        context = _spotify.GetPlayback();
        if (context.Item != null) {
            Debug.Log("Currently playing song: " + context.Item.Name);
            currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id);

        }
    }

	public void resumePlayback() {
		PlaybackContext context = _spotify.GetPlayback ();	
		//	ErrorResponse error = _spotify.PausePlayback(context.Device.Id);
		ErrorResponse error = _spotify.ResumePlayback(context.Device.Id);
		if (error.Error != null) {
			Debug.Log (error.Error.Status);
			Debug.Log (error.Error.Message);
		}
        if (context.Item != null)
        {
            Debug.Log("Currently playing song: " + context.Item.Name);
            Debug.Log("Artist: " + context.Item.Artists[0].Name);
            currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id);
        }

    }

    public void pausePlayback() {
		PlaybackContext context = _spotify.GetPlayback ();	
		//	ErrorResponse error = _spotify.PausePlayback(context.Device.Id);
		ErrorResponse error = _spotify.PausePlayback(context.Device.Id);
		if (error.Error != null) {
			Debug.Log (error.Error.Status);
			Debug.Log (error.Error.Message);
		}
	}

    public void searchSpotify(string searchQuery) {
        SearchItem searchItem = _spotify.SearchItems(searchQuery, SearchType.All);

        if (searchItem != null) { 
        searchItem.Albums.Items.ForEach(item => Debug.Log("Album: " + item.Name));
        searchItem.Tracks.Items.ForEach(item => Debug.Log("Track: " + item.Name));
        searchItem.Playlists.Items.ForEach(item => Debug.Log("Playlist: " + item.Name));

        StartCoroutine(loadObjectsFromSearch(searchItem));
        }
        else {
            Debug.LogError("Null search query");
        }
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
		return true;
	}

	async void ImplicitGrantAuth() {
		WebAPIFactory webApiFactory = new WebAPIFactory(
			redirectUriLocal,
			8080,
			clientId,
			Scope.UserModifyPlaybackState,
			TimeSpan.FromSeconds(20)
		);

		try
		{
			//This will open the user's browser and returns once
			//the user is authorized
			Debug.Log("Redirect URI: "+ redirectUriLocal);
			_spotify = await webApiFactory.GetWebApi();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message.ToString());
		}

		if (_spotify == null)
			return;
	}
	
	private void getContext() {
		PlaybackContext context = _spotify.GetPlayback ();	
		if (context.Item != null) {
			Debug.Log ("Device: " + context.Device.Name); 
		}
		if (context.Item != null) {
			Debug.Log ("Context: " + context.Item.Name); 
		} else {
			Debug.Log ("Context null");
		}
	}

	private IEnumerator loadObjectsFromSearch(SearchItem searchItem) {

        searchItem.Albums.Items.ForEach(item => Debug.Log("Album: " + item.Name));
        searchItem.Tracks.Items.ForEach(item => Debug.Log("Track: " + item.Name));
        searchItem.Playlists.Items.ForEach(item => Debug.Log("Playlist: " + item.Name));

        if (searchItem != null) {
			Debug.Log (searchItem.ToString ());
		} else {
			Debug.Log ("Null searchItem");
		}

        int numRows = (int)(searchItem.Playlists.Items.Count / 3);
        int k = 0;
        for (int j = 0; j < numRows; j++)
        {


            for (int i = 0; i < 6; i++)
            {

                SimplePlaylist playlist = searchItem.Playlists.Items[k];

                GameObject gameObject = Instantiate(prefab, new Vector3(4, (j * 1) + 0.75f, ((i * 1) - 3)), Quaternion.AngleAxis(90, Vector3.up));
                
                string playlistImageURL = playlist.Images[0].Url;

                WWW imageURLWWW = new WWW(playlistImageURL);

                yield return imageURLWWW;

                GameObject gameObjectQuad = gameObject.transform.GetChild(0).gameObject;
                Renderer renderer = gameObjectQuad.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = imageURLWWW.texture;

                PlaylistScript playlistScript = gameObject.GetComponent<PlaylistScript>();
                playlistScript.setPlaylistName(playlist.Name);
                playlistScript.setPlaylistURI(playlist.Uri);
                Debug.Log("Setting playlist uri to : " + playlist.Uri);

           //     UnityEngine.UI.Text playlistName = gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
          //      playlistName.text = playlist.Name;

           //     UnityEngine.UI.Text playlistDescription = gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
          //      playlistDescription.text = playlist.SnapshotId;
                k++;
            }
        }
    }


    private IEnumerator loadPlaylistsObjectsWithPrefab2()
    {
        FeaturedPlaylists playlists = _spotify.GetFeaturedPlaylists();

        if (playlists != null)
        {
            Debug.Log(playlists.ToString());
        }
        else
        {
            Debug.Log("Null Playlists");
        }

        int numRows = (int)(playlists.Playlists.Items.Count / 3);
        int k = 0;
        for (int j = 0; j < numRows; j++) {

            
            for (int i = 0; i < 6; i++)
            {
                
                SimplePlaylist playlist = playlists.Playlists.Items[k];

                GameObject gameObject = Instantiate(prefab, new Vector3(((i * 1) - 2), (j*1) + 0.75f, 3), Quaternion.identity);

                string playlistImageURL = playlist.Images[0].Url;

                WWW imageURLWWW = new WWW(playlistImageURL);

                yield return imageURLWWW;

                GameObject gameObjectQuad = gameObject.transform.GetChild(0).gameObject;
                Renderer renderer = gameObjectQuad.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = imageURLWWW.texture;

                PlaylistScript playlistScript = gameObject.GetComponent<PlaylistScript>();
                playlistScript.setPlaylistName(playlist.Name);
                playlistScript.setPlaylistURI(playlist.Uri);
                Debug.Log("Setting playlist uri to : " + playlist.Uri);

           //     UnityEngine.UI.Text playlistName = gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
           //     playlistName.text = playlist.Name;

           //     UnityEngine.UI.Text playlistDescription = gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
           //     playlistDescription.text = playlist.SnapshotId;
                k++;
            }
        }
    }
}