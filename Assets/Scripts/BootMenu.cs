using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
public class BootMenu : MonoBehaviour
{
    public static BootMenu Instance;

    public UnityEngine.UI.Image SoundToggle;
    public Sprite SoundOnTex;
    public Sprite SoundOffTex;
    public void Awake()
    {
        Instance = this;
        SoundManager.Instance.PlayBGM(SoundManager.Instance.menuBackgroundMusic);


        if(SoundManager.Instance.IsSilent())
        {
            SoundToggle.sprite = SoundOffTex;
        }
        else
        {
            SoundToggle.sprite = SoundOnTex;
        }
    }
}
