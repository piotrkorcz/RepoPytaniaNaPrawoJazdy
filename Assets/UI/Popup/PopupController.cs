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

    public void Setup(string message = null, Action onConfirmAction = null)
    {
        if(message!=null)
            messageHolder.text = message;

        confirmButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            if(onConfirmAction != null)
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
