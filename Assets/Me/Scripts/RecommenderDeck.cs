using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecommenderDeck : MonoBehaviour
{

    public List<GameObject> recommendationSeeds;

    public List<GameObject> activeSeeds;

    public UserRecommendations userRecommendations;

    public TextMeshProUGUI seed1Text, seed2Text;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetRecommendations()
    {
        Debug.Log("GetRecommendations called");
        if (activeSeeds.Count > 0)
        {

            List<string> seedIds = new List<string>();

            foreach (var seed in activeSeeds)
            {
                PlaylistScript playlistScript = seed.GetComponent<VinylScript>().playlistScript;
                if (playlistScript.trackType == PlaylistScript.TrackType.artist)
                {
                    seedIds.Add(playlistScript.artistId);
                }
                else
                {
                    Debug.LogError("only supports artisTI");
                }


            }
            if (seedIds.Count != 0)
            {
               StartCoroutine(userRecommendations.LoadUserRecommendations(seedIds));
            }
            else {
                Debug.LogError("Seed Id list is empty");
            }
        }
    }
}
