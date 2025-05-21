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

    private bool isSilent = false; // Replaces isMuted

    private void Awake()
    {
        Instance = this;
    }
    public bool IsSilent()
    {
        return isSilent;
    }

    public void Initialize(AudioClip menuBackgroundMusic, AudioClip gameplayBackgroundMusic, AudioClip buttonClick, AudioClip enterBin, AudioSource musicSource, AudioSource sfxSource)
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
        ApplyVolume(); // Apply current volume state
    }

    public void StopBGM()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    public bool ToggleSound()
    {
        SetSoundActive(isSilent);
        return !isSilent;//is Silent se menja u grodnjoj funkciji
    }

    public void SetSoundActive(bool active)
    {
        isSilent = !active;
        ApplyVolume();
    }


    private void ApplyVolume()
    {
        musicSource.volume = isSilent ? 0f : 1f;
        sfxSource.volume = isSilent ? 0f : 1f;
    }
}
