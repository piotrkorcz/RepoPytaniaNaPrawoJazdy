using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DatabaseUIController : MonoBehaviour
{
    private static DatabaseUIController instance;
    public static DatabaseUIController Instance { get { return instance; } }

    [SerializeField]
    private GameObject playPanel;


    private void Start()
    {
        instance = this;
    }

    private void GoBackAndWaitForLoad()
    {
        DataLoader.Instance.LoadNewDataSetInTheBackground();
        playPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void Open()
    {
        if (DataLoader.Instance.databaseQuestions.Count == 0)
        {
            DataLoader.Instance.OnLoad += OpenWindowFirstTime;
            //ConsistentManager.Instance.SpawnPopup(onConfirmAction: GoBackAndWaitForLoad, onRefuseAction: DataLoader.Instance.LoadNewDataSet);
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
