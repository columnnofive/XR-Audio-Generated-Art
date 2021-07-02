using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public struct Clip
{
    public float time { get; set; }
    public string name { get; set; }
    public Clip(float time, string name)
    {
        this.time = time;
        this.name = name;
    }
}

public class SaveLoad : LocalStorage
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SaveList(List<Clip> timeLine)
    {
        string destination = getBasepath() + "/TimeLineSaves";  //Move to folder destination
        if (!Directory.Exists(destination))                     
            Directory.CreateDirectory(destination);             //Create Directory if it doesn't exist
        destination += "/" + this.name + ".dat";                //Move to file path

        FileStream file;
        if (File.Exists(destination))
            file = File.OpenWrite(destination);                 //Open file
        else file = File.Create(destination);                   //Create file if it doesn't exist

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, timeLine);
        file.Close();
    }


    public List<Clip> LoadList()
    {
        string destination = getBasepath() + "/TimeLineSaves";  //Move to folder destination
        if (!Directory.Exists(destination))                     
            Directory.CreateDirectory(destination);             //Create Directory if it doesn't exist
        destination += "/" + this.name + ".dat";                 //Move to file path
        
        FileStream file;
        List<Clip> list = new List<Clip>();
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            if (file.Length != 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                list = (List<Clip>)bf.Deserialize(file);
            }
            
            file.Close();
            return list;
        }   
        else
        {
            return list;
        }
        
    }

    public List<float> LoadTimesList()
    {
        List<Clip> list = LoadList();
        List<float> timesList = new List<float>();
        foreach (Clip clip in list)
        {
            timesList.Add(clip.time);
        }
        return timesList;
    }
}
