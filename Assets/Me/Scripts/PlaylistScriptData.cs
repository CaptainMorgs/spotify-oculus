using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web.Models;
using UnityEngine;

[System.Serializable]
public class PlaylistScriptData  {

    public string playlistName, playlistURI, artistName, artistId;

    public int popularity;
   // public SimplePlaylist simplePlaylist;
   // public FullTrack fullTrack;
   // public SimpleAlbum simpleAlbum;
   // public FullArtist fullArtist;
   // public AudioAnalysis audioAnalysis;
 //   public UnityEngine.UI.Image image;
 //   public Sprite sprite;
 //   public FullAlbum fullAlbum;
 //   public WWW www;

    public PlaylistScriptData(PlaylistScript playlistScript)
    {
        playlistName = playlistScript.playlistName;
        playlistURI = playlistScript.playlistURI;
        artistName = playlistScript.artistName;
        popularity = playlistScript.popularity;
        if (playlistScript.artistId != null)
        {
            artistId = playlistScript.artistId;
        }
        //   simplePlaylist = playlistScript.simplePlaylist;
        //   fullTrack = playlistScript.fullTrack;
        //   simpleAlbum = playlistScript.simpleAlbum;
        //   fullArtist = playlistScript.fullArtist;
        //   audioAnalysis = playlistScript.audioAnalysis;
        //    image = playlistScript.image;
        //     sprite = playlistScript.sprite;
        //     fullAlbum = playlistScript.fullAlbum;
        //     www = playlistScript.www;
    }
}
