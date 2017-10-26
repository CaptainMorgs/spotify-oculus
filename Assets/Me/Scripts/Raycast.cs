using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Raycast : MonoBehaviour {

    // Use this for initialization

    private LineRenderer lineRenderer;
    public int raycastDistance = 20;

    void Start () {
         lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetColors(Color.white, Color.white);
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material = whiteDiffuseMat;
        //    lineRenderer.SetVertexCount(2);
    }

    // Update is called once per frame
    void Update () {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            RayCast2();
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

    void RayCast2()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance))
        {
            Debug.Log(hit.transform.name);
            ResumePlayback resumePlayback = hit.transform.GetComponent<ResumePlayback>();
            PausePlayback pausePlayback = hit.transform.GetComponent<PausePlayback>();

            if (resumePlayback != null)
            {
                resumePlayback.ResumePlaybackFunction();
            }           
            else if (pausePlayback != null)
            {             
                pausePlayback.PausePlaybackFunction();
            }

        }
    }
        void RayCast() {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            Debug.Log("a pressed");

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                //WorldUI is my layer name
                if (results[0].gameObject.layer == LayerMask.NameToLayer("WorldUI"))
                {
                    string dbg = "Root Element: {0} \n GrandChild Element: {1}";
                    Debug.Log(string.Format(dbg, results[results.Count - 1].gameObject.name, results[0].gameObject.name));
                    //Debug.Log("Root Element: "+results[results.Count-1].gameObject.name);
                    //Debug.Log("GrandChild Element: "+results[0].gameObject.name);
                    results.Clear();
                }
            }
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
