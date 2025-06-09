using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip menuBackgroundMusic;
    public AudioClip gameplayBackgroundMusic;
    public AudioClip buttonClick;
    public AudioClip enterBin;

    private bool isMusicSilent = false;
    private bool isSFXSilent = false;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsMusicSilent() => isMusicSilent;
    public bool IsSFXSilent() => isSFXSilent;

    public void Initialize(
        AudioClip menuBackgroundMusic,
        AudioClip gameplayBackgroundMusic,
        AudioClip buttonClick,
        AudioClip enterBin,
        AudioSource musicSource,
        AudioSource sfxSource)
    {
        this.menuBackgroundMusic = menuBackgroundMusic;
        this.gameplayBackgroundMusic = gameplayBackgroundMusic;
        this.buttonClick = buttonClick;
        this.enterBin = enterBin;
        this.musicSource = musicSource;
        this.sfxSource = sfxSource;
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (musicSource.clip != bgmClip)
        {
            musicSource.Stop();
            musicSource.clip = bgmClip;
            musicSource.loop = true;
            musicSource.Play();
        }
        ApplyMusicVolume();
    }

    public void StopBGM()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!isSFXSilent)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    public void ToggleMusic()
    {
        isMusicSilent = !isMusicSilent;
        ApplyMusicVolume();
    }

    public void ToggleSFX()
    {
        isSFXSilent = !isSFXSilent;
        ApplySFXVolume();
    }

    public void SetMusicActive(bool active)
    {
        isMusicSilent = !active;
        ApplyMusicVolume();
    }

    public void SetSFXActive(bool active)
    {
        isSFXSilent = !active;
        ApplySFXVolume();
    }

    private void ApplyMusicVolume()
    {
        musicSource.volume = isMusicSilent ? 0f : 1f;
    }

    private void ApplySFXVolume()
    {
        sfxSource.volume = isSFXSilent ? 0f : 1f;
    }
}
