using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesPanelController : MonoBehaviour
{
    [SerializeField]
    private Toggle dontShowAgainToggle;

    [SerializeField]
    private GameObject playPanel;


    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("dontShowRulesPage") == 1)
        {
            this.gameObject.SetActive(false);
        }
        if (ConsistentManager.Instance.ShouldSpawnFetchPopup())
        {
            ConsistentManager.Instance.SpawnPopup(onConfirmAction: HandleAcceptPopup, onRefuseAction: DataLoader.Instance.LoadExam);
        }
        else
        {
            DataLoader.Instance.LoadExam();
        }


    }


    private void HandleAcceptPopup()
    {
        DataLoader.Instance.LoadNewDataSetInTheBackground();
        this.gameObject.SetActive(false);
        playPanel.SetActive(true);
    }
    public void HandleToggleSwitched()
    {
        PlayerPrefs.SetInt("dontShowRulesPage", System.Convert.ToInt32(dontShowAgainToggle.isOn));
    }
}
