using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsGameplay : MonoBehaviour
{

    public void TogglePause()
    {
        bool pause = !BootGameplay.Instance.PauseUI.activeSelf;
        BootGameplay.Instance.PauseUI.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }
    public void Home()
    {
        BootMain.Instance.LoadSceneFromBoot(Scenes.Menu);
    }
    public void SoundToggle()
    {
        SoundManager.Instance.PlayButtonClick(); 
        if (SoundManager.Instance.ToggleSound())
        {
            BootGameplay.Instance.SoundToggle.sprite = BootGameplay.Instance.SoundOnTex;
        }
        else
        {
            BootGameplay.Instance.SoundToggle.sprite = BootGameplay.Instance.SoundOffTex;
        }
    }
    public void Retry()
    {
        BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);//ovo je najbolje resenje jer treba i za svaku kantu da vracam da nema moc itd... n eisplati mi se druga opcija bez pokretanja scene
        //TrashManager.Instance.GameOverUIScreenActivate(false);
    }
}
