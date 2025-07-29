using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    public string text;

    [SerializeField]
    public Button confirmButton;

    [SerializeField]
    public Button rejectButton;

    private void Start()
    {
        confirmButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();

        rejectButton.onClick.AddListener(Quit);

    }

    public void Setup(string message, Action onConfirmAction)
    {

        confirmButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            onConfirmAction();
            Destroy(this.gameObject);
        });

        rejectButton.onClick.AddListener(Quit);
    }

    private void Quit()
    {
        Destroy(this.gameObject);
    }

    
}
