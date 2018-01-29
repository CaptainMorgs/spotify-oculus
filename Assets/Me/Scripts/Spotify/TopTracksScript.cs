using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class TopTracksScript : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private AudioAnalysis[] audioAnalysisArray;
    public Paging<FullTrack> usersTopTracks;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = thisGameObject.GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();

        //  StartCoroutine(loadTopTracks());

        LoadTopTracksFromFile();
    }

    public IEnumerator loadTopTracks()
    {
        //TODO subscribe to spotify manager event of authorization being complete
        yield return new WaitForSeconds(2);
        usersTopTracks = spotifyManagerScript.GetUsersTopTracks();
        if (usersTopTracks == null || usersTopTracks.Items.Count == null)
        {
            Debug.LogError("usersTopTracks is null/empty");

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
                playlistScript.setFullTrack(usersTopTracks.Items[i]);
                playlistScript.audioAnalysis = spotifyManagerScript.GetAudioAnalysis(usersTopTracks.Items[i].Id);
                playlistScript.www = imageURLWWW;
                playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                SaveLoad.savedTopTracks.Add(new PlaylistScriptData(playlistScript));
                Debug.Log("Adding playlist " + i);
                SaveLoad.QuickSaveSpriteToFile(playlistScript.sprite, "sprite" + i);
            }
            SaveLoad.Save();
        }
    }

    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteToUse;
    }

    private void LoadTopTracksFromFile()
    {
        SaveLoad.Load();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = SaveLoad.savedTopTracks[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Sprite sprite = SaveLoad.QuickLoadSpriteFromFile("sprite" + i);

            meshRenderers[i].material.mainTexture = sprite.texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = SaveLoad.QuickLoadSpriteFromFile("sprite" + i);
        }
    }
}
