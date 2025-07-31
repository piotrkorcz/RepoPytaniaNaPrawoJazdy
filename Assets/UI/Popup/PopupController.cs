using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI messageHolder;

    [SerializeField]
    public Button confirmButton;

    [SerializeField]
    public Button rejectButton;

    public void Setup(string message = null, Action onConfirmAction = null, Action onRefuseAction = null)
    {
        if(message!=null)
            messageHolder.text = message;

        confirmButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            if(onConfirmAction != null)
                onConfirmAction();
            Quit();
        });

        rejectButton.onClick.AddListener(() =>
        {
            if (onRefuseAction != null)
                onRefuseAction();
            Quit();
        });
    }

    private void Quit()
    {
        Destroy(this.gameObject);
    }

    
}
