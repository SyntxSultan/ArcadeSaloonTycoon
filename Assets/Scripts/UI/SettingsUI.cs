using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    private void Start()
    {
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);
    }

    private void OnEnable()
    {
        if (AudioManager.Instance) musicSlider.value = AudioManager.Instance.GetMusicVolume();
        if (AudioManager.Instance) sfxSlider.value = AudioManager.Instance.GetSFXVolume();
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
