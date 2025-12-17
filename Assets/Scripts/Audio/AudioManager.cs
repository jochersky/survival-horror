using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    public void PlaySFX(AudioClip clip, AudioSource source = null, float volume = 1)
    {
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
