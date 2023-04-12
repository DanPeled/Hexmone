using System.Collections;
using UnityEngine;
using DG.Tweening;
public class AudioManager : MonoBehaviour
{
    [Header("Audio Players")]
    public AudioSource musicPlayer;
    public AudioSource sfxPlayer;

    public float fadeDuration = 0.75f;
    public float originalMusicVolume;
    public static AudioManager i;
    void Awake()
    {
        i = this;
    }
    void Start()
    {
        originalMusicVolume = musicPlayer.volume;
    }
    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    {
        if (clip == null) return;
        StartCoroutine(PlayMusicAsync(clip, loop, fade));
    }
    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            yield return musicPlayer.DOFade(originalMusicVolume, fadeDuration).WaitForCompletion();
        }
    }
}