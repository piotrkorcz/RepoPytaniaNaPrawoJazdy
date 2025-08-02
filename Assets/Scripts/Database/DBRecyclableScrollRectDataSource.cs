using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DBRecyclableScrollRectDataSource : MonoBehaviour, IRecyclableScrollRectDataSource
{
    private static DBRecyclableScrollRectDataSource instance;
    public static DBRecyclableScrollRectDataSource Instance { get { return instance; } }

    [SerializeField] RecyclableScrollRect recyclableScrollRect;
    [SerializeField] TextMeshProUGUI answeredQuestionsCountTextView;
    private List<QuestionData> data = new();
    private List<QuestionData> filteredData = new();

    private bool areExamQuestions;
    public bool AreExamQuestions { set { areExamQuestions = value; } get { return areExamQuestions; } }

    private void OnEnable()
    {
        instance = this;

        DatabaseSorting.sortingType = SortingType.All;

        recyclableScrollRect.DataSource = this;

        answeredQuestionsCountTextView.text = DataLoader.Instance.answeredQuestions.Count.ToString();

        FilterData(true);
    }

    public void FilterData(bool sortingTypeChanged)
    {
        data = (DatabaseSorting.sortingType == SortingType.Correct || DatabaseSorting.sortingType == SortingType.Incorrect) ? DataLoader.Instance.answeredQuestions : DataLoader.Instance.databaseQuestions;

        if (areExamQuestions)
            data = DataLoader.Instance.examQuestions;

        if (DatabaseSorting.sortingType != SortingType.All)
        {
            if (areExamQuestions && DatabaseSorting.sortingType == SortingType.Unanswered)
                filteredData = data.FindAll(data => data.sortingType == SortingType.Skipped);
            else
                filteredData = data.FindAll(data => data.sortingType == DatabaseSorting.sortingType);
        }
        else
            filteredData = new(data);

        if (sortingTypeChanged)
            recyclableScrollRect.ReloadData();
    }

    public int GetItemCount()
    {
        return filteredData.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        DBCell item = cell as DBCell;
        if (index == filteredData.Count - 1 && !(DatabaseSorting.sortingType == SortingType.Correct || DatabaseSorting.sortingType == SortingType.Incorrect) && !areExamQuestions)
        {
            //DataLoader.Instance.LoadNewDataSet();
            //StartCoroutine(OnNewDataSetLoaded());
        }
        item.ConfigureCell(filteredData[index], index);
    }

    private IEnumerator OnNewDataSetLoaded()
    {
        yield return new WaitUntil(() => !DataLoader.Instance.IsLoading);

        data = DataLoader.Instance.databaseQuestions;
        FilterData(false);
    }

}
