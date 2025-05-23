using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsMenu : MonoBehaviour
{
    //BOJAN: generalno je dobra praksa da sve dugmice drzis kao SerializeField
    //BOJAN: i da onda u awake/start koristis onClick.SetListener()
    //BOJAN: zato sto je preglednije kroz kod da trazis sta dugme radi, nego da sve povezujes kroz scene/prefabe
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
