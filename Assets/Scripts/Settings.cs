using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private Dropdown resolutions;
    [SerializeField] private Button applyButton;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider volumeSlider;
    private Resolution[] resolutionsList;

    void Awake()
    {
        resolutionsList = Screen.resolutions;
        Load();
        BuildMenu();
    }

    void Load()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
    }

    void Save()
    {
        PlayerPrefs.Save();
    }

    string ResToString(Resolution res)
    {
        return res.width + " x " + res.height;
    }

    void RefreshDropdown()
    {
        resolutions.RefreshShownValue();
    }

    void BuildMenu()
    {
        resolutions.ClearOptions();

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutionsList.Length; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = ResToString(resolutionsList[i]);
            options.Add(option);

            if (resolutionsList[i].height == Screen.currentResolution.height && resolutionsList[i].width == Screen.currentResolution.width)
            {
                currentResolutionIndex = i;
            }
        }

        resolutions.options = options;
        resolutions.value = currentResolutionIndex;

        applyButton.onClick.AddListener(ApplySettings);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void ApplySettings()
    {
        Resolution selectedResolution = resolutionsList[resolutions.value];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenToggle.isOn);
        Save();
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }
}