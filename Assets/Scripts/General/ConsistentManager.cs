using EditorAttributes;
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
    [Button]
    public void SpawnPopup(string message)
    {
        GameObject newPopupObject = Instantiate(popupPrefab, mainCanvas.transform);

        PopupController popupController = newPopupObject.GetComponent<PopupController>();

        if (popupController != null)
        {
            popupController.Setup(message);
        }
        else
        {
            Debug.LogError("The popupPrefab does not have a PopupController script attached to it!");
        }
    }
}
