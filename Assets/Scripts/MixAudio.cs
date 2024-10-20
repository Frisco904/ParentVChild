using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class MixAudio : MonoBehaviour
{
    public AudioMixer masterMixer;
    
    public void SetMasterVolume(float vol)
    {
        masterMixer.SetFloat("masterVolume", vol);
    }

    public void SetMusicVolume(float vol)
    {
        masterMixer.SetFloat("musicVolume", vol);
    }

    public void SetSfxVolume(float vol)
    {
        masterMixer.SetFloat("sfxVolume", vol);
    }
}
