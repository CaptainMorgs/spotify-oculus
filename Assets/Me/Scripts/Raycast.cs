﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Raycast : MonoBehaviour {

    // Use this for initialization

    private LineRenderer lineRenderer;
    public int raycastDistance = 20;
    private GameObject ovrPlayerController;
    private bool movementStarted = false;
    private RaycastHit newPosition;
    Material material;
    public GameObject vinyl, rightHandAnchor;
    public bool playOnClick = false;
    private GameObject spawnedVinyl;

    void Start () {

        material = new Material(Shader.Find("Particles/Additive"));

         lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetColors(Color.white, Color.white);
        lineRenderer.material = material;

        ovrPlayerController = GameObject.Find("OVRPlayerController");
    }

   /// <summary>
   /// If trigger is pressed, change line renderer's colour to blue if the raycast hits the floor, 
   /// if the raycast is pointing to the floor on trigger release, teleport player to location.
   /// </summary>
    void Update () {
        RayCast();

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            RayCastInput();
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
             newPosition = RayCastMovement();
        }

        else  {
            lineRenderer.SetColors(Color.white, Color.white);
            lineRenderer.material.color = Color.white;

            if (movementStarted && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                Debug.Log("Moving to " + newPosition.ToString());

                ovrPlayerController.transform.position = new Vector3(newPosition.point.x, newPosition.point.y + 0.5f, newPosition.point.z);

                lineRenderer.material.color = Color.white;
                lineRenderer.SetColors(Color.white, Color.white);

                movementStarted = false;
            }
        }

       
        
       

    }

    void FixedUpdate()
    {
       Vector3 fwd = transform.TransformDirection(Vector3.forward);

     // Physics.Raycast(transform.position, fwd, raycastDistance);
        //   print("There is something in front of the object!");

        
        lineRenderer.SetPosition(0, transform.position);
       
            lineRenderer.SetPosition(1, transform.forward * raycastDistance + transform.position);
    }

    void RayCastInput()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance))
        {
           Debug.Log(hit.transform.name);
            ResumePlayback resumePlayback = hit.transform.GetComponent<ResumePlayback>();
            PausePlayback pausePlayback = hit.transform.GetComponent<PausePlayback>();
            PlaylistScript playlistScript = hit.transform.GetComponent<PlaylistScript>();

            if (resumePlayback != null)
            {
                resumePlayback.ResumePlaybackFunction();
            }           
            else if (pausePlayback != null)
            {             
                pausePlayback.PausePlaybackFunction();
            }
            else if (playlistScript != null)
            {
                if (playOnClick)
                {
                    playlistScript.playSomething();
                }

                if (GameObject.FindWithTag("vinyl") != null) {
                    Destroy(GameObject.FindWithTag("vinyl"));
                }

                spawnedVinyl = Instantiate(vinyl, rightHandAnchor.transform.position, Quaternion.identity);
                spawnedVinyl.GetComponent<VinylScript>().playlistScript = playlistScript;
                Debug.LogError("Spawning Vinyl");
            }

        }
    }

    RaycastHit RayCastMovement()
    {
        //  Debug.LogError("Moving!");
        
          RaycastHit hit;

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance) && hit.transform.tag == "Floor")
        {
       
            lineRenderer.SetColors(Color.blue, Color.blue);
            lineRenderer.material.color = Color.blue;

            movementStarted = true;
           
        }
        else {
            lineRenderer.material.color = Color.white;
            lineRenderer.SetColors(Color.white, Color.white);
            movementStarted = false;
        }


        return hit;
        
    }

    void RayCast()
    {

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
    }

    /// <summary>
    /// prevents memory leak
    /// </summary>
    private void OnDestroy()
    {
        Destroy(GetComponent<Renderer>());
    }
}
