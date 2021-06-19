using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class AnimationsTimeLine : SaveLoadList
{
    public List<float> timeLine;

    enum DeleteOption
    {
        ChildSafety,
        AtIndex,
        //Range,
        Everything
    }

    [SerializeField]
    private DeleteOption deleteOption = DeleteOption.ChildSafety;

    [MyBox.ConditionalField(nameof(deleteOption), false, DeleteOption.AtIndex)]
    [SerializeField]
    private int deleteAtIndex = 0;
    /*
    [MyBox.ConditionalField(nameof(deleteOption), false, DeleteOption.Range)]
    [SerializeField]
    private int deleteAtStartIndex = 0;
    [MyBox.ConditionalField(nameof(deleteOption), false, DeleteOption.Range)]
    [SerializeField]
    private int deleteAtEndIndex = 0;
    */

    private string directoryPath;

    [MyBox.ButtonMethod]
    private string Delete()
    {
        switch (deleteOption)
        {
            case DeleteOption.ChildSafety:
                return "Fiù... That was close...";                                          //it's just a prank bro
                break;

            case DeleteOption.AtIndex:
                timeLine.RemoveAt(deleteAtIndex);                                           //remove from timeline
                RefreshList();                                                              //need to refresh to have correct timeLine.Count in next line
                DeleteAssetAtIndex(deleteAtIndex);                                          //remove from assets
                AssetDatabase.Refresh();
                return "Animation " + deleteAtIndex + " has been deleted";
                break;

            /*case DeleteOption.Range:

                return "Animations from " + deleteAtStartIndex + " to " + deleteAtEndIndex + " have been deleted";
                break;
            */
            case DeleteOption.Everything:
                if (Directory.Exists(directoryPath))  Directory.Delete(directoryPath, true);//delete directory
                Directory.CreateDirectory(directoryPath);                                   //recreate directory
                timeLine.Clear();                                                           //remove items
                RefreshList();
                AssetDatabase.Refresh();
                return "All animations have been deleted";
                break;

            default:
                return "You must be a magician or sum";
        }
    }

    private void Awake()
    {
        directoryPath = "Assets/Visualization Export/Custom Visualization/" + this.name;
        timeLine = LoadList(this.name);
        RemoveScaleProperty();
    }
    
    private void Update() //enables to manually modify values from the Inspector
    {
        RefreshList();
    }

    //Remove scale property from animations
    //this happens because the animations recorder records everything, but the size should be handled later by the LineInstance
    private void RemoveScaleProperty()
    {
        for(int i = 0; i < timeLine.Count; i++)
        {
            AnimationClip anim = (AnimationClip)AssetDatabase.LoadAssetAtPath(GetClipPath(i), (typeof(Object)));
            anim.SetCurve("", typeof(Transform), "m_LocalScale", null);
        }
    }

    private void RefreshList()
    {
        SaveList(timeLine, this.name);
        timeLine = LoadList(this.name);
    }

    private void DeleteAssetAtIndex(int clipIndex)
    {
        //delete asset
        AssetDatabase.DeleteAsset(GetClipPath(clipIndex));

        //shift other assets by one to the left
        for (int i = clipIndex; i < timeLine.Count; i++)
        {
            AssetDatabase.RenameAsset(GetClipPath(i+1), i + ".anim");
        }
    }

    private string GetClipPath(int index)
    {
        return directoryPath + "/" + index.ToString() + ".anim";
    }

}
#endif
