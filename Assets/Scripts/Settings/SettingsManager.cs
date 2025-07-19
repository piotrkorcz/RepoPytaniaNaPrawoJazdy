using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private const string SOUND_TOGGLE_SAVE_KEY = "soundToggle";
    private const string SOUND_SLIDER_SAVE_KEY = "soundSlider";
    private const string COLOR_MODE_TOGGLE_SAVE_KEY = "colorModeToggle";

    private const bool DEFAULT_SOUND_TOGGLE_VALUE = true;
    private const float DEFAULT_SOUND_SLIDER_VALUE = 0.5f;
    private const bool DEFAULT_COLOR_MODE_TOGGLE_SAVE_KEY = false;

    private static bool soundToggle = DEFAULT_SOUND_TOGGLE_VALUE;
    private static float soundSlider = DEFAULT_SOUND_SLIDER_VALUE;
    private static bool colorModeToggle = DEFAULT_COLOR_MODE_TOGGLE_SAVE_KEY;

    public static bool SoundToggle { get { return soundToggle; } }
    public static float SoundSlider { get { return soundSlider; } }
    public static bool ColorModeToggle { get { return colorModeToggle; } }

    private void Awake()
    {
        soundToggle = PlayerPrefs.GetInt(SOUND_TOGGLE_SAVE_KEY, soundToggle.ToInt()).ToBool();
        soundSlider = PlayerPrefs.GetFloat(SOUND_SLIDER_SAVE_KEY, soundSlider);
        colorModeToggle = PlayerPrefs.GetInt(COLOR_MODE_TOGGLE_SAVE_KEY, colorModeToggle.ToInt()).ToBool();
    }

    public static void OnSoundToggle()
    {
        soundToggle = !soundToggle;
        PlayerPrefs.SetInt(SOUND_TOGGLE_SAVE_KEY, soundToggle.ToInt());

        SoundManager.Instance.SetVolume();
    }

    public static void OnSoundSlider(float value)
    {
        soundSlider = value;
        PlayerPrefs.SetFloat(SOUND_SLIDER_SAVE_KEY, soundSlider);

        SoundManager.Instance.SetVolume();
    }

    public static void OnColorModeToggle()
    {
        colorModeToggle = !colorModeToggle;
        PlayerPrefs.SetInt(COLOR_MODE_TOGGLE_SAVE_KEY, colorModeToggle.ToInt());

        CLRManager.RenderColorMode();
    }

}
