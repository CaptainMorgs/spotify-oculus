using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web.Models;
using UnityEngine;

[System.Serializable]
public class PlaylistScriptData
{

    public string playlistName, playlistURI, artistName, artistId, ownerId, playlistId, albumId, trackId;

    public int popularity;

    public PlaylistScriptData(PlaylistScript playlistScript)
    {
        playlistName = playlistScript.playlistName;
        playlistURI = playlistScript.playlistURI;
        artistName = playlistScript.artistName;
        popularity = playlistScript.popularity;
        ownerId = playlistScript.ownerId;
        playlistId = playlistScript.playlistId;
        albumId = playlistScript.albumId;
        trackId = playlistScript.trackId;
        artistId = playlistScript.artistId;
    }
}
