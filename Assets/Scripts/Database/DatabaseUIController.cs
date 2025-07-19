using UnityEngine;

public class DatabaseUIController : MonoBehaviour
{
    private static DatabaseUIController instance;
    public static DatabaseUIController Instance { get { return instance; } }

    private void Start()
    {
        instance = this;
    }

    public void Open()
    {
        if (DataLoader.Instance.databaseQuestions.Count == 0)
        {
            DataLoader.Instance.OnLoad += OpenWindowFirstTime;
            DataLoader.Instance.LoadNewDataSet();
        }
        else
            OpenWindow();
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    public void OpenWindowFirstTime()
    {
        DataLoader.Instance.OnLoad -= OpenWindowFirstTime;
        OpenWindow();
    }

    public void Sort(bool sortingTypeChanged)
    {
        DBRecyclableScrollRectDataSource.Instance.FilterData(sortingTypeChanged);
    }

}
