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
using System.IO;
using System.Threading;

public class Spotify : MonoBehaviour
{

    public static SpotifyWebAPI _spotify;
    public GameObject playlistPrefab, recordPlayer, FeaturedPlaylistTab, CurrentSongGameObject, searchResultsTab, audioVisualizer;
    private static string clientId = "4dd553a707024f8bb4f210bb86d73ee1";
    private static string redirectUriLocal = "http://localhost";
    private FeaturedPlaylistTabScript featuredPlaylistTabScript;
    private CurrentSong currentSongScript;
    private RecordPlayer recordPlayerScript;
    private bool waitingOnRestCall = false;
    private SearchResultsScript searchResultsScript;
    private AudioVisualizer audioVisualizerScript;
    public ParticleVisualizer particleVisualizer;
    private PlaybackContext context;
    private PrivateProfile privateProfile;
    private bool shuffleState, threadRunning;
    private RepeatState repeatState;
    private AudioAnalysis audioAnalysis;
    public delegate void ClickAction();
    public static event ClickAction OnClicked;


    //TODO save users followed artists/top tracks etc between sessions
    //TODO ADD A QUEING/UP NEXT FEATURE WHERE YOU PLACE RECORDS PHYSICALLY IN A QUEUE, may not be possible    
    //TODO add browser that shows music videos
    //TODO playlist are in wrong order
    //TODO alter speakers scale to increase/decrease volume
    //TODO playlist manager
    //TODO follow/unfollow artist with physical motion

    // Use this for initialization
    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        ImplicitGrantAuth();

        context = _spotify.GetPlayback();

        shuffleState = context.ShuffleState;

        repeatState = context.RepeatState;

        privateProfile = _spotify.GetPrivateProfile();

        audioVisualizer = GameObject.Find("AudioVisualizer");

        audioVisualizerScript = audioVisualizer.GetComponent<AudioVisualizer>();

        featuredPlaylistTabScript = FeaturedPlaylistTab.GetComponent<FeaturedPlaylistTabScript>();

        searchResultsScript = searchResultsTab.GetComponent<SearchResultsScript>();

        currentSongScript = CurrentSongGameObject.GetComponent<CurrentSong>();

        recordPlayerScript = recordPlayer.GetComponent<RecordPlayer>();

        //Ignore collisions between character controller and vinyls
        Physics.IgnoreLayerCollision(8, 9);

        OnClicked += SendAudioAnaylisToParticleVisualizer;
    }

    // Update is called once per frame
    void Update()
    {
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

    public FeaturedPlaylists GetFeaturedPlaylists()
    {
        FeaturedPlaylists playlists = _spotify.GetFeaturedPlaylists();
        return playlists;
    }

    public FullAlbum GetAlbum(string id)
    {
        FullAlbum fullAlbum = _spotify.GetAlbum(id);
        return fullAlbum;
    }

    public NewAlbumReleases GetNewAlbumReleases()
    {
        NewAlbumReleases newAlbumReleases = _spotify.GetNewAlbumReleases(privateProfile.Country, 9, 0);
        return newAlbumReleases;
    }
    //TODO use editor input var for time range
    public Paging<FullTrack> GetUsersTopTracks()
    {
        Paging<FullTrack> usersTopTracks = _spotify.GetUsersTopTracks(TimeRangeType.ShortTerm, 9, 0);
        return usersTopTracks;
    }

    /// <summary>
    /// Get user recommendations based on seeds of their top tracks and artists
    /// </summary>
    /// <param name="usersTopTracks"></param>
    /// <param name="usersTopArtists"></param>
    /// <returns></returns>
    public Recommendations GetUserRecommendations(Paging<FullTrack> usersTopTracks, Paging<FullArtist> usersTopArtists)
    {
        List<String> trackIds = new List<String>();

        List<String> artistIds = new List<String>();

        List<String> genres = new List<String>();

        //TODO what if the user doesn't have 5 top tracks, artists (possible?)
        for (int i = 0; i < 5; i++)
        {
            trackIds.Add(usersTopTracks.Items[i].Id);
            //    artistIds.Add(usersTopTracks.Items[i].Artists[0].Id);
            //    genres.Add(usersTopArtists.Items[i].Genres[0]);
        }

        Recommendations usersRecommendations = _spotify.GetRecommendations(trackIds);

        return usersRecommendations;
    }

    public Recommendations GetUserRecommendations(string artistId)
    {
        List<String> trackIds = new List<String>();

        List<String> artistIds = new List<String>();

        List<String> genres = new List<String>();

        artistIds.Add(artistId);

        //TODO what if the user doesn't have 5 top tracks, artists (possible?)
        for (int i = 0; i < 5; i++)
        {
      //      trackIds.Add(savedTopTracks[i].);
            //    artistIds.Add(usersTopTracks.Items[i].Artists[0].Id);
            //    genres.Add(usersTopArtists.Items[i].Genres[0]);
        }

        Recommendations usersRecommendations = _spotify.GetRecommendations(artistIds);

        if (usersRecommendations.HasError())
        {
            Debug.LogError(usersRecommendations.Error.Message);
            Debug.LogError(usersRecommendations.Error.Status);

        }

        return usersRecommendations;
    }

    public Recommendations GetRecommendations(List<string> idList)
    {
              
        Recommendations usersRecommendations = _spotify.GetRecommendations(idList);

        if (usersRecommendations.HasError())
        {
            Debug.LogError(usersRecommendations.Error.Message);
            Debug.LogError(usersRecommendations.Error.Status);         
        }

        return usersRecommendations;
    }

    public Recommendations GetRecommendationsWithTrack(List<string> trackIdList)
    {

        Recommendations usersRecommendations = _spotify.GetRecommendations(null, null, trackIdList);

        if (usersRecommendations.HasError())
        {
            Debug.LogError(usersRecommendations.Error.Message);
            Debug.LogError(usersRecommendations.Error.Status);
        }

        return usersRecommendations;
    }



    public Paging<FullArtist> GetUsersTopArtists()
    {
        Paging<FullArtist> usersTopArtists = _spotify.GetUsersTopArtists(TimeRangeType.ShortTerm, 9, 0);
        return usersTopArtists;
    }

    public FullArtist GetFullArtist(string artistID)
    {
        FullArtist artist = _spotify.GetArtist(artistID);
        return artist;
    }

    public SeveralTracks GetArtistsTopTracks(string artistID)
    {
        SeveralTracks artistTopTracks = _spotify.GetArtistsTopTracks(artistID, privateProfile.Country);
        return artistTopTracks;
    }

    //TODO implement threading
    public FullPlaylist GetPlaylist(string userId, string playlistId) {

        FullPlaylist fullPlaylist = _spotify.GetPlaylist(userId, playlistId);

        if (fullPlaylist.HasError()) {
            Debug.LogError(fullPlaylist.Error.Message);
            Debug.LogError(fullPlaylist.Error.Status);
        }

        return fullPlaylist;
    }


    public void playSong(string songID)
    {
        PlaybackContext context = _spotify.GetPlayback();
        ErrorResponse error = _spotify.ResumePlayback(uris: new List<string> { context.Device.Id, "spotify:track:4iV5W9uYEdYUVa79Axb7Rh" });
        // AudioAnalysis audioAnalysis = _spotify.GetAudioAnalysis(context.Item.Id);
        // audioVisualizerScript.SendAnalysis(audioAnalysis);
        currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name);

        if (error.Error != null)
        {
            Debug.LogError(error.Error.Message);
        }
    }

    public AudioAnalysis GetAudioAnalysis(String id)
    {
        return _spotify.GetAudioAnalysis(id);
    }

    public Paging<SimplePlaylist> GetUsersPlayists()
    {
        return _spotify.GetUserPlaylists(privateProfile.Id, 9, 0);
    }

    public FollowedArtists GetUsersFollowedArtists()
    {
        return _spotify.GetFollowedArtists(FollowType.Artist, 9);
    }

    public FullTrack GetTrack(String id)
    {
        return _spotify.GetTrack(id);
    }

    public void PlayUri(string playlistURI)
    {
        Thread myThread = new Thread(() => PlayURIThread(playlistURI));
        myThread.Start();
        Debug.Log("PlayURIThread finished");
    }

    public void PlayURIThread(string playlistURI)
    {

        PlaybackContext context = _spotify.GetPlayback();

        //    float before = Time.realtimeSinceStartup;
        ErrorResponse error = _spotify.ResumePlayback(context.Device.Id, playlistURI);
        //     Debug.Log("Time taken to play URI: " + (Time.realtimeSinceStartup - before));
        recordPlayerScript.recordPlayerActive = true;

        if (error.Error != null)
        {
            Debug.LogError(error.Error.Status);
            Debug.LogError(error.Error.Message);
            Debug.LogError(playlistURI);
        }
        context = _spotify.GetPlayback();
        if (context.Item != null)
        {
            Debug.Log("Currently playing song: " + context.Item.Name);
            Debug.Log("Artist: " + context.Item.Artists[0].Name);
            //    AudioAnalysis audioAnalysis = _spotify.GetAudioAnalysis(context.Item.Id);
            //currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name, audioAnalysis);
            currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name);

            //    audioVisualizerScript.SendAnalysis(audioAnalysis);
        }

    }

    public void PlaySongUri(string songURI)
    {
        ThreadStart starter = new ThreadStart(() => PlaySongURIThread(songURI));

        //use callback to send audio analysis to particle visualizer
        starter += () =>
        {
           // OnClicked();
           // particleVisualizer.SendAnalysis(audioAnalysis);
        };

        Thread myThread = new Thread(starter);

        myThread.Start();
        threadRunning = true;

        while (threadRunning)
        {
            //TODO this can be better
        }

        SendAudioAnaylisToParticleVisualizer();

        //    Thread myThread = new Thread(() => PlaySongURIThread(songURI));

        //    myThread.Start();
        //    Debug.Log("playSongURIThread finished");
    }

    public void PlaySongURIThread(string songURI)
    {
       
        PlaybackContext context = _spotify.GetPlayback();

        ErrorResponse error = _spotify.ResumePlayback(context.Device.Id, uris: new List<string> { songURI });
        recordPlayerScript.recordPlayerActive = true;

        if (error.HasError())
        {
            Debug.LogError(error.Error.Message);
            Debug.LogError(songURI);
        }
        context = _spotify.GetPlayback();
        if (context.Item != null)
        {
            //TODO currently playing song is the previous song that was played
            Debug.Log("Currently playing song: " + context.Item.Name);
            Debug.Log("Artist: " + context.Item.Artists[0].Name);
            audioAnalysis = _spotify.GetAudioAnalysis(context.Item.Id);
            //currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name, audioAnalysis);
            //currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name);

            //audioVisualizerScript.SendAnalysis(audioAnalysis);
           
        }
        threadRunning = false;
    }

    private void SendAudioAnaylisToParticleVisualizer()
    {
        if (audioAnalysis != null)
        {
            particleVisualizer.SendAnalysis(audioAnalysis);
        }
    }

    public void ResumePlayback()
    {
        Thread myThread = new Thread(ResumePlaybackThread);
        myThread.Start();
        //   Debug.Log("Thread finished");
    }



    public void ResumePlaybackThread()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (!context.IsPlaying)
        {


            ErrorResponse error = _spotify.ResumePlayback(context.Device.Id);
            recordPlayerScript.recordPlayerActive = true;

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            if (context.Item != null)
            {
                Debug.Log("Currently playing song: " + context.Item.Name);
                Debug.Log("Artist: " + context.Item.Artists[0].Name);

                //AudioAnalysis audioAnalysis = _spotify.GetAudioAnalysis(context.Item.Id);
                //currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name, _spotify.GetAudioAnalysis(context.Item.Id));
                currentSongScript.updateCurrentlyPlaying(context.Item.Artists[0].Id, context.Item.Artists[0].Name);

                audioVisualizerScript.repeat = true;
                //audioVisualizerScript.SendAnalysis(audioAnalysis);

            }
        }
    }

    //TODO check if we need to get the current context and block skipping track if not currently playing
    public void SkipPlaybackToNext()
    {
        PlaybackContext context = _spotify.GetPlayback();

        // if (context.IsPlaying)
        //  {
        ErrorResponse error = _spotify.SkipPlaybackToNext();

        if (error.Error != null)
        {
            Debug.Log(error.Error.Message);
        }
        //   }
        //   else {
        //       Debug.Log("Can't skip playback to next if not currently playing");
        //   }
    }

    public void SkipPlaybackToPrevious()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.IsPlaying)
        {
            ErrorResponse error = _spotify.SkipPlaybackToPrevious();

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
        }
        else
        {
            Debug.Log("Can't skip playback to previous if not currently playing");
        }
    }

    public void SetShuffle()
    {
        if (shuffleState)
        {
            ErrorResponse error = _spotify.SetShuffle(false);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                shuffleState = false;
                Debug.Log("Shuffle state set to false");
            }
        }
        else
        {
            ErrorResponse error = _spotify.SetShuffle(true);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                shuffleState = true;
                Debug.Log("Shuffle state set to true");
            }
        }
    }

    public void SetRepeatMode()
    {
        if (repeatState == RepeatState.Off)
        {
            ErrorResponse error = _spotify.SetRepeatMode(RepeatState.Context);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                repeatState = RepeatState.Context;
                Debug.Log("Repeat state is context");
            }
        }
        else if (repeatState == RepeatState.Context)
        {
            ErrorResponse error = _spotify.SetRepeatMode(RepeatState.Track);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                repeatState = RepeatState.Track;
                Debug.Log("Repeat state is track");

            }
        }
        else if (repeatState == RepeatState.Track)
        {
            ErrorResponse error = _spotify.SetRepeatMode(RepeatState.Off);

            if (error.Error != null)
            {
                Debug.Log(error.Error.Message);
            }
            else
            {
                repeatState = RepeatState.Off;
                Debug.Log("Repeat state is off");

            }
        }
    }

    public void PausePlayback()
    {
        Thread myThread = new Thread(PausePlaybackThread);
        myThread.Start();
        //Debug.Log("Pause playback thread finished");
    }

    public void PausePlaybackThread()
    {
        PlaybackContext context = _spotify.GetPlayback();

        if (context.IsPlaying)
        {
            ErrorResponse error = _spotify.PausePlayback(context.Device.Id);
            recordPlayerScript.recordPlayerActive = false;
            audioVisualizerScript.repeat = false;

            if (error.Error != null)
            {
                Debug.LogError(error.Error.Status);
                Debug.LogError(error.Error.Message);
            }
        }
    }

    public void SaveAlbum(string id)
    {
        Thread myThread = new Thread(() => SaveAlbum(id));
        myThread.Start();
    }

    public void SaveAlbumThread(string id)
    {
        ErrorResponse errorResponse = _spotify.SaveAlbum(id);
        
        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Saved album " + id);
        }
    }

    public void FollowPlaylist(string ownerId, string playlistId)
    {
        Thread myThread = new Thread(() => FollowPlaylist(ownerId, playlistId));
        myThread.Start();
    }

    public void FollowPlaylistThread(string ownerId, string playlistId)
    {
        ErrorResponse errorResponse = _spotify.FollowPlaylist(ownerId, playlistId);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Saved track " + playlistId);
        }
    }

    public void SaveTrack(string id)
    {
        Thread myThread = new Thread(() => SaveTrack(id));
        myThread.Start();
    }

    public void SaveTrackThread(string id)
    {
        ErrorResponse errorResponse = _spotify.SaveTrack(id);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Saved track " + id);
        }
    }

    public void Follow(FollowType followType, string id)
    {
        Thread myThread = new Thread(() => FollowThread(followType, id));
        myThread.Start();
    }

    public void FollowThread(FollowType followType, string id)
    {
        ErrorResponse errorResponse = _spotify.Follow(followType, id);

        if (errorResponse.HasError())
        {
            Debug.LogError(errorResponse.Error.Message);
            Debug.LogError(errorResponse.Error.Status);
        }
        else
        {
            Debug.Log("Followed artist " + id);
        }
    }

    public void SearchSpotify(string searchQuery)
    {
        SearchItem searchItem = _spotify.SearchItems(searchQuery, SearchType.All);

        if (searchItem != null)
        {
            //throws index out of bounds for empty list returned
            //  searchItem.Albums.Items.ForEach(item => Debug.Log("Album: " + item.Name));
            //  searchItem.Tracks.Items.ForEach(item => Debug.Log("Track: " + item.Name));
            //  searchItem.Playlists.Items.ForEach(item => Debug.Log("Playlist: " + item.Name));

            StartCoroutine(searchResultsScript.LoadSearchResults(searchItem));
        }
        else
        {
            Debug.LogError("Null search result");
        }
    }

    //TODO find out what this does
    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    //Authenticates to Spotify
    async void ImplicitGrantAuth()
    {
        WebAPIFactory webApiFactory = new WebAPIFactory(
            redirectUriLocal,
            8080,
            clientId,
            Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
         Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistReadCollaborative |
         Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState | Scope.UserModifyPlaybackState,
            TimeSpan.FromSeconds(20)
        );

        try
        {
            //This will open the user's browser and returns once
            //the user is authorized
           // Debug.Log("Redirect URI: " + redirectUriLocal);
            _spotify = await webApiFactory.GetWebApi();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message.ToString());
        }

        if (_spotify == null)
            return;
    }

    public void GetContext()
    {
        PlaybackContext context = _spotify.GetPlayback();
        if (context.Item != null)
        {
            Debug.Log("Device: " + context.Device.Name);
        }
        if (context.Item != null)
        {
            Debug.Log("Context: " + context.Item.Name);
        }
        else
        {
            Debug.Log("Context null with error " + context.Error.Message + " with message: " + context.Error.Message);

        }
    }
}

