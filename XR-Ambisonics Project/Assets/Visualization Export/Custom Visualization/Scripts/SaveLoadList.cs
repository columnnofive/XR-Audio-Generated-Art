using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadList
{
    public static void SaveList(List<float> timeLine, string lineName)
    {
        //Move to folder destination
        string destination = "Assets/Visualization Export/Custom Visualization/Lines/IO/";
        if (!Directory.Exists(destination)) //Create Directory if it doesn't exist
            Directory.CreateDirectory(destination);

        //Move to file path
        destination = "Assets/Visualization Export/Custom Visualization/Lines/IO/" + lineName + ".dat";
        

        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination); //Create File if it doesn't exist

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, timeLine);
        file.Close();
    }

    public static List<float> LoadList(string lineName)
    {
        //Move to folder destination
        string destination = "Assets/Visualization Export/Custom Visualization/Lines/IO/";
        if (!Directory.Exists(destination)) //Create Directory if it doesn't exist
            Directory.CreateDirectory(destination);

        //Move to file path
        destination = "Assets/Visualization Export/Custom Visualization/Lines/IO/" + lineName + ".dat";

        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            return null;
        }

        if(file.Length != 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<float> timeLine = (List<float>)bf.Deserialize(file);
            file.Close();
            return timeLine;
        }
        else
        {
            file.Close();
            List<float> emptyList = new List<float>();
            return emptyList;
        }
        
    }
}
