using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Animator transition;
    public bool notOPEnough = false;

    public PJSO playerSO;

    public List<PinchoSO> posibles_pinchos = new List<PinchoSO>();

    private List<AudioSource> pausedSFXSources = new List<AudioSource>();

    public float maseterVolumen;
    public float musicVolumen;

    public float SFXVolumen;
    public int nivelAct;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    private bool musicMuted = false;
    private bool sfxMuted = false;
    private List<AudioSource> activeSFXSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource != null)
            {
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.outputAudioMixerGroup = musicMixerGroup;
                musicSource.priority = 128;
            }
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void LoadScene(string name)
    {
        StartCoroutine(Transition(name));
    }
    IEnumerator Transition(string name)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        transition.SetTrigger("Return");
        SceneManager.LoadScene(name);
    }

    public void createHand()
    {
        playerSO.baraja.Clear();
        List<PinchoSO> baraja = new List<PinchoSO>();
        baraja.Add(Instantiate(posibles_pinchos[0]));
        baraja.Add(Instantiate(posibles_pinchos[0]));
        baraja.Add(Instantiate(posibles_pinchos[1]));
        baraja.Add(Instantiate(posibles_pinchos[2]));
        for (int i = 0; i < 3; i++)
        {
            int j = UnityEngine.Random.Range(0, posibles_pinchos.Count);
            baraja.Add(Instantiate(posibles_pinchos[j]));
        }
        playerSO.baraja = baraja;
    }
    public void PlayMusic(AudioClip musicClip, float relativeVolume = 1f, float speed = 1f)
    {
        if (musicClip == null || musicSource == null) return;

        relativeVolume = Mathf.Clamp01(relativeVolume);
        speed = Mathf.Max(0.01f, speed);

        musicSource.volume = relativeVolume;
        musicSource.pitch = speed;
        musicSource.mute = musicMuted;

        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void StopMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
    }
    public void PlaySFX(AudioClip sfxClip, float relativeVolume = 1f, float speed = 1f)
    {
        if (sfxClip == null) return;
        if (sfxMuted) return;

        relativeVolume = Mathf.Clamp01(relativeVolume);
        speed = Mathf.Max(0.01f, speed);

        GameObject sfxObject = new GameObject("SFX_" + sfxClip.name);
        sfxObject.transform.SetParent(transform);

        AudioSource source = sfxObject.AddComponent<AudioSource>();

        source.clip = sfxClip;
        source.volume = relativeVolume;
        source.pitch = speed;
        source.loop = false;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.priority = 64;
        source.mute = sfxMuted;

        activeSFXSources.Add(source);

        source.Play();

        StartCoroutine(RemoveSFXSourceWhenFinished(source));
    }
    private IEnumerator RemoveSFXSourceWhenFinished(AudioSource source)
    {
        while (source != null && (source.isPlaying || pausedSFXSources.Contains(source)))
        {
            yield return null;
        }

        if (source != null)
        {
            activeSFXSources.Remove(source);
            pausedSFXSources.Remove(source);
            Destroy(source.gameObject);
        }
    }
    public void SetMusicMuted(bool muted)
    {
        musicMuted = muted;

        if (musicSource != null)
        {
            musicSource.mute = musicMuted;
        }
    }
    public void ToggleMusicMuted() { SetMusicMuted(!musicMuted); }
    public void SetSFXMuted(bool muted)
    {
        sfxMuted = muted;

        for (int i = activeSFXSources.Count - 1; i >= 0; i--)
        {
            if (activeSFXSources[i] == null)
            {
                activeSFXSources.RemoveAt(i);
                continue;
            }

            activeSFXSources[i].mute = sfxMuted;
        }
    }
    public void ToggleSFXMuted() { SetSFXMuted(!sfxMuted); }
    public void StopAllSFX()
    {
        for (int i = activeSFXSources.Count - 1; i >= 0; i--)
        {
            if (activeSFXSources[i] != null)
            {
                Destroy(activeSFXSources[i].gameObject);
            }
        }

        activeSFXSources.Clear();
        pausedSFXSources.Clear();
    }

    public AudioSource PlayLoopSFX(AudioClip sfxClip, float relativeVolume = 1f, float speed = 1f)
    {
        if (sfxClip == null) return null;

        relativeVolume = Mathf.Clamp01(relativeVolume);
        speed = Mathf.Max(0.01f, speed);

        GameObject sfxObject = new GameObject("LoopSFX_" + sfxClip.name);
        sfxObject.transform.SetParent(transform);

        AudioSource source = sfxObject.AddComponent<AudioSource>();

        source.clip = sfxClip;
        source.volume = relativeVolume;
        source.pitch = speed;
        source.loop = true;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.priority = 64;
        source.mute = sfxMuted;

        activeSFXSources.Add(source);

        source.Play();

        return source;
    }

    public void StopSFX(AudioSource source)
    {
        if (source == null) return;

        activeSFXSources.Remove(source);
        pausedSFXSources.Remove(source);

        Destroy(source.gameObject);
    }

    public void PauseAllSFX()
    {
        for (int i = activeSFXSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = activeSFXSources[i];

            if (source == null)
            {
                activeSFXSources.RemoveAt(i);
                continue;
            }

            if (source.isPlaying && !pausedSFXSources.Contains(source))
            {
                source.Pause();
                pausedSFXSources.Add(source);
            }
        }
    }

    public void ResumeAllSFX()
    {
        for (int i = pausedSFXSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = pausedSFXSources[i];

            if (source != null)
            {
                source.mute = sfxMuted;
                source.UnPause();
            }
        }

        pausedSFXSources.Clear();
    }

}

