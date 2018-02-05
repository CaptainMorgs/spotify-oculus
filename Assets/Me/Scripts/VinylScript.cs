using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylScript : MonoBehaviour {

    public PlaylistScript playlistScript;
    public GameObject fragments, followCube;
    public AudioSource audioSource;
    public GameObject vinylUIGameobject;
    private VinylUI vinylUI;
    public float animationTime = 1.5f;

    // Use this for initialization
    void Start () {
        vinylUI = vinylUIGameobject.GetComponent<VinylUI>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision) {
        //ignore layers grabbable, player and vinyl
        if (collision.gameObject.layer != 8 && collision.gameObject.layer != 9 && collision.gameObject.layer != 10 && collision.gameObject.layer != 11)
        {
            HandleThowCollision(collision);
        }
    }

        private void HandleThowCollision(Collision collision) {
        Debug.Log("Collision with vinyl at layer" + collision.gameObject.layer + "with gameobject " + collision.gameObject);
        //set gravity of the vinyl to enabled when it hits something
        gameObject.GetComponent<Rigidbody>().useGravity = true;

        //re enable rotation when it hits something
        gameObject.GetComponent<Rigidbody>().freezeRotation = false;

        //play breaking sound when collision occurs
        Debug.LogError("Playing audio: " + gameObject.GetComponent<AudioSource>().clip.ToString());
        gameObject.GetComponent<AudioSource>().Play();
        //      audioSource.Play();


        //spawn fragments
        Instantiate(fragments, gameObject.transform.position, Quaternion.identity);
        Rigidbody[] fragmentRigidBodies = fragments.GetComponentsInChildren<Rigidbody>();
        fragmentRigidBodies[0].AddExplosionForce(5.0f, fragmentRigidBodies[0].transform.position, 5.0f, 5.0f, ForceMode.Force);
        //  for (int i = 0; i < fragmentRigidBodies.Length; i++)
        //   {
        //       fragmentRigidBodies[i].AddExplosionForce(1.0f, fragmentRigidBodies..transform.position, 2.0f, 2.0f, ForceMode.Force);
        //
        //  }
        //destroy this gameobject
        Destroy(gameObject);
    }

    public  void DisableUI()
    {
        vinylUIGameobject.SetActive(false);
    }

    public void EnableUI()
    {
        vinylUIGameobject.SetActive(true);
    }

    public void AnimateToPlayer(Vector3 vector3)
    {
        DisableUI();
        Hashtable hashtable = new Hashtable();
        hashtable.Add("x", vector3.x);
        hashtable.Add("y", vector3.y);
        hashtable.Add("z", vector3.z);
        hashtable.Add("time", animationTime);
        hashtable.Add("oncomplete", "AnimateOnComplete");

        //  iTween.MoveTo(gameObject, vector3, 2);
        iTween.MoveTo(gameObject, hashtable);

        iTween.RotateTo(gameObject, new Vector3(0,0,0), animationTime);
    }

    public void AnimateOnComplete()
    {
        EnableUI();
        vinylUI.FadeInImage();
        vinylUI.FadeInPanel();
        SpawnFollowCube();
    }

    public void InitializeUI(PlaylistScript playlistScript1)
    {
        vinylUI = vinylUIGameobject.GetComponent<VinylUI>();
        vinylUI.InitializeUI(playlistScript1);
    }

    private void SpawnFollowCube()
    {
        Vector3 v = gameObject.transform.position;
        GameObject spawnedFollowCube = Instantiate(followCube, v + new Vector3(-0.5f, 0, 0), Quaternion.identity);
        spawnedFollowCube.GetComponent<FollowCubeScript>().playlistScript = playlistScript;
        spawnedFollowCube.GetComponent<FollowCubeScript>().artistId = playlistScript.artistId;
    }
}
