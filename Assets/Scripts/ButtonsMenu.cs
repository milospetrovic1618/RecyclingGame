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
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);
    }

    public void LoadAchievements()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Achievements);
    }

    public void Options()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadOptions();

    }
    public void Learn()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Learn);

    }
}
