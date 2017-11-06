using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylScript : MonoBehaviour {

    public PlaylistScript playlistScript;
    public GameObject fragments;
    public AudioSource audioSource;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision) {
        //ignore layers grabbable, player and vinyl
        if (collision.gameObject.layer != 8 && collision.gameObject.layer != 9 && collision.gameObject.layer != 10)
        {
            //set gravity of the vinyl to enabled when it hits something
            gameObject.GetComponent<Rigidbody>().useGravity = true;

            //re enable rotation when it hits something
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;

            //play breaking sound when collision occurs
            Debug.LogError("Playing audio: " + gameObject.GetComponent<AudioSource>().clip.ToString());
            gameObject.GetComponent<AudioSource>().Play();
            audioSource.Play();


            //spawn fragments
            Instantiate(fragments, gameObject.transform.position, Quaternion.identity);

            //destroy this gameobject
            Destroy(gameObject);
        }
    }
}
