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

[Serializable]
public class AchievementUI
{
    public UnityEngine.UI.Image background;
    public TextMeshProUGUI titleOutline;
    public TextMeshProUGUI progressTextOutline;
    public TextMeshProUGUI title;
    public TextMeshProUGUI progressText;
    public UnityEngine.UI.Image fillBackground;
    public UnityEngine.UI.Image fill;
    public Color[] rankColor = new Color[]
{
    new Color(0.263f, 0.290f, 0.329f),
    new Color(0.604f, 0.443f, 0.180f),
    new Color(0.384f, 0.553f, 0.769f), 
    new Color(0.807f, 0.549f, 0.129f), 
    new Color(0.667f, 0.608f, 0.753f), 
    new Color(0.231f, 0.573f, 0.294f), 
};

    public AchievementUI (GameObject AchievementPrefabInitialized, AchievementData achievementData)
    {
        AchievementPrefabInitialized.name = achievementData.name;
        background = AchievementPrefabInitialized.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        titleOutline = AchievementPrefabInitialized.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        progressTextOutline = AchievementPrefabInitialized.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        title = AchievementPrefabInitialized.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        progressText = AchievementPrefabInitialized.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        fillBackground = AchievementPrefabInitialized.transform.GetChild(5).GetComponent<UnityEngine.UI.Image>();
        fill = AchievementPrefabInitialized.transform.GetChild(6).GetComponent<UnityEngine.UI.Image>();
        
        background.sprite = Resources.Load<Sprite>($"Achievements/{achievementData.rank}");
        titleOutline.text = achievementData.title;
        progressTextOutline.text = achievementData.progressText;
        title.text = achievementData.title;
        progressText.text = achievementData.progressText;
        if (achievementData.noFill)
        {
            fillBackground.color = new Color(0,0,0,0);
            fill.color = new Color(0, 0, 0, 0);
        }
        else
        {
            fillBackground.color = rankColor[achievementData.rank-1];

            fill.color = Color.white;
            fill.fillAmount = achievementData.fill;
        }

        //OUTLINE
        foreach(TextMeshProUGUI backText in new List<TextMeshProUGUI> { titleOutline, progressTextOutline })
        {

            Material backTextMat = new Material(backText.font.material);
            backText.fontMaterial = backTextMat;
            backText.outlineWidth = 1f;
            backText.outlineColor = rankColor[achievementData.rank - 1]; ;
        }
    }
}
public class AchievementData
{
    public string name;
    public string title;
    public string progressText = "";
    public int rank;
    public float fill;//-1 -> destroyFill and and fill background
    public bool noFill;
    /*
    public UnityEngine.UI.Image background;
    public TextMeshProUGUI title;
    public TextMeshProUGUI progressText;
    public UnityEngine.UI.Image fillBackground;
    public UnityEngine.UI.Image fill;*/
    public AchievementData(string name)
    {
        this.name = name;
        switch (name)
        {
            case "TotalCount":
                title = "RECYCLED TRASH";
                rank = SaveSystem.Instance.Player.GetRankTotalCount();

                if (rank == 6)
                {
                    progressText = PlayerSave.totalMultiplierTresholds[4] + "+";
                    noFill = true;
                }
                else
                {
                    long TotalCount = SaveSystem.Instance.Player.TotalCount;
                    progressText = TotalCount+ "/" +PlayerSave.totalMultiplierTresholds[rank-1];
                    fill = (float)TotalCount / (float)PlayerSave.totalMultiplierTresholds[rank - 1];
                }
                break;
            case "PlasticMetalCount":
                title = "PLASTIC AND METAL WASTE";
                long PlasticMetalCount = SaveSystem.Instance.Player.PlasticMetalCount;
                rank = SaveSystem.Instance.Player.GetRankBin(PlasticMetalCount);


                if (rank == 6)
                {
                    progressText = PlayerSave.binMultiplierTresholds[4] + "+";
                    noFill = true;
                }
                else
                {
                    progressText = PlasticMetalCount + "/" + PlayerSave.binMultiplierTresholds[rank - 1];
                    fill = (float)PlasticMetalCount / (float)PlayerSave.binMultiplierTresholds[rank - 1];
                }
                break;
            case "PaperCount":
                title = "PAPER WASTE";
                long PaperCount = SaveSystem.Instance.Player.PaperCount;
                rank = SaveSystem.Instance.Player.GetRankBin(PaperCount);

                if (rank == 6)
                {
                    progressText = PlayerSave.binMultiplierTresholds[4] + "+";
                    noFill = true;
                }
                else
                {
                    progressText = PaperCount + "/" + PlayerSave.binMultiplierTresholds[rank - 1];
                    fill = (float)PaperCount / (float)PlayerSave.binMultiplierTresholds[rank - 1];
                }
                break;
            case "GlassCount":
                title = "GLASS WASTE";
                long GlassCount = SaveSystem.Instance.Player.GlassCount;
                rank = SaveSystem.Instance.Player.GetRankBin(GlassCount);

                if (rank == 6)
                {
                    progressText = PlayerSave.binMultiplierTresholds[4] + "+";
                    noFill = true;
                }
                else
                {
                    progressText = GlassCount + "/" + PlayerSave.binMultiplierTresholds[rank - 1];
                    fill = (float)GlassCount / (float)PlayerSave.binMultiplierTresholds[rank - 1];
                }
                break;
            case "ElectronicsBatteriesCount":
                title = "ELECTRONICS WASTE";
                long ElectronicsBatteriesCount = SaveSystem.Instance.Player.ElectronicsBatteriesCount;
                rank = SaveSystem.Instance.Player.GetRankBin(ElectronicsBatteriesCount);

                if (rank == 6)
                {
                    progressText = PlayerSave.binMultiplierTresholds[4] + "+";
                    noFill = true;
                }
                else
                {
                    progressText = ElectronicsBatteriesCount + "/" + PlayerSave.binMultiplierTresholds[rank - 1];
                    fill = (float)ElectronicsBatteriesCount / (float)PlayerSave.binMultiplierTresholds[rank - 1];
                }
                break;

            case "OrganicCount":
                title = "ORGANIC WASTE";
                long OrganicCount = SaveSystem.Instance.Player.OrganicCount;
                rank = SaveSystem.Instance.Player.GetRankBin(OrganicCount);

                if (rank == 6)
                {
                    progressText = PlayerSave.binMultiplierTresholds[4] + "+";
                    noFill = true;
                }
                else
                {
                    progressText = OrganicCount + "/" + PlayerSave.binMultiplierTresholds[rank - 1];
                    fill = (float)OrganicCount / (float)PlayerSave.binMultiplierTresholds[rank - 1];
                }
                break;
            case "CountChangeBins":
                title = "Change bins!";
                rank = SaveSystem.Instance.Player.GetRankCountChangeBins();
                //Debug.Log(rank-4);
                long ChangeBins = SaveSystem.Instance.Player.CountChangeBins;

                if (rank == 6)
                {
                    progressText = PlayerSave.changeBinsRankTresholds[2] + "+";
                    noFill = true;
                }
                else
                {
                    long changeBinsCount = SaveSystem.Instance.Player.CountChangeBins;
                    int index = Mathf.Clamp(rank - 3,0, PlayerSave.changeBinsRankTresholds.Length-1);
                    progressText = changeBinsCount + "/" + PlayerSave.changeBinsRankTresholds[index];
                    fill = (float)changeBinsCount / (float)PlayerSave.changeBinsRankTresholds[index];
                }
                break;
            case "Trivia":
                title = "Trivia!";
                rank = SaveSystem.Instance.Player.GetRankTrivia();

                int maxTrivia = Quiz.quizData.Count;
                if (rank == 6)
                {
                    progressText = maxTrivia.ToString();
                    noFill = true;
                }
                else
                {
                    int triviaCount = SaveSystem.Instance.Player.GetTriviaCount();
                    progressText = triviaCount + "/" + maxTrivia;
                    fill = (float)triviaCount / (float)maxTrivia;
                }
                break;
            case "CatchTrashMidAir":
                title = "Catch Trash Mid Air!";
                rank = SaveSystem.Instance.Player.GetRankCatchMidAir();
                noFill = true;
                progressText = "";
                break;
        }

        /*ui.background.sprite = Resources.Load<Sprite>($"Achievements/{rank}");*/
        if (rank == 6)
        {
            noFill = true;
        }
    }
}
public class Achievements : MonoBehaviour
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

    public void CloseDescription ()
    {
        AchievementDescription.SetActive(false);
    }
    public void OpenDescription()
    {
        AchievementDescription.SetActive(true);
    }
    public void ReturnHome()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Menu);
    }
    void Start()
    {
        List<AchievementData> achievementDataList = new List<AchievementData>
        {
            /*new AchievementData("TotalCount", "RECYCLED TRASH"),
            new AchievementData("PlasticMetalCount", "PLASTIC AND METAL WASTE"),
            new AchievementData("PaperCount", "PAPER WASTE"),
            new AchievementData("GlassCount", "GLASS WASTE"),
            new AchievementData("ElectronicsBatteriesCount", "ELECTRONICS WASTE"),
            new AchievementData("OrganicCount", "ORGANIC WASTE"),
            new AchievementData("CountChangeBins", "Change bins!"),
            new AchievementData("Trivia", "Trivia!"),
            new AchievementData("CatchTrashMidAir", "Catch Trash Mid Air!")*/
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
        }
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
