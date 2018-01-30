using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class TopArtistsScript : MonoBehaviour
{

    private GameObject thisGameObject;
    public MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    public Paging<FullArtist> usersTopArtists;
    public SaveLoad saveLoad;
    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

    //    StartCoroutine(loadTopArtists());

    //    LoadTopArtistsFromFile();
    }

    public IEnumerator loadTopArtists()
    {
        //TODO subscribe to spotify manager event of authorization being complete
        yield return new WaitForSeconds(2);
        usersTopArtists = spotifyManagerScript.GetUsersTopArtists();
        if (usersTopArtists == null)
        {
            Debug.LogError("usersTopArtists is null");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string featuredPlaylistImageURL = usersTopArtists.Items[i].Images[0].Url;

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();
                //  playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);

                WWW imageURLWWW = new WWW(featuredPlaylistImageURL);

                yield return imageURLWWW;

                meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                playlistScript.setPlaylistName(usersTopArtists.Items[i].Name);
                playlistScript.setPlaylistURI(usersTopArtists.Items[i].Uri);
                playlistScript.fullArtist = usersTopArtists.Items[i];
                playlistScript.artistId = usersTopArtists.Items[i].Id;
                playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                saveLoad.SaveTextureToFilePNG(ConvertWWWToTexture(imageURLWWW), "topArtist" + i + ".png");
                saveLoad.savedTopArtists.Add(new PlaylistScriptData(playlistScript));
             //   saveLoad.QuickSaveSpriteToFile(playlistScript.sprite, "topArtistSprite" + i);
            }
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

    private Texture2D ConvertWWWToTexture(WWW www)
    {
        //TODO look at what texture format is best to use
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);
        // Debug.Log("Texture width " + www.texture.width + ", texture height " + www.texture.height);    

        return texture;
    }

    private Sprite ConvertTextureToSprite(Texture2D texture)
    {
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 1);
    }

    public void LoadTopArtistsFromFile()
    {
     //   saveLoad.Load();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedTopArtists[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Sprite sprite = saveLoad.QuickLoadSpriteFromFile("topArtistSprite" + i);

            meshRenderers[i].material.mainTexture = sprite.texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = saveLoad.QuickLoadSpriteFromFile("topArtistSprite" + i);
        }
    }

    public void LoadTopArtistsFromFilePNG()
    {
        //TODO take this out
        //   saveLoad.Load();

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        spotifyManager = GameObject.Find("SpotifyManager");
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedTopArtists[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Texture2D texture = saveLoad.LoadTextureFromFilePNG("topArtist" + i + ".png");

            meshRenderers[i].material.mainTexture = texture;

            playlistScript.artistId = playlistScriptLoaded.artistId;
            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = ConvertTextureToSprite(texture);
        }
    }
}
