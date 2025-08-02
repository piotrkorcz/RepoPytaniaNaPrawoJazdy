using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField] private GameObject soundToggle;
    [SerializeField] private Slider soundSlider;

    private void Start()
    {
        soundToggle.SetActive(SettingsManager.SoundToggle);
        soundSlider.value = SettingsManager.SoundSlider;
    }

    public void OnSoundToggle()
    {
        soundToggle.SetActive(!SettingsManager.SoundToggle);
        SettingsManager.OnSoundToggle();
    }

    public void OnSoundSlider()
    {
        SettingsManager.OnSoundSlider(soundSlider.value);
    }

    public void OnColorModeToggle()
    {
        SettingsManager.OnColorModeToggle();
    }

    public void HandleDownloadButton()
    {
        DataLoader.Instance.LoadNewDataSetInTheBackground();
    }

}
