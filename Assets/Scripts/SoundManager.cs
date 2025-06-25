using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    public static SoundManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip menuBackgroundMusic;
    public AudioClip gameplayBackgroundMusic;

    public AudioClip buttonTap;
    public AudioClip pointSound;
    public AudioClip binFull;
    public AudioClip binWrong;
    public AudioClip fieldFillUpWarning;
    public AudioClip quizCorrect;
    public AudioClip quizIncorrect;
    public AudioClip[] glass;
    public AudioClip[] paper;
    public AudioClip[] plastic;
    public AudioClip[] electronic;
    public AudioClip[] organic;
    int order = 0;

    private bool isMusicSilent = false;
    private bool isSFXSilent = false;

    private void Awake()
    {
        Instance = this;
    }
    public void Button()
    {
        PlaySFX(buttonTap);
    }

    public void Point()
    {
        if (false) PlaySFX(pointSound);
    }

    public void BinFull()
    {
        PlaySFX(binFull);
    }
    public void BinWrong()
    {
        PlaySFX(binWrong);
    }
    public void FieldFillUpWarning()
    {
        PlaySFX(fieldFillUpWarning);
    }
    public void QuizCorrect()
    {
        PlaySFX(quizCorrect);
    }
    public void QuizIncorrect()
    {
        PlaySFX(quizIncorrect);
    }
    public void Glass()
    {
        PlaySFX(glass[order]);
        order++;
        order = order % 5;
    }

    public void Paper()
    {
        PlaySFX(paper[order]);
        order++;
        order = order % 5;
    }

    public void Plastic()
    {
        PlaySFX(plastic[order]);
        order++;
        order = order % 5;
    }

    public void Electronic()
    {
        PlaySFX(electronic[order]);
        order++;
        order = order % 5;
    }

    public void Organic()
    {
        PlaySFX(organic[order]);
        order++;
        order = order % 5;
    }
    public bool IsMusicSilent() => isMusicSilent;
    public bool IsSFXSilent() => isSFXSilent;

    /*public void Initialize(
        AudioClip menuBackgroundMusic,
        AudioClip gameplayBackgroundMusic,
        AudioClip buttonClick,
        AudioClip enterBin,
        AudioSource musicSource,
        AudioSource sfxSource)
    {
        this.menuBackgroundMusic = menuBackgroundMusic;
        this.gameplayBackgroundMusic = gameplayBackgroundMusic;
        this.buttonTap = buttonClick;
        this.pointSound = enterBin;
        this.musicSource = musicSource;
        this.sfxSource = sfxSource;
    }*/
    /*public void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }*/

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
