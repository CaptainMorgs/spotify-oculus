using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Mono.Csv;

public class CSVReader : MonoBehaviour
{
    public ArrayList chartTrackList = new ArrayList();

    
    // Use this for initialization
    void Start()
    {
        ReadCSV();
    }

    private void ReadCSV()
    {

        List<List<string>> dataGrid = CsvFileReader.ReadAll("C:/Users/Public/Documents/Unity Projects/Spotify Oculus/Assets/Me/Resources/CSV/regional-global-daily-latest.csv", Encoding.GetEncoding("gbk"));

        // TODO: deal with data grid
        foreach (var row in dataGrid)
        {

            ChartTrack chartTrack = new ChartTrack(row);
            chartTrackList.Add(chartTrack);
        //    Debug.Log(chartTrack.ToString());
        }

        //CsvFileWriter.WriteAll(dataGrid, "C:/Users/Public/Documents/Unity Projects/Spotify Oculus/Assets/Me/Resources/CSV/output.csv", Encoding.GetEncoding("gbk"));
    }
}

