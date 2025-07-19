using System;
using UnityEngine;

public class CLRManager : MonoBehaviour
{
    public static event Action<bool> OnCLRModeChanged;

    private void Start()
    {
        RenderColorMode();
    }

    public static void RenderColorMode()
    {
        OnCLRModeChanged?.Invoke(SettingsManager.ColorModeToggle);
    }

}
