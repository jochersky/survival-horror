using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = System.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SfxSO sfxSO;
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;
    
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(SfxType type, AudioSource source = null, float volume = 1)
    {
        SfxList sfxList = sfxSO.sfx[(int)type];
        AudioClip[] sfx = sfxList.sfx;
        if (sfx.Length == 0) return;
        
        AudioClip clip = sfx[UnityEngine.Random.Range(0, sfx.Length)];
        float v = sfxList.volume;
        if (v < 1) volume = v;
        
        if (!source)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            source.PlayOneShot(clip, volume);
        }
    }
}

[Serializable]
public struct SfxList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixerGroup;
    public AudioClip[] sfx;
}
