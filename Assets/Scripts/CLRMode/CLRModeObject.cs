using EditorAttributes;
using UnityEngine;

public class CLRModeObject : MonoBehaviour
{
    [SerializeField, ShowField(nameof(IsImage))] protected bool useSprite;
    [SerializeField, HideField(nameof(useSprite))] protected Color32 lightColor;
    [SerializeField, HideField(nameof(useSprite))] protected Color32 darkColor;

    protected bool IsImage { get { return this is CLRModeImage; } }

    private void Awake()
    {
        CLRManager.OnCLRModeChanged += Render;
    }

    private void OnDestroy()
    {
        CLRManager.OnCLRModeChanged -= Render;
    }

    private void OnEnable()
    {
        Render(SettingsManager.ColorModeToggle);
    }

    protected virtual void Render(bool isLight) { }

}
