using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LineIO : MonoBehaviour
{
    private string directoryPath;
    private string basePath = "Assets/Visualization Export/Custom Visualization/"; //change this if you want to store the lines somewhere else

    protected virtual void Awake()
    {
        directoryPath = basePath + this.name;

        if (!Directory.Exists(directoryPath)) //Create Directory if it doesn't exist
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    protected string getDirectoryPath()
    {
        return directoryPath;
    }

    protected void setDirectoryPath(string newPath)
    {
        directoryPath = newPath;
    }

    protected string getClipPath(int index)
    {
        return directoryPath + "/" + index.ToString() + ".anim";
    }

    protected void ClearDirectory()
    {
        if (Directory.Exists(getDirectoryPath())) Directory.Delete(getDirectoryPath(), true);//delete directory
        Directory.CreateDirectory(getDirectoryPath());                                       //recreate directory
        AssetDatabase.Refresh();
    }

    


}
