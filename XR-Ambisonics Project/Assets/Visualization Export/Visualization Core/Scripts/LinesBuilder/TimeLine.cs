#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class TimeLine : TimeLineBase
{
    #region "delete options"
    enum DeleteOption
    {
        AtIndex,
        Everything
    }

    [MyBox.Foldout("Delete Options", true)]
    [SerializeField]
    private DeleteOption deleteOption = DeleteOption.AtIndex;

    [MyBox.ConditionalField(nameof(deleteOption), false, DeleteOption.AtIndex)]
    [SerializeField]
    private int IndexToDelete = 0;

    [SerializeField]
    private bool deleteButton = true;
    private void Delete()
    {
        switch (deleteOption)
        {
            case DeleteOption.AtIndex:
                RemoveAt(IndexToDelete);
                Debug.Log("Animation " + IndexToDelete + " has been deleted");
                break;
            case DeleteOption.Everything:
                RemoveAll();
                Debug.Log("All animations have been deleted");
                break;
        }
    }
    #endregion

    #region "modify time"
    [MyBox.Foldout("Modify Time", true)]
    [SerializeField]
    private int index;
    [SerializeField]
    private float newTime;
    [SerializeField]
    private bool modifyButton = true;
    private void ModifyTime() //this function is called on button press from the inspector
    {
        Modify(index, newTime);
        Debug.Log("Modified Clip is now at index " + GetIndexAtTime(newTime));
    }
    #endregion

    #region "offset times"
    [MyBox.Foldout("Offset Times", true)]
    [SerializeField]
    private int rangeStartIndex;
    [SerializeField]
    private int rangeEndIndex;
    [SerializeField]
    private float offset;
    [SerializeField]
    private bool offsetButton;
    private void OffsetTimes()
    {
        if (Offset(rangeStartIndex, rangeEndIndex, offset))
            Debug.Log("Offset applied");
        else
            Debug.Log("Offset couldn't be applied, check your indexes");
    }
    #endregion

    #region "copy animation"
    [MyBox.Foldout("Copy Animation", true)]

    [SerializeField]
    private int getNameAtIndex;
    [SerializeField]
    [ReadOnlyField]
    private string nameInAssets;

    [SerializeField]
    private AnimationClip clipToCopy;
    [SerializeField]
    private float copyToTime;
    [SerializeField]
    private bool copyButton = true;
    private void CopyAnimation()
    {
        Copy(clipToCopy, copyToTime);
        Debug.Log("A copy has been created at index " + GetIndexAtTime(copyToTime));
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnValidate()   //gets called every time a inspector value changes
    {
        if (!modifyButton)
        {
            ModifyTime();
            modifyButton = true;
        }

        if (!deleteButton)
        {
            Delete();
            deleteButton = true;
        }

        if (!copyButton)
        {
            CopyAnimation();
            copyButton = true;
        }

        if (!offsetButton)
        {
            OffsetTimes();
            offsetButton = true;
        }

        nameInAssets = GetNameAtIndex(getNameAtIndex);
    }

}
#endif