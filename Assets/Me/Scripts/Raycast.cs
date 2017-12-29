using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using VRKeyboard.Utils;
using TMPro;

public class Raycast : MonoBehaviour {

    // Use this for initialization

    private LineRenderer lineRenderer;
    public int raycastDistance = 20;
    private GameObject ovrPlayerController;
    private bool movementStarted = false;
    private RaycastHit newPosition;
    Material material;
    public GameObject vinyl, rightHandAnchor;
    public bool playOnClick = true;
    private GameObject spawnedVinyl;
    public GameObject hoverUIGameObject;
    private HoverUI hoverUI;
    public GameObject keyboardGameObject;
    private KeyboardManager keyboardManagerScript;


    void Start () {

        material = new Material(Shader.Find("Particles/Additive"));

        hoverUI = hoverUIGameObject.GetComponent<HoverUI>();

        keyboardManagerScript = keyboardGameObject.GetComponent<KeyboardManager>();

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

    /// <summary>
    /// Logic for when a raycast line collides with a collider and and the input button is pressed
    /// </summary>
    void RayCastInput()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance))
        {
           Debug.Log(hit.transform.name);

            ResumePlayback resumePlayback = hit.transform.GetComponent<ResumePlayback>();
            PausePlayback pausePlayback = hit.transform.GetComponent<PausePlayback>();
            PlaylistScript playlistScript = hit.transform.GetComponent<PlaylistScript>();

            //TODO make better with unity event system.
            if (hit.transform.gameObject.tag == "key") {
                Text text = hit.transform.gameObject.GetComponentInChildren<Text>();

             //   TextMeshProUGUI textPro = hit.transform.gameObject.GetComponentInChildren<TextMeshProUGUI>();

                keyboardManagerScript.GenerateInput(text.text);
            //    keyboardManagerScript.GenerateInput(textPro.text);
            }

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
                    playlistScript.playSomethingAsync();
                }

                if (GameObject.FindWithTag("vinyl") != null) {
                    Destroy(GameObject.FindWithTag("vinyl"));
                }

                spawnedVinyl = Instantiate(vinyl, rightHandAnchor.transform.position, Quaternion.identity);
                spawnedVinyl.GetComponent<VinylScript>().playlistScript = playlistScript;
                Debug.Log("Spawning Vinyl");
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


            if (hit.transform.gameObject.tag == "song" || hit.transform.gameObject.tag == "playlist" || hit.transform.gameObject.tag == "artist")
            {
                GameObject playlistGameObject = hit.transform.gameObject;
                hoverUI.updateHoverUI(playlistGameObject.GetComponent<PlaylistScript>());
         //       Debug.Log("Pointing at a song");
            }
           // else {
            //    Debug.Log("Pointing at a gameobject with tag " + hit.transform.gameObject.tag);

            //}

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
