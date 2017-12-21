using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSearch : MonoBehaviour {

    private UnityEngine.UI.Text text;
    private GameObject spotifyManager;
    private Spotify script;

    // Use this for initialization
    void Start () {
        text = transform.root.gameObject.GetComponent<UnityEngine.UI.Text>();
        spotifyManager = GameObject.Find("SpotifyManager");
        script = spotifyManager.GetComponent<Spotify>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SearchForText(string searchTerm) {
        if (searchTerm != null)
        {
            script.searchSpotify(searchTerm);
        }
        else {
            Debug.LogError("Null search term");
        }
    }
}
