using System.Collections;
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
    public SaveLoad saveLoad;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

     //   StartCoroutine(loadNewAlbumReleases());

     //   LoadNewReleasesFromFile();
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
            playlistScript.albumId = newAlbumReleases.Albums.Items[i].Id;          
            playlistScript.simpleAlbum = newAlbumReleases.Albums.Items[i];
            playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
            saveLoad.SaveTextureToFilePNG(ConvertWWWToTexture(imageURLWWW), "newReleases" + i + ".png");
            saveLoad.savedNewReleases.Add(new PlaylistScriptData(playlistScript));
            //   saveLoad.QuickSaveSpriteToFile(playlistScript.sprite, "newReleasesSprite" + i);

            yield return new WaitForSeconds(10);
            saveLoad.Save();
        }
    }
    //TODO fix code duplication 
    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteToUse;
    }

    public void LoadNewReleasesFromFile()
    {
    //    saveLoad.Load();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedNewReleases[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Sprite sprite = saveLoad.QuickLoadSpriteFromFile("newReleasesSprite" + i);

            meshRenderers[i].material.mainTexture = sprite.texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = saveLoad.QuickLoadSpriteFromFile("newReleasesSprite" + i);
            playlistScript.albumId = playlistScriptLoaded.albumId;

        }
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

    public void LoadNewReleasesFromFilePNG()
    {
        //TODO take this out
        //   saveLoad.Load();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedNewReleases[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Texture2D texture = saveLoad.LoadTextureFromFilePNG("newReleases" + i + ".png");

            meshRenderers[i].material.mainTexture = texture;

            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = ConvertTextureToSprite(texture);
            playlistScript.albumId = playlistScriptLoaded.albumId;
        }
    }
}
