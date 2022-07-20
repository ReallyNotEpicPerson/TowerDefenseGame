using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string levelToLoad = "LevelSelection";
    public SceneFader sceneFader;
    public AudioSource audioSource;

    public GameObject optionGO;
    [SerializeField] Slider VolumeSlider;
    public Toggle theToggle;

    public void Awake()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        else
        {
            LoadSetting();
        }
        theToggle.isOn = Screen.fullScreen;
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
    }

    public void LoadSetting()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
        SaveSetting();
    }

    public void PlaySound()
    {
        GameAsset.I.PlayPopingSound();
    }
    public void Play()
    {
        audioSource.Play();
        sceneFader.FadeTo(levelToLoad);
    }

    public void ShowOptionUI()
    {
        optionGO.SetActive(true);
    }
    public void HideOptionUI()
    {
        optionGO.SetActive(false);
    }

    public void Quit()
    {
        audioSource.Play();
        Debug.Log("....exiting....");
        Application.Quit();
    }

    List<int> widths = new List<int>() { 568, 960, 1280, 1920 };
    List<int> heights = new List<int>() { 320, 540, 800, 1080 };

    public void SetScreenSize(int index)
    {
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen);
    }

    public void SetFullScreen(bool _fullScreen)
    {
        Screen.fullScreen = _fullScreen;
    }
}
