using System.Collections.Generic;
using UnityEngine;

public class DatabaseSorting : MonoBehaviour
{
    private static DatabaseSorting instance;
    public static DatabaseSorting Instance { get { return instance; } }

    public static SortingType sortingType = SortingType.All;
    private DatabaseSortingOption sortingOption;

    [SerializeField] private GameObject sortingUIPanel;
    [SerializeField] private List<DatabaseSortingOption> sortingOptions;
    [SerializeField] private Color optionSelectedColorInner;
    [SerializeField] private Color optionSelectedColorOuter;
    [SerializeField] private Color optionUnselectedColorInner;
    [SerializeField] private Color optionUnselectedColorOuter;
    [SerializeField] private Color optionUnselectedColorInnerLight;
    [SerializeField] private Color optionUnselectedColorOuterLight;

    private void Start()
    {
        instance = this;
    }

    public void StartSortingOptionSelection()
    {
        sortingOption = sortingOptions.Find(sO => sO.sortingType == sortingType);
        sortingUIPanel.SetActive(true);
        Render();
    }

    private void Render()
    {
        if (sortingOption == null)
            return;

        sortingOptions.ForEach(sO => {
            sO.selectionIconInner.color = SettingsManager.ColorModeToggle ? optionUnselectedColorInnerLight : optionUnselectedColorInner;
            sO.selectionIconOuter.color = SettingsManager.ColorModeToggle ? optionUnselectedColorOuterLight : optionUnselectedColorOuter;
        });

        sortingOption.selectionIconInner.color = optionSelectedColorInner;
        sortingOption.selectionIconOuter.color = optionSelectedColorOuter;
    }

    public void Select(DatabaseSortingOption sortingOption)
    {
        this.sortingOption = sortingOption;
        Render();
    }

    public void SaveSortingType()
    {
        if (sortingType != sortingOption.sortingType)
        {
            sortingType = sortingOption.sortingType;
            DatabaseUIController.Instance.Sort(true);
        }
        else
            DatabaseUIController.Instance.Sort(false);
        sortingUIPanel.SetActive(false);
    }

}
