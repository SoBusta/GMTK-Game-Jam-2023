using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
{
    public AudioMixer myAudioMixerMusic;

    public AudioMixer myAudioMixerSFX;

    public Slider musicSlider;

    public Slider sfxSlider;

    private void Start()
    {
        if(PlayerPrefs.GetFloat("VolumeMus") == 0)
        {
            PlayerPrefs.SetFloat("VolumeMus", musicSlider.maxValue / 2f);
        }
        if (PlayerPrefs.GetFloat("VolumeSfx") == 0)
        {
            PlayerPrefs.SetFloat("VolumeSfx", sfxSlider.maxValue / 2f);
        }

        //musicSlider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("VolumeMus") / 20f));

        //sfxSlider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("VolumeSfx") / 20f));

    }
    public float GetMixerVolume(string exposedParameterName)
    {
        float volume;

        myAudioMixerMusic.GetFloat(exposedParameterName, out volume);

        return volume;

    }
    public float GetMixerSFX(string exposedParameterName)
    {
        float volume;

        myAudioMixerSFX.GetFloat(exposedParameterName, out volume);

        return volume;

    }
    public void SetMusicVolume(float sliderValue)
    {
        myAudioMixerMusic.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20f);

        //Sauvegarde dans les PlayerPrefs
        PlayerPrefs.SetFloat("VolumeMus", GetMixerVolume("MusicVolume"));

    }

    public void SetSFXVolume(float sliderValue)
    {
        myAudioMixerSFX.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20f);

        //Sauvegarde dans les PlayerPrefs
        PlayerPrefs.SetFloat("VolumeSfx", GetMixerSFX("SFXVolume"));
    }
}
