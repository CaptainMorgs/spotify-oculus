using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;

//TODO save popularity
public class UsersFollowedArtists : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private SaveLoad saveLoad;

    // Use this for initialization
    void Start()
    {
        thisGameObject = transform.root.gameObject;
        meshRenderers = thisGameObject.GetComponentsInChildren<MeshRenderer>();

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

      //  StartCoroutine(LoadUsersFollowedArtists());
    }

    public IEnumerator LoadUsersFollowedArtists()
    {
        //TODO subscribe to spotify manager event of authorization being complete
        yield return new WaitForSeconds(2);
        FollowedArtists followedArtists = spotifyManagerScript.GetUsersFollowedArtists();
        if (followedArtists == null)
        {
            Debug.LogError("followedArtists is null");

        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                string followedArtistsImageURL = followedArtists.Artists.Items[i].Images[0].Url;

                GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();
                //  playlistScript.setPlaylistURI(featuredPlaylists.Playlists.Items[i].Uri);

                WWW imageURLWWW = new WWW(followedArtistsImageURL);

                yield return imageURLWWW;

                meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                playlistScript.setPlaylistName(followedArtists.Artists.Items[i].Name);
                playlistScript.setPlaylistURI(followedArtists.Artists.Items[i].Uri);
                playlistScript.fullArtist = followedArtists.Artists.Items[i];
                playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
                playlistScript.artistId = followedArtists.Artists.Items[i].Id;
                saveLoad.SaveTextureToFilePNG(Converter.ConvertWWWToTexture(imageURLWWW), "userFollowedArtist" + i + ".png");
                saveLoad.savedUserFollowedArtists.Add(new PlaylistScriptData(playlistScript));
              //  Debug.Log(" followed artist running " + i);

            }

            yield return new WaitForSeconds(10);
            saveLoad.Save();
        }
    }

    public void LoadUserFollowedArtistsFromFilePNG()
    {
        //TODO take this out
        //   saveLoad.Load();

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        spotifyManager = GameObject.Find("SpotifyManager");
        saveLoad = spotifyManager.GetComponent<SaveLoad>();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            PlaylistScriptData playlistScriptLoadedData = saveLoad.savedUserFollowedArtists[i];

            PlaylistScript playlistScriptLoaded = new PlaylistScript(playlistScriptLoadedData);

            GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

            PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

            Texture2D texture = saveLoad.LoadTextureFromFilePNG("userFollowedArtist" + i + ".png");

            meshRenderers[i].material.mainTexture = texture;

            playlistScript.artistId = playlistScriptLoaded.artistId;
            playlistScript.setPlaylistName(playlistScriptLoaded.playlistName);
            playlistScript.setPlaylistURI(playlistScriptLoaded.playlistURI);
            playlistScript.artistName = playlistScriptLoaded.artistName;
            playlistScript.sprite = Converter.ConvertTextureToSprite(texture);
        }
    }

    private Sprite ConvertWWWToSprite(WWW www)
    {

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteToUse;
    }
}
