using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button[] closeButtons;
    
    private void Start()
    {
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);
        foreach (var button in closeButtons)
        {
            button.onClick.AddListener(ScreenManager.Instance.CloseSettingsPopup);
        }
    }

    private void OnEnable()
    {
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    private void OnSfxSliderValueChanged(float arg0)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(arg0);
        }
    }

    private void OnMusicSliderValueChanged(float arg0)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(arg0);
        }
    }
}
