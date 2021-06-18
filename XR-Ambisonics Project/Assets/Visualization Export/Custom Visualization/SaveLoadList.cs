using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadList : MonoBehaviour
{
    public void SaveList(List<float> timeLine, string lineName)
    {
        string destination = Application.persistentDataPath + "/" + lineName + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, timeLine);
        file.Close();
    }

    public List<float> LoadList(string lineName)
    {
        string destination = Application.persistentDataPath + "/" + lineName + ".dat";
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
