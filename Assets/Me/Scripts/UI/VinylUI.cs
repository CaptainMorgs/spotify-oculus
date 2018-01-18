﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class VinylUI : MonoBehaviour {

    public TextMeshProUGUI artistNameProText, songNameProText, descriptionProText;
    public Image uiImage;
    public GameObject panel;
    private Image panelImage;

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializeUI(PlaylistScript playlistScript) {

        if (playlistScript.gameObject.tag == "song")
        {
            artistNameProText.text += playlistScript.getFullTrack().Artists[0].Name;
            songNameProText.text += playlistScript.getFullTrack().Name;
            descriptionProText.text += ("Popularity: " + playlistScript.getFullTrack().Popularity);
        }
        else if (playlistScript.gameObject.tag == "artist")
        {
            artistNameProText.text += playlistScript.getPlaylistName();
            songNameProText.text = "";
            descriptionProText.text += ("Popularity: " + playlistScript.fullArtist.Popularity + "/n" + " Genre: " + playlistScript.fullArtist.Genres[0]);

        }
        else if (playlistScript.gameObject.tag == "playlist")
        {
            artistNameProText.text += playlistScript.getPlaylistName();
            songNameProText.text = "";

            //TODO new releases are tagged as playlist so getSimplePlaylist() will be null
            if (playlistScript.getSimplePlaylist() != null)
            {
                descriptionProText.text += ("Playlist Owner: " + playlistScript.getSimplePlaylist().Owner.DisplayName);
            }
        }
        else {
            Debug.LogError("Could not initialize Vinyl UI, tag not found");
        }

        //Making the image not transparent when its loaded (avoids white image if it doesn't load)
        if (uiImage != null) {
            uiImage.sprite = playlistScript.sprite;
            Color color = uiImage.GetComponent<Image>().color;
            if (color != null)
            {
                StartCoroutine(FadeTo(uiImage, color, 1.0f, 0.5f));
         
            }
        }
        else
        {
            Debug.LogError("uiImage is null");
        }

        panelImage = GetComponent<Image>();

        //TODO look into Graphic.CrossFadeAlpha
        if (panelImage != null)
        {
            //uiImage.sprite = playlistScript.sprite;
            Color color = panelImage.color;
            if (color != null)
            {
                StartCoroutine(FadeTo(panelImage, color, 0.4f, 0.5f));

            }
        }
        else {
            Debug.LogError("panelImage is null");
        }
    }

    //Fades a color's alpha from 0 to 1 (transparent to fully visible)
    private IEnumerator FadeTo(Image uiImage, Color color, float aValue, float aTime)
    {
        float alpha = color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            uiImage.color = newColor;
            yield return null;
        }
    }
}