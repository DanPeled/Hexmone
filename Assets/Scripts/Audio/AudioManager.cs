using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using AYellowpaper.SerializedCollections;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    [Header("SFXs")]
    public List<AudioData> sfxList;
    public Dictionary<AudioId, AudioData> sfxLookup;

    [Header("Audio Players")]
    public AudioSource musicPlayer;
    public AudioSource sfxPlayer;

    public float fadeDuration = 0.75f;
    public float originalMusicVolume;
    public static AudioManager i;
    public AudioClip currentMusic;

    void Awake()
    {
        i = this;
    }

    void Start()
    {
        originalMusicVolume = musicPlayer.volume;
        sfxLookup = sfxList.ToDictionary(x => x.audioId);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    {
        if (clip == null || clip == currentMusic)
            return;
        currentMusic = clip;
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

    IEnumerator StopMusicAsync()
    {
        yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        musicPlayer.Stop();
    }

    public void PlaySFX(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null)
            return;

        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }

        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySFX(AudioId id, bool pauseMusic = false)
    {
        if (!sfxLookup.ContainsKey(id))
        {
            return;
        }
        var audioData = sfxLookup[id];
        PlaySFX(audioData.clip, pauseMusic);
    }

    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVolume, fadeDuration);
    }

    public void StopMusic()
    {
        StartCoroutine(StopMusicAsync());
    }

    public void StopSFX()
    {
        sfxPlayer.Stop();
    }
}

public enum AudioId
{
    UISelect,
    Hit,
    Faint,
    ExpGain,
    ItemObtained,
    CreatureObtained
}

public enum AudioOutput
{
    SFX,
    MUSIC
}

[System.Serializable]
public class AudioData
{
    public AudioId audioId;
    public AudioClip clip;
    public AudioOutput output;
}
