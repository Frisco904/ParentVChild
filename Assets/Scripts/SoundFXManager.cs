using System;
using UnityEngine;
using UnityEngine.Audio;

public enum MixerGroup
{
    Music,
    World,
    Menu
}
public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource SoundFXObject;
    private AudioMixer mixer;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mixer = Resources.Load("MainMix") as AudioMixer;
        }
    }
    
    public void PlaySoundFXClip(AudioClip audioClip, MixerGroup mixerGroup, Transform parent, float volume, float spacialBlend = .8f, bool looping = false, float pitch = 1f)
    {
        AudioSource audioSource = Instantiate(SoundFXObject, parent.position, Quaternion.identity, parent);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = spacialBlend;
        audioSource.loop = looping;
        audioSource.pitch = pitch;

        // Set Mixer group
        switch (mixerGroup)
        {
            case MixerGroup.Music:
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
                break;
            case MixerGroup.World:
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("World")[0];
                break;
            case MixerGroup.Menu:
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Menu")[0];
                break;
        }

        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
        
    }
}