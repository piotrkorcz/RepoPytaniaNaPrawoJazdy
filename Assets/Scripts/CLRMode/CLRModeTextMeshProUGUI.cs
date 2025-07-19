using TMPro;
using UnityEngine;

public class CLRModeTextMeshProUGUI : CLRModeObject
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    protected override void Render(bool isLight) 
    {
        textMeshProUGUI.color = isLight ? lightColor : darkColor;
    }

}
