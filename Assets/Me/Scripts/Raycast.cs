using UnityEngine;
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

    void Start () {
         lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetColors(Color.white, Color.white);
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material = whiteDiffuseMat;
        //    lineRenderer.SetVertexCount(2);

        ovrPlayerController = GameObject.Find("OVRPlayerController");
    }

    // Update is called once per frame
    void Update () {
        RayCast();

        if (OVRInput.Get(OVRInput.Button.One))
        {
            RayCastInput();
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
         //   Debug.Log("Trigger pressed");
             newPosition = RayCastMovement();
        }
        if (movementStarted && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.Log("Moving!");
            ovrPlayerController.transform.position = new Vector3(newPosition.transform.position.x, newPosition.transform.position.y + 0.5f, newPosition.transform.position.z);
            lineRenderer.material.color = Color.white;
            lineRenderer.SetColors(Color.white, Color.white);
            movementStarted = false;
        }

        lineRenderer.material.color = Color.white;
        lineRenderer.SetColors(Color.white, Color.white);

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
                playlistScript.playSomething();
            }

        }
    }

    RaycastHit RayCastMovement()
    {
        //  Debug.LogError("Moving!");
        
          RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance) )
        {
            Debug.Log(hit.transform.tag);
            //   Debug.Log("Y position of hit in range: " + hit.transform.position.y);
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetColors(Color.blue, Color.blue);
            //   lineRenderer.startColor = Color.blue;
            //   lineRenderer.endColor = Color.blue;
            lineRenderer.material.color = Color.blue;
            movementStarted = true;
        }
        else {
        //    Debug.Log("Y position of hit out of range: " + hit.transform.position.y);
            lineRenderer.material.color = Color.white;
            lineRenderer.SetColors(Color.white, Color.white);
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
