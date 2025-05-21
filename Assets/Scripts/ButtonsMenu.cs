using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsMenu : MonoBehaviour
{
    public void LoadGameplay()
    {
        SoundManager.Instance.PlayButtonClick();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);
    }

    public void LoadQuiz()
    {
        SoundManager.Instance.PlayButtonClick();
    }
    public void ShowBedges()
    {
        SoundManager.Instance.PlayButtonClick();
    }
    public void ShowItems()
    {
        SoundManager.Instance.PlayButtonClick();
    }
    public void SoundToggle()
    {
        SoundManager.Instance.PlayButtonClick();
        if (SoundManager.Instance.ToggleSound())
        {
            BootMenu.Instance.SoundToggle.sprite = BootMenu.Instance.SoundOnTex;
        }
        else
        {
            BootMenu.Instance.SoundToggle.sprite = BootMenu.Instance.SoundOffTex;
        }
    }
}
