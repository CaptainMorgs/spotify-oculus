using System;
using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web.Models;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour {

    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;
    private AudioAnalysis audioAnalysis;
    public double trackLength, tempo, beatsPerSecond;
    public int timeSignature, key;
    private String keyString;

    private List<String> keys = new List<String> {"C", "CSharp", "D", "DSharp", "E", "F", "FSharp", "G", "Gsharp", "A", "ASharp", "B"};

    public Transform startMarker;
    public Transform endMarker;
    //TODO rename cubeLoudness, currently nothing to do with loudness, pitch and bpm
    public GameObject cubeTempo, cubeLoudness;
    List<GameObject> cubeChildren = new List<GameObject>();
    // public float speed = 1.0F;
    private float startTime;
    private float journeyLength;
    public float overTime = 5.0f;
    public float tempoSmoothing = 100f;
    public float loudnessSmoothing = 10f;
    public float pitchSmoothing = 0.2f;
    public float speed = 1f;
    public bool repeat = true;
    private bool isVisualizing = false;
    public float bpsSmoothing = 1f;
    public float bpsPulseAmount = 1f;
    public float changeColourDuration = 2f;
    private float x, y, z;

    // Use this for initialization
    void Start()
    {
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();

        for (int i = 0; i < cubeLoudness.transform.childCount; i++) {
            cubeChildren.Add(cubeLoudness.transform.GetChild(i).gameObject);
        }

        //getting initial reference scales
        x = cubeChildren[0].transform.localScale.x;
        y = cubeChildren[0].transform.localScale.y;
        z = cubeChildren[0].transform.localScale.z;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //TODO  Visualizer is not resuming from where it left off
    public void SendAnalysis(AudioAnalysis audioAnalysis)
    {
        if (audioAnalysis != null)
        {
            Debug.Log("Analysing Track");
            this.audioAnalysis = audioAnalysis;
            AnalyzeTrack();
            if (!isVisualizing)
            {
               
                StartCoroutine(VisualizePitch());
                StartCoroutine(VisualizeBPS());
                StartCoroutine(VisualizeColour());
            }
            else {
            //    StopCoroutine(VisualizePitch());
           //     StartCoroutine(VisualizePitch());
            }
        }
        else {
            Debug.LogError("AudioAnalysis null");
        }
    }

    public void ResumeVisualization() {
        if (isVisualizing) {

        }
    }

    /// <summary>
    /// tempo is beats per minute
    /// </summary>
    private void AnalyzeTrack()
    {
        trackLength = audioAnalysis.Track.Duration;
        tempo = audioAnalysis.Track.Tempo;
        beatsPerSecond = tempo / (double) 60;
        key = audioAnalysis.Track.Key;
        keyString = keys[key];
        
    }

   

    /// <summary> 
    /// Loops through the segments and lerps between the average pitches
    /// </summary>
    /// <returns></returns>
    IEnumerator VisualizePitch()
    {
        isVisualizing = true;
        ArrayList avgPitchList = new ArrayList();
        Debug.Log("No. of segments: " + audioAnalysis.Segments.Count);
        float elapsedTime = 0f;
       
        AveragePitches(avgPitchList);

        float startTime = Time.time;

        float totalTime = 0f;

        while (repeat)
        {

        for (int k = 0; k < audioAnalysis.Segments.Count - 1; k++)
            {
                Vector3 startVector = new Vector3(x, (float)avgPitchList[k] / pitchSmoothing, z);
                Vector3 endVector = new Vector3(x, (float)avgPitchList[k + 1] / pitchSmoothing, z);

            //   float journeyLength = Vector3.Distance(startVector, endVector);
            //   float distCovered = (Time.time - startTime) * speed;
            //   float fracJourney = distCovered / journeyLength;
           
                float t = 0f;
                float duration = (float)audioAnalysis.Segments[k].Duration;
                while (t < 1 && repeat == true)
                {
                    t += Time.deltaTime / duration;
                 totalTime += t;

              //  Debug.Log("elapsed time: " + totalTime);

                    cubeLoudness.transform.localScale = Vector3.Lerp(startVector, endVector, t);

                    yield return null;
                }
           //     Debug.Log("SEGMENT NO " + k + "of " + audioAnalysis.Segments.Count);
            }
       }
        isVisualizing = false;
    }

    IEnumerator VisualizeBPS() {

        

        Debug.LogWarning("In VisualizeBPS");

        float f = (float)((1 / beatsPerSecond) * bpsSmoothing);

        while (repeat) {
            //TODO take these calculations out of while loop, will be constant once good values decided

            Vector3 startVector = cubeLoudness.transform.localScale;
            Vector3 endVector = new Vector3(bpsPulseAmount, bpsPulseAmount, bpsPulseAmount) + startVector;

            float duration = f;

            float t = 0f;
           
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                for (int i=0; i < cubeChildren.Count; i++) {
                    cubeChildren[i].transform.localScale = Vector3.Lerp(startVector, endVector, t);
                }
               
           //     Debug.LogWarning("Lerping one");

                yield return null;
            }

            t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                for (int i = 0; i < cubeChildren.Count; i++)
                {
                    cubeChildren[i].transform.localScale = Vector3.Lerp(endVector, startVector, t);
                }
         //       Debug.LogWarning("Lerping two");

                yield return null;
            }
        }
    }

    IEnumerator VisualizeColour()
    {
        while (repeat)
        {
            float duration = changeColourDuration;

            float t = 0f;

            Color newColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);

            while (t < 1)
            {
                t += Time.deltaTime / duration;
               

                for (int i = 0; i < cubeChildren.Count; i++)
                {
                    //      cubeChildren[i].GetComponent<Renderer>().material.color = newColor;

                   Color lerpedColor = Color.Lerp(cubeChildren[i].GetComponent<Renderer>().material.color, newColor, t);

                    cubeChildren[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", lerpedColor);
                    //       Debug.Log("Changing color");
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// /// Loops through the audio analysis segments of a song and averages there pitches.
    /// </summary>
    /// <param name="avgPitchList"></param>
    private void AveragePitches(ArrayList avgPitchList) {
        float pitchSum = 0f;
        double segmentDurationSum = 0;

        for (int i = 0; i < audioAnalysis.Segments.Count; i++)
        {
            for (int j = 0; j < audioAnalysis.Segments[i].Pitches.Count - 1; j++)
            {
                pitchSum += (float)audioAnalysis.Segments[i].Pitches[j];
            }
            float avgPitch = pitchSum / audioAnalysis.Segments[i].Pitches.Count;

            segmentDurationSum += audioAnalysis.Segments[i].Duration;

         //   Debug.Log("Average pitch for segment " + i + " " + avgPitch);
            avgPitchList.Add(avgPitch);
            pitchSum = 0;
        }
        Debug.Log("Total duration of segments " + segmentDurationSum + " vs track duration " + trackLength);
    }

    IEnumerator Visualize(float speed) {
        isVisualizing = true;
        Debug.Log("Visualizer Tempo: " + speed);
        while (repeat) {

            startTime = Time.time;
            journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

            while (cubeTempo.transform.position != endMarker.position //Time.time < startTime + overTime
                ) {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                cubeTempo.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
                yield return null;
            }
            // cube.transform.position = endMarker.position;

            startTime = Time.time;
            journeyLength = Vector3.Distance(endMarker.position, startMarker.position);

            while (cubeTempo.transform.position != startMarker.position //Time.time < startTime + overTime
               )
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                cubeTempo.transform.position = Vector3.Lerp(endMarker.position, startMarker.position, fracJourney);
                yield return null;
            }

        }
        //   cube.transform.position = endMarker.position;
        isVisualizing = false;
    }

    IEnumerator Visualize2()
    {
        for (int i = 0; i < audioAnalysis.Segments.Capacity; i++)
        {

            float startTime = Time.time;
            Debug.Log(audioAnalysis.Segments[i].Duration);
            Vector3 start = new Vector3(1, (float)audioAnalysis.Segments[i].LoudnessStart / loudnessSmoothing, 1);
            Vector3 max = new Vector3(1, (float)audioAnalysis.Segments[i].LoudnessMax / loudnessSmoothing, 1);
            Vector3 end = new Vector3(1, (float)audioAnalysis.Segments[i].LoudnessEnd / loudnessSmoothing, 1);


            while (Time.time - startTime < (float)audioAnalysis.Segments[i].Duration)
            {
                cubeLoudness.transform.localScale = Vector3.Lerp(start, max, (Time.time - startTime) / (float)audioAnalysis.Segments[i].Duration / 2);
                cubeLoudness.transform.localScale = Vector3.Lerp(end, max, (Time.time - startTime) / (float)audioAnalysis.Segments[i].Duration / 2);
                yield return null;

            }
        }
    }

    IEnumerator Visualize3()
    {
        ArrayList vectors = new ArrayList();
        Debug.Log("No. of segments: " + audioAnalysis.Segments.Count);
        float elapsedTime = 0f;

        //   for (int i = 0; i < audioAnalysis.Segments.Count; i++)
        //   {
        //    Debug.Log("No. of pitches in segment " + i + ": " + audioAnalysis.Segments[i].Pitches.Capacity);

        //    for (int k = 0; k < audioAnalysis.Segments[i].Pitches.Count; k++)
        //    {
        //       vectors.Add(new Vector3(1, (float)audioAnalysis.Segments[i].Pitches[k], 1));
        //       Debug.Log("Added pitch " + k);
        //  }
        //   }

        //         Debug.Log("Finished adding vectors" );
        float startTime = Time.time;

        //   Vector3 start = new Vector3(1, (float)audioAnalysis.Segments[i].LoudnessStart / loudnessSmoothing, 1);
        //  Vector3 max = new Vector3(1, (float)audioAnalysis.Segments[i].LoudnessMax / loudnessSmoothing, 1);
        //  Vector3 end = new Vector3(1, (float)audioAnalysis.Segments[i].LoudnessEnd / loudnessSmoothing, 1);

        //    while (Time.time - startTime < (float)audioAnalysis.Segments[i].Duration)
        for (int i = 0; i < audioAnalysis.Segments.Count; i++)
        {
            for (int j = 0; j < audioAnalysis.Segments[i].Pitches.Count - 1; j++)
            {
                //         cubeLoudness.transform.localScale = Vector3.Lerp((Vector3)vectors[j], (Vector3)vectors[j + 1], (Time.time - startTime) / (float)audioAnalysis.Segments[j].Duration / (float)audioAnalysis.Segments[i].Pitches.Capacity);



                cubeLoudness.transform.localScale = Vector3.Lerp(new Vector3(1, (float)audioAnalysis.Segments[i].Pitches[j] / pitchSmoothing, 1), new Vector3(1, (float)audioAnalysis.Segments[i].Pitches[j + 1] / pitchSmoothing, 1), //(Time.time - startTime) / 
                    ((float)audioAnalysis.Segments[j].Duration / (float)audioAnalysis.Segments[i].Pitches.Count) / 10f);

                elapsedTime += ((float)audioAnalysis.Segments[j].Duration / (float)audioAnalysis.Segments[i].Pitches.Count) / 10f;
                Debug.Log(elapsedTime);


                //  cubeLoudness.transform.localScale = Vector3.Lerp((Vector3)vectors[j + 1], (Vector3)vectors[j], (Time.time - startTime) / (float)audioAnalysis.Segments[i].Duration / 2);
                yield return null;

            }
        }
    }
}

