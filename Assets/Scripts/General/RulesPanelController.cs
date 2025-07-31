using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesPanelController : MonoBehaviour
{
    [SerializeField]
    private Toggle dontShowAgainToggle;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("dontShowRulesPage") == 1)
        {
            this.gameObject.SetActive(false);
            DataLoader.Instance.LoadExam();
        }

        
    }
    public void HandleToggleSwitched()
    {
        PlayerPrefs.SetInt("dontShowRulesPage", System.Convert.ToInt32(dontShowAgainToggle.isOn));
    }
}
