using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class SettingsMenu : MonoBehaviour
{
    private MixerController mixer;

    public AudioMixer musicMixer;

    public AudioMixer sfxMixer;

    public Slider sliderMusic;

    public Slider sliderSFX;

    private void Start()
    {
        mixer = gameObject.AddComponent<MixerController>();

        mixer.myAudioMixerMusic = musicMixer;

        mixer.myAudioMixerSFX = sfxMixer;

        mixer.musicSlider = sliderMusic;

        mixer.sfxSlider = sliderSFX;

        mixer.sfxSlider = sliderSFX;

    }

    public void SetMusic(float volume)
    {
        mixer.SetMusicVolume(volume);
    }
    public void SetSFX(float volume)
    {
        mixer.SetSFXVolume(volume);
    }

}
