using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.UIElements;

public class Learn : MonoBehaviour
{
    public GameObject AchievementPrefab;
    public GameObject Content;
    public Scrollbar scrollVerical;

    public VerticalLayoutGroup verticalLayoutGroup;//u odnosu na height mu postavi spacing 0 je za 2400, i onda od toga oduzmi 40
    //u odnosu na width mu postavi scale, u

    public GameObject AchievementDescription;
    public UnityEngine.UI.Button achievementBack;

    //case 1
    public TextMeshProUGUI type;
    public TextMeshProUGUI earnMultipliers;
    public TextMeshProUGUI currentMultiplierTitle;
    public TextMeshProUGUI currentMultiplier;
    public TextMeshProUGUI nextTierTitle;
    public TextMeshProUGUI nextTier;

    public RectTransform canvasRectTransform;

    public void ReturnHome()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Menu);
    }
    void Start()
    {
        /*
        List<AchievementData> achievementDataList = new List<AchievementData>
        {
            new AchievementData("TotalCount"),
            new AchievementData("PlasticMetalCount"),
            new AchievementData("PaperCount"),
            new AchievementData("GlassCount"),
            new AchievementData("ElectronicsBatteriesCount"),
            new AchievementData("OrganicCount"),
            new AchievementData("CountChangeBins"),
            new AchievementData("Trivia"),
            new AchievementData("CatchTrashMidAir")
        };

        achievementDataList.Sort((a, b) => a.rank.CompareTo(b.rank));//prvo idu najnizi rankovi
        achievementDataList.Sort((a, b) => b.fill.CompareTo(a.fill));//prvo idu najveci fillovi
        // 2400:2400-n = -40:x
        //nije u heightu nego u odnosu
        //2400/1080 : (2400/1080) - (height/width) = -40
        //verticalLayoutGroup.spacing = (-40 * (Screen.height - 2400)) / 2400;
        //odnos ekrana

        //verticalLayoutGroup.spacing = (-40f * ( ((float)Screen.height / 2040f) / ((float)Screen.width/1080f)) ) * ((Screen.height / Screen.width)/(2040f/1080f)); 
        verticalLayoutGroup.spacing = Mathf.Lerp(-60f, 40f, Mathf.Clamp01((float)canvasRectTransform.rect.height / canvasRectTransform.rect.width - 0.4333f) / (0.5625f - 0.4333f));


        foreach (AchievementData achievementData in achievementDataList)
        {
            GameObject AchievementPrefabInitialized = Instantiate(AchievementPrefab);
            AchievementUI achievementUI = new AchievementUI(AchievementPrefabInitialized, achievementData);

            AchievementPrefabInitialized.transform.SetParent(Content.transform);

            Action<string[]> yourAction = (descriptionTexts) => {
                type.text = descriptionTexts[0];
                earnMultipliers.text = descriptionTexts[1];
                currentMultiplierTitle.text = descriptionTexts[2];
                currentMultiplier.text = descriptionTexts[3];
                nextTierTitle.text = descriptionTexts[4];
                nextTier.text = descriptionTexts[5];
            };
            string[] descriptionArguments = GetDescriptionArguments(achievementData);
            AchievementPrefabInitialized.AddComponent<UnityEngine.UI.Button>().onClick.AddListener(() => { yourAction.Invoke(descriptionArguments); OpenDescription(); });
            AchievementPrefabInitialized.transform.GetComponent<RectTransform>().localScale = new Vector2(((float)canvasRectTransform.rect.width) / 1080f, ((float)canvasRectTransform.rect.width) / 1080f);
        }*/
        scrollVerical.value = 0.735f;

    }
    public void LateUpdate()
    {
        if (scrollVerical.value > 0.735f)
        {
            scrollVerical.value = 0.735f;
        }
    }

    public string[] GetDescriptionArguments(AchievementData achievementData)
    {
        string[] arguments = new string[6];

        string mainType = "";
        switch (achievementData.name)
        {
            case "TotalCount":
                arguments[0] = "Recycled trash";
                arguments[1] = "EARN MULTIPLIERS!";
                arguments[2] = "CURRENT MULTIPLIER";
                arguments[3] = "+" + (((achievementData.rank - 1) * 0.2f) * 100).ToString() + "%";
                if (achievementData.rank != 6)
                {
                    arguments[4] = "NEXT TIER";
                    arguments[5] = "+" + ((achievementData.rank * 0.2f) * 100).ToString() + "%";
                }

                break;
            case "ElectronicsBatteriesCount":
            case "OrganicCount":
            case "PlasticMetalCount":
            case "PaperCount":
            case "GlassCount":
                string trashName = "";
                switch (achievementData.name)
                {
                    case "ElectronicsBatteriesCount":
                        trashName = "Electronics"; //"Electronics & Batteries";//"Electronics";
                        break;
                    case "OrganicCount":
                        trashName = "Organic";
                        break;
                    case "PlasticMetalCount":
                        trashName = "Plastic & Metal";
                        break;
                    case "PaperCount":
                        trashName = "Paper";
                        break;
                    case "GlassCount":
                        trashName = "Glass";
                        break;
                }
                arguments[0] = trashName;
                arguments[1] = $"All {trashName} trash is worth more!";
                arguments[2] = "CURRENT";
                arguments[3] = "+" + ((achievementData.rank)).ToString();
                if (achievementData.rank != 6)
                {
                    arguments[4] = "NEXT TIER";
                    arguments[5] = "+" + ((achievementData.rank + 1)).ToString();
                }
                break;
            case "CountChangeBins":
                if (achievementData.rank != 6)
                {
                    arguments[0] = "BECOME A BIN MASTER!";
                }
                else
                {
                    arguments[0] = "BIN MASTER!";
                }
                arguments[1] = "Empty the bins, be responsible!";
                //arguments[2] = "Reward:";
                break;
            case "Trivia":
                if (achievementData.rank != 6)
                {
                    arguments[0] = "TRIVIA BEGINNER!";
                }
                else
                {
                    arguments[0] = "TRIVIA KING!";
                }
                arguments[1] = "Answer all trivia questions correctly!";
                //arguments[2] = "Reward:";
                break;
            case "CatchTrashMidAir":
                arguments[0] = "MID AIR!";
                arguments[1] = "Catch trash mid air to get this achievement!";
                //arguments[2] = "Reward:";
                break;
        }

        return arguments;
    }
}
