﻿using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using UnityEngine;


public class ChartsScript : MonoBehaviour
{

    private GameObject thisGameObject;
    private MeshRenderer[] meshRenderers;
    private List<GameObject> quads;
   // private List<GameObject> popCubes;
    public GameObject[] popCubes;
    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private CSVReader csvReader;
    private AudioAnalysis[] audioAnalysisArray;
    public Paging<FullTrack> usersTopTracks;
    private ArrayList chartTracks = new ArrayList();
    public float streamsScaling = 0.00000001f;
    private const int MESHRENDERERSIZE = 10;

    // Use this for initialization
    void Start()
    {
        quads = GetChildGameObjectWithTag("song");

  //      popCubes = GetChildGameObjectWithTag("popularity");

        meshRenderers = GetMeshRenderers(quads);

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        csvReader = spotifyManager.GetComponent<CSVReader>();

        chartTracks = csvReader.chartTrackList;

        StartCoroutine(LoadChartTracks());
    }

    private List<GameObject> GetChildGameObjectWithTag(string comparativeTag)
    {
        Transform[] tempTransforms = GetComponentsInChildren<Transform>();
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (var t in tempTransforms)
        {
            if (t.tag == comparativeTag)
            {
                gameObjects.Add(t.gameObject);
            }
        }

        return gameObjects;
    }

    private MeshRenderer[] GetMeshRenderers(List<GameObject> gameObjects)
    {
        MeshRenderer[] meshRenderers = new MeshRenderer[MESHRENDERERSIZE];

        for (int i = 0; i < gameObjects.Count; i++)
        {
            meshRenderers[i] = gameObjects[i].GetComponent<MeshRenderer>();
        }

        return meshRenderers;
    }

    private IEnumerator LoadChartTracks()
    {
        yield return new WaitForSeconds(2);

        if (chartTracks == null)
        {
            Debug.LogError("Chart tracks is null");
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                //i+1 because first row is header names
                ChartTrack chartTrack = (ChartTrack) chartTracks[i+1];
                FullTrack fullTrack = spotifyManagerScript.GetTrack(chartTrack.apiUrl);

                if (fullTrack.HasError())
                {
                    Debug.LogError(fullTrack.Error.Status);
                    Debug.LogError(fullTrack.Error.Message);
                }
                else
                {
                    string chartTrackImageURL = fullTrack.Album.Images[0].Url;

                    GameObject meshRendererGameObject = meshRenderers[i].transform.gameObject;

                    PlaylistScript playlistScript = meshRendererGameObject.GetComponent<PlaylistScript>();

                    WWW imageURLWWW = new WWW(chartTrackImageURL);

                    yield return imageURLWWW;

                    meshRenderers[i].material.mainTexture = imageURLWWW.texture;

                    playlistScript.setFullTrack(fullTrack);
                    playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);

                    Vector3 v = popCubes[i].transform.localScale;
                    popCubes[i].transform.localScale = new Vector3(v.x, float.Parse(chartTrack.streams) * streamsScaling, v.z);
                }
            }
        }
    }

    public IEnumerator loadTopTracks()
    {
        //TODO subscribe to spotify manager event of csv being loaded
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
                playlistScript.sprite = ConvertWWWToSprite(imageURLWWW);
            }
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