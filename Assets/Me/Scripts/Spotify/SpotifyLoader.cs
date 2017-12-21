using System.Collections;
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

public class SpotifyLoader : MonoBehaviour {

	public GameObject prefab;

	private static SpotifyWebAPI _spotify = Spotify._spotify;

	// Use this for initialization
	void Start () {
		//StartCoroutine (loadPlaylistsObjectsWithPrefab ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator loadPlaylistsObjectsWithPrefab() {
		FeaturedPlaylists playlists = _spotify.GetFeaturedPlaylists();

		if (playlists != null) {
			Debug.Log (playlists.ToString ());
		} else {
			Debug.Log ("Null Playlists");
		}

		for (int i = 0; i < playlists.Playlists.Items.Count; i++) {
			SimplePlaylist playlist = playlists.Playlists.Items [i];

			GameObject gameObject = Instantiate (prefab, new Vector3 (((i * 1)-10), 0.2f, 3), Quaternion.identity);

			string playlistImageURL = playlist.Images [0].Url;

			WWW imageURLWWW = new WWW(playlistImageURL);

			yield return imageURLWWW;

			GameObject gameObjectQuad = gameObject.transform.GetChild (0).gameObject;
			Renderer renderer = gameObjectQuad.GetComponent<MeshRenderer> ();
			renderer.material.mainTexture = imageURLWWW.texture;

			UnityEngine.UI.Text playlistName = gameObject.GetComponentInChildren<UnityEngine.UI.Text> ();
			playlistName.text = playlist.Name;

			UnityEngine.UI.Text playlistDescription = gameObject.GetComponentInChildren<UnityEngine.UI.Text> ();
			playlistDescription.text = playlist.SnapshotId;
		}
	}
}
