using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField] private GameObject soundToggle;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Button downloadButton;
    [SerializeField] private GameObject disclaimerText;
    [SerializeField] private Button deleteButton;

    private void Start()
    {
        soundToggle.SetActive(SettingsManager.SoundToggle);
        soundSlider.value = SettingsManager.SoundSlider;

        bool should = ConsistentManager.Instance.ShouldSpawnFetchPopup();
        downloadButton.interactable = should;
        disclaimerText.SetActive(!should);
        deleteButton.interactable = !should;
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
        disclaimerText.SetActive(true);
        downloadButton.interactable = false;
    }

    public void HandleDeleteButton()
    {
        SaveSystem.DeleteLocallySavedData();

        downloadButton.interactable = true;
        disclaimerText.SetActive(false);
        deleteButton.interactable = false;


    }

}
