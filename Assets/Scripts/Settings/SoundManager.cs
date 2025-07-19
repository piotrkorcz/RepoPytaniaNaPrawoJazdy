using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonSoundEffect;

    private void Start()
    {
        instance = this;
        FindObjectsOfType<Button>(true).ToList().ForEach(button => button.onClick.AddListener(PlayButtonSoundEffect));
        SetVolume();
    }

    private void PlayButtonSoundEffect()
    {
        audioSource.PlayOneShot(buttonSoundEffect);
    }

    public void SetVolume()
    {
        audioSource.volume = SettingsManager.SoundToggle ? SettingsManager.SoundSlider : 0;
    }

}
