using EditorAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistentManager : MonoBehaviour
{
    public static ConsistentManager Instance { get; private set; }

    [SerializeField]
    private GameObject popupPrefab;

    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField]
    public GameObject loadingInformation;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Start()
    {
        SpawnPopup(onConfirmAction: DataLoader.Instance.LoadNewDataSetInTheBackground);

        DataLoader.Instance.OnLoad += OnDatasetDownloaded;
    }
    private void OnDestroy()
    {
        DataLoader.Instance.OnLoad -= OnDatasetDownloaded;
    }

    private void OnDatasetDownloaded()
    {
        Debug.Log("DATA FINISHED DOWNLOADING");
        loadingInformation.SetActive(false);

    }

    [Button]
    public void SpawnPopup(string message = null, Action onConfirmAction = null, Action onRefuseAction = null)
    {
        void extendedConfirmAction()
        {
            loadingInformation.SetActive(true);
            onConfirmAction?.Invoke();
        }

        GameObject newPopupObject = Instantiate(popupPrefab, mainCanvas.transform);

        PopupController popupController = newPopupObject.GetComponent<PopupController>();

        if (popupController != null)
        {
            popupController.Setup(message, extendedConfirmAction, onRefuseAction);
        }
        else
        {
            Debug.LogError("The popupPrefab does not have a PopupController script attached to it!");
        }
    }
}
