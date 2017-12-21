using SpotifyAPI.Web.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchResultsScript : MonoBehaviour {

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
    }

    public IEnumerator LoadSearchResults(SearchItem searchItem)
    {
  
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string searchItemImageURL = searchItem.Albums.Items[i].Images[0].Url;

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();
                //  playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);

                WWW imageURLWWW = new WWW(searchItemImageURL);

                yield return imageURLWWW;

                meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                playlistScript.setPlaylistName(searchItem.Albums.Items[i].Name);
                playlistScript.setPlaylistURI(searchItem.Albums.Items[i].Uri);
            }
        }
    }
}
