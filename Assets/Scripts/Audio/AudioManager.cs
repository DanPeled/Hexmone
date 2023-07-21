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
    private float desiredMusicVolume;
    private AudioClip currentMusic;

    public static AudioManager i;

    void Awake()
    {
        i = this;
    }

    void Start()
    {
        desiredMusicVolume = musicPlayer.volume;
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
        // Cancel any existing fade
        musicPlayer.DOKill();

        // Fade out if required
        if (fade)
        {
            musicPlayer.DOFade(0, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);
        }

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        // Fade in if required
        if (fade)
        {
            musicPlayer.volume = 0; // Start at 0 volume
            musicPlayer.DOFade(desiredMusicVolume, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);
        }
    }

    IEnumerator StopMusicAsync()
    {
        // Cancel any existing fade
        musicPlayer.DOKill();

        musicPlayer.DOFade(0, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        musicPlayer.Stop();
        musicPlayer.volume = desiredMusicVolume; // Set the music volume to the desired value
        musicPlayer.DOFade(desiredMusicVolume, fadeDuration);
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
        musicPlayer.DOFade(desiredMusicVolume, fadeDuration);
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
