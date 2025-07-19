using UnityEngine;
using UnityEngine.UI;

public class DatabaseSortingOption : MonoBehaviour
{
    public SortingType sortingType;
    public Image selectionIconInner;
    public Image selectionIconOuter;

    public void Select()
    {
        DatabaseSorting.Instance.Select(this);
    }

}

public enum SortingType
{
    Correct,
    Incorrect,
    Skipped,
    Unanswered,
    All
}