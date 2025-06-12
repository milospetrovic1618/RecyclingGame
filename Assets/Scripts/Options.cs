using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class Options : MonoBehaviour
{
    public UnityEngine.UI.Image musicButtonBackground;
    public TextMeshProUGUI musicText;
    public UnityEngine.UI.Image sfxButtonBackground;
    public TextMeshProUGUI sfxText;
    private void Awake()
    {
        UpdateMusicButton();
        UpdateSFXButton();
    }
    public void UpdateMusicButton()
    {
        if (SoundManager.Instance.IsMusicSilent())
        {
            musicButtonBackground.sprite = Resources.Load<Sprite>("ButtonRedBackground");
            musicText.text = "MUSIC OFF";
        }
        else
        {
            musicButtonBackground.sprite = Resources.Load<Sprite>("ButtonGreenBackground");
            musicText.text = "MUSIC ON";
        }

    }
    public void UpdateSFXButton()
    {
        if (SoundManager.Instance.IsSFXSilent())
        {
            sfxButtonBackground.sprite = Resources.Load<Sprite>("ButtonRedBackground");
            sfxText.text = "SFX OF";
        }
        else
        {
            sfxButtonBackground.sprite = Resources.Load<Sprite>("ButtonGreenBackground");
            sfxText.text = "SFX ON";
        }
    }
    public void CloseScene()
    {
        SoundManager.Instance.PlayButtonClick();
        BootMain.Instance.UnloadOptions();
    }
    public void MusicToggle()
    {
        SoundManager.Instance.PlayButtonClick();
        SoundManager.Instance.ToggleMusic();
        UpdateMusicButton();
    }
    public void SFXToggle()
    {
        SoundManager.Instance.PlayButtonClick();
        SoundManager.Instance.ToggleSFX();
        UpdateSFXButton();
    }
    public void AboutUs()
    {
        SoundManager.Instance.PlayButtonClick();
    }
}
