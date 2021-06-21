using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class AnimationsTimeLine : LineIO
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

    [MyBox.ButtonMethod]
    private string DeleteButton() //this function is called on button press from the inspector
    {
        return Delete();
        
    }

    protected override void Awake()
    {
        base.Awake();
        timeLine = SaveLoadList.LoadList(this.name);
        RemoveScaleProperty();
    }
    
    private void Update() //enables to manually modify values from the Inspector
    {
        RefreshList();
    }

    private void RefreshList()
    {
        SaveLoadList.SaveList(timeLine, this.name);
        timeLine = SaveLoadList.LoadList(this.name);
    }

    //Remove scale property from animations
    //The RecordController records EVERYTHING. While we want to have a preview of the line when recording,
    //when playing the animation we want the scale to change accordingly with the Band/Amplitude visualizer.
    //Therefore, we get rid of the scale property alltogether.
    private void RemoveScaleProperty()
    {
        for (int i = 0; i < timeLine.Count; i++)
        {
            AnimationClip anim = (AnimationClip)AssetDatabase.LoadAssetAtPath(base.getClipPath(i), (typeof(Object)));
            anim.SetCurve("", typeof(Transform), "m_LocalScale", null);
        }
    }

    private void DeleteAssetAtIndex(int clipIndex)
    {
        //delete asset
        AssetDatabase.DeleteAsset(getClipPath(clipIndex));

        //shift other assets by one to the left
        for (int i = clipIndex; i < timeLine.Count; i++)
        {
            AssetDatabase.RenameAsset(getClipPath(i + 1), i + ".anim");
        }
    }

    private string Delete()
    {
        switch (deleteOption)
        {
            case DeleteOption.ChildSafety:
                return "Fiù... That was close...";                                          //...
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
                base.ClearDirectory();                                                          //clear folder
                timeLine.Clear();                                                               //clear list
                RefreshList();
                return "All animations have been deleted";
                break;

            default:
                return "You must be a magician";
        }
    }

}
#endif
