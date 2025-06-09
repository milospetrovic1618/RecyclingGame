using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class BootMenu : MonoBehaviour
{
    public static BootMenu Instance;

    public GameObject CameraPrefab;
    public GameObject EventSystemPrefab;

    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI totalScoreOutlineText;

    public UnityEngine.UI.Image SoundToggle;
    public Sprite SoundOnTex;
    public Sprite SoundOffTex;
    public GameObject CameraInstantiated;
    public GameObject EventSystemInstantiated;
    public void Awake()
    {
        Instance = this;
        SoundManager.Instance.PlayBGM(SoundManager.Instance.menuBackgroundMusic);

        CameraInstantiated = Instantiate(CameraPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        CameraInstantiated.tag = "MainCamera";

        EventSystemInstantiated = Instantiate(EventSystemPrefab, Vector3.zero, quaternion.identity);

        Material totalScoreOutlineMat = new Material(totalScoreOutlineText.font.material);
        totalScoreOutlineText.fontMaterial = totalScoreOutlineMat;
        totalScoreOutlineText.outlineWidth = 1f;
        totalScoreOutlineText.outlineColor = totalScoreOutlineText.color;

        if (SoundManager.Instance.IsMusicSilent())
        {
            SoundToggle.sprite = SoundOffTex;
        }
        else
        {
            SoundToggle.sprite = SoundOnTex;
        }
    }
    private void Start()
    {
        //kad se inicijalizuje SaveSystem i player
        string scoreStr = SaveSystem.Instance.Player.GetHighScore().ToString();
        int len = scoreStr.Length;
        float fontSize;

        if (len < 5)
        {
            fontSize = 200f;
        }
        else
        {
            // Lerp from 190 (length 5) to 100 (length 11)
            float t = Mathf.InverseLerp(5f, 11f, len);
            fontSize = Mathf.Lerp(190f, 80f, t);
        }

        // Apply font size
        totalScoreText.fontSize = (int)fontSize;
        totalScoreOutlineText.fontSize = (int)fontSize;

        // Update the text
        totalScoreText.text = scoreStr;
        totalScoreOutlineText.text = scoreStr;
    }
}
