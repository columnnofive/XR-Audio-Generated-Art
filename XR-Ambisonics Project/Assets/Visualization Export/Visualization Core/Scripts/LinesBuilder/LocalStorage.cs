using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class LocalStorage : MonoBehaviour
{
    private string basePath;
    private string animationsPath;

    protected virtual void Awake()
    {
        basePath = "Assets/Visualization Export/Custom Visualization/Saves/"; //change this if you want to store the lines somewhere else

        if (!Directory.Exists(basePath)) //Create Directory if it doesn't exist
        {
            Directory.CreateDirectory(basePath);
        }

        basePath += SceneManager.GetActiveScene().name;

        if (!Directory.Exists(basePath)) //Create Directory if it doesn't exist
        {
            Directory.CreateDirectory(basePath);
        }

        animationsPath = basePath + "/" + this.name;

        if (!Directory.Exists(animationsPath)) //Create Directory if it doesn't exist
        {
            Directory.CreateDirectory(animationsPath);
        }
    }

    protected string getBasepath()
    {
        return basePath;
    }
    
    protected string getAnimationsPath()
    {
        return animationsPath;
    }


    protected string getClipPath(string name)
    {
        return animationsPath + "/" + name + ".anim";
    }

    protected void ClearAnimationsDirectory()
    {
        if (Directory.Exists(getAnimationsPath())) Directory.Delete(getAnimationsPath(), true); //delete directory
        Directory.CreateDirectory(getAnimationsPath());                                         //recreate directory
        AssetDatabase.Refresh();
    }




}
