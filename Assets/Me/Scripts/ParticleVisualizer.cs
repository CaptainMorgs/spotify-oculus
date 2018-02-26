using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpotifyAPI.Web.Models;

public class ParticleVisualizer : MonoBehaviour
{

    private AudioAnalysis audioAnalysis;
    private GameObject spotifyManager;
    private ParticleSystem particleSystem;
    private Spotify spotifyManagerScript;
    public double trackLength, tempo, beatsPerSecond;
    private bool isVisualizing = false;
    public int timeSignature, key;
    private string keyString;
    private List<string> keys = new List<string> { "C", "CSharp", "D", "DSharp", "E", "F", "FSharp", "G", "Gsharp", "A", "ASharp", "B" };
    public bool repeat = true;
    public float pitchSmoothing = 0.1f, emissionSmoothing, velocitySmoothing;
    private float x, y, z;
    public float bpsSmoothing = 1f;
    public float bpsPulseAmount = 1f;

    // Use this for initialization
    void Start()
    {

        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyManagerScript = spotifyManager.GetComponent<Spotify>();
        particleSystem = GetComponent<ParticleSystem>();

        //getting initial reference scales
        x = gameObject.transform.localScale.x;
        y = gameObject.transform.localScale.y;
        z = gameObject.transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {

    }

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
                //    StartCoroutine(VisualizeColour());
            }
            else
            {
                //    StopCoroutine(VisualizePitch());
                //     StartCoroutine(VisualizePitch());
            }
        }
        else
        {
            Debug.LogError("AudioAnalysis null");
        }
    }

    IEnumerator VisualizePitch()
    {
        isVisualizing = true;
        ArrayList avgPitchList = new ArrayList();
        Debug.Log("No. of segments: " + audioAnalysis.Segments.Count);
        float elapsedTime = 0f;

        AveragePitches(avgPitchList);

        float startTime = Time.time;

        float totalTime = 0f;

        var emission = particleSystem.emission;

        var particleVelocity = particleSystem.velocityOverLifetime;
        

        while (repeat)
        {

            for (int k = 0; k < audioAnalysis.Segments.Count - 1; k++)
            {
                Vector3 startVector = new Vector3((float)avgPitchList[k] / pitchSmoothing, (float)avgPitchList[k] / pitchSmoothing, (float)avgPitchList[k] / pitchSmoothing);
                Vector3 endVector = new Vector3((float)avgPitchList[k] / pitchSmoothing, (float)avgPitchList[k + 1] / pitchSmoothing, (float)avgPitchList[k] / pitchSmoothing);

                Vector3 startVectorVelocity = new Vector3((float)avgPitchList[k] / velocitySmoothing, (float)avgPitchList[k] / velocitySmoothing, (float)avgPitchList[k] / velocitySmoothing);
                Vector3 endVectorVelocity = new Vector3((float)avgPitchList[k] / velocitySmoothing, (float)avgPitchList[k + 1] / velocitySmoothing, (float)avgPitchList[k] / velocitySmoothing);

                float t = 0f;
                float duration = (float)audioAnalysis.Segments[k].Duration;
                while (t < 1 && repeat == true)
                {
                    t += Time.deltaTime / duration;
                    totalTime += t;

                    //  Debug.Log("elapsed time: " + totalTime);

                    gameObject.transform.localScale = Vector3.Lerp(startVector, endVector, t);
                    emission.rateOverTime = Mathf.Lerp(startVector.x * emissionSmoothing, endVector.x * emissionSmoothing, t);

                    int positiveOrNegative = Random.Range(0, 2) * 2 - 1;

                    particleVelocity.x = positiveOrNegative * Mathf.Lerp(startVectorVelocity.x, endVectorVelocity.x, t);
                    particleVelocity.y = positiveOrNegative * Mathf.Lerp(startVectorVelocity.x, endVectorVelocity.x, t);
                    particleVelocity.z = positiveOrNegative * Mathf.Lerp(startVectorVelocity.x, endVectorVelocity.x, t);

                    yield return null;
                }
                //     Debug.Log("SEGMENT NO " + k + "of " + audioAnalysis.Segments.Count);
            }
        }
        isVisualizing = false;
    }

    IEnumerator VisualizeBPS()
    {
        float f = (float)((1 / beatsPerSecond) * bpsSmoothing);

        while (repeat)
        {
            //TODO take these calculations out of while loop, will be constant once good values decided
            

            Vector3 startVector = gameObject.transform.localScale;
            Vector3 endVector = new Vector3(bpsPulseAmount, bpsPulseAmount, bpsPulseAmount) + startVector;

            float duration = f;

            float t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / duration;

                gameObject.transform.localScale = Vector3.Lerp(startVector, endVector, t);

                yield return null;
            }

            t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / duration;

                gameObject.transform.localScale = Vector3.Lerp(endVector, startVector, t);

                yield return null;
            }
        }
    }

    private void AnalyzeTrack()
    {
        trackLength = audioAnalysis.Track.Duration;
        tempo = audioAnalysis.Track.Tempo;
        beatsPerSecond = tempo / (double)60;
        key = audioAnalysis.Track.Key;
        keyString = keys[key];

    }

    /// <summary>
    /// /// Loops through the audio analysis segments of a song and averages there pitches.
    /// </summary>
    /// <param name="avgPitchList"></param>
    private void AveragePitches(ArrayList avgPitchList)
    {
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
}
