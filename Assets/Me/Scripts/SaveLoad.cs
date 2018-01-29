using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using CI.QuickSave;
using UnityEngine;

public static class SaveLoad  {

    private static readonly int CHUNK_SIZE = 1024;
    public static List<PlaylistScriptData> savedTopTracks = new List<PlaylistScriptData>();
    public static List<PlaylistScriptData> savedFeaturedPlaylists = new List<PlaylistScriptData>();
    public static List<PlaylistScriptData> savedTopArtists = new List<PlaylistScriptData>();
 //   public static List<PlaylistScript> savedTopTracks = new List<PlaylistScript>();
 //   public static List<PlaylistScript> savedTopTracks = new List<PlaylistScript>();
 //   public static List<PlaylistScript> savedTopTracks = new List<PlaylistScript>();


    //it's static so we can call it from anywhere
    public static void Save()
    {
    //    SaveLoad.savedTopTracks.Add(PlaylistScript.current);
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        Debug.Log("Saving at " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
        bf.Serialize(file, SaveLoad.savedTopTracks);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            Debug.Log("Loading from " + Application.persistentDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            SaveLoad.savedTopTracks = (List<PlaylistScriptData>)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void SaveWWWToFile(WWW www, string fileName)
    {
        byte[] bytes = www.bytes;
        int length = www.bytes.Length;
        Debug.Log("Saving www " + fileName + " at " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/" + fileName);
      //  BinaryFormatter bf = new BinaryFormatter();
      //  bf.Serialize(file);
        BinaryWriter br = new BinaryWriter(file);
        br.Write(bytes);
        file.Close();
    }

    public static WWW LoadWWWFromFile(string fileName)
    {
        Debug.Log("Loading www " + fileName + " from " + Application.persistentDataPath);
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
        MemoryStream ms = new MemoryStream();
        file.CopyTo(ms);
        byte[] bytes = ms.ToArray();
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        WWW www = (WWW)bf.Deserialize(ms);
        file.Close();
        return www;
    }

    public static void QuickSaveSpriteToFile(Sprite sprite, string fileName)
    {
        Debug.Log("Saving " + fileName);
        QuickSaveRoot.Save<Sprite>(fileName, sprite);
    }

    public static Sprite QuickLoadSpriteFromFile(string fileName)
    {
        Debug.Log("Loading " + fileName);
       return QuickSaveRoot.Load<Sprite>(fileName);
    }

    /// <summary>
    /// Loads everything by making calls to spotify
    /// </summary>
    public static void Reload()
    {
        
        Debug.Log("Reload called");
    }
}
