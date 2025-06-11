using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerSave : GenericSave
{

    [JsonProperty]
    private bool tutorialFinished;

    //base fields
    [JsonProperty]
    private float totalScore;
    [JsonProperty]
    private float highScore;

    [JsonProperty]
    private bool catchTrashMidAir;
    [JsonProperty]
    //public HashSet<string> triviaHashset = new HashSet<string>();//da li je odgovorio na sva pitanja
    private HashSet<string> triviaHashset= new HashSet<string>();//da li je odgovorio na sva pitanja
    [JsonProperty]
    private long countChangeBins;

    [JsonProperty]
    public int lastRankCatchTrashMidAir = 1;
    [JsonProperty]
    public int lastRankTriviaHashset = 1;
    [JsonProperty]
    public int lastRankCountChangeBins = 1;
    //za ove ostale uporedjujes multipliere, i ako su razliciti onda izbacis najnoviji rank

    //base fields
    [JsonProperty]
    private long totalCount;
    [JsonProperty]
    private long paperCount;
    [JsonProperty]
    private long glassCount;
    [JsonProperty]
    private long plasticMetalCount;
    [JsonProperty]
    private long organicCount;
    [JsonProperty]
    private long electronicsBatteriesCount;


    //multiplier
    [JsonProperty]
    public float totalMultiplier = 1;
    [JsonProperty]
    public long paperMultiplier = 1;
    [JsonProperty]
    public long glassMultiplier = 1;
    [JsonProperty]
    public long plasticMetalMultiplier = 1;
    [JsonProperty]
    public long organicMultiplier = 1;
    [JsonProperty]
    public long electronicsBatteriesMultiplier = 1;

    public float ScoreIncrease(RecyclingType recyclingType, int count)//int caount dodat zbog moci, inace bi bilo ++ tj +=1
    {
        TotalCount+= count;

        float multipliedScore = totalMultiplier * count;
        switch (recyclingType)
        {
            case RecyclingType.Paper:
                PaperCount+= count;
                multipliedScore *= paperMultiplier;
                break;
            case RecyclingType.PlasticMetal:
                PlasticMetalCount += count;
                multipliedScore *= plasticMetalMultiplier;
                break;
            case RecyclingType.Glass:
                GlassCount += count;
                multipliedScore *= glassMultiplier;
                break;
            case RecyclingType.Organic:
                OrganicCount += count;
                multipliedScore *= organicMultiplier;
                break;
            case RecyclingType.ElectronicsBatteries:
                ElectronicsBatteriesCount += count;
                multipliedScore *= electronicsBatteriesMultiplier;
                break;
        }



        totalScore += multipliedScore;

        GameObject addScore = new GameObject();
        ScoreShow scoreComponent = addScore.AddComponent<ScoreShow>();
        scoreComponent.Initialize(multipliedScore, recyclingType, count > 1);



        SaveSystem.Instance.Flag(this);
        return multipliedScore;
    }
    //base
    [JsonIgnore]
    public long TotalCount
    {
        get => totalCount;
        set
        {
            totalCount = value;

            /*float newTotalScoreMultiplier = GetTotalMultiplier();
            if (newTotalScoreMultiplier != totalMultiplier)
            {
                //rank

                totalMultiplier = newTotalScoreMultiplier;
            }*/

            SaveSystem.Instance.Flag(this);
        }
    }
    

    [JsonIgnore]
    public float HighScore
    {
        get => highScore;
        set
        {
            highScore = value;

            SaveSystem.Instance.Flag(this);
        }
    }

    [JsonIgnore]
    public long PaperCount
    {
        get => paperCount;
        set
        {
            paperCount = value;

            /*int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != paperMultiplier)
            {
                //rank

                paperMultiplier = newMultiplier;
            }*/

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public long GlassCount
    {
        get => glassCount;
        set
        {
            glassCount = value;

            /*int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != glassMultiplier)
            {
                //rank

                glassMultiplier = newMultiplier;
            }*/

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public long PlasticMetalCount
    {
        get => plasticMetalCount;
        set
        {
            plasticMetalCount = value;

            /*int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != plasticMetalMultiplier)
            {
                //rank

                plasticMetalMultiplier = newMultiplier;
            }*/

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public long OrganicCount
    {
        get => organicCount;
        set
        {
            organicCount = value;

            /*int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != organicMultiplier)
            {
                //rank

                organicMultiplier = newMultiplier;
            }*/

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public long ElectronicsBatteriesCount
    {
        get => electronicsBatteriesCount;
        set
        {
            electronicsBatteriesCount = value;

            /*int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != electronicsBatteriesMultiplier)
            {
                //rank

                electronicsBatteriesMultiplier = newMultiplier;
            }*/

            SaveSystem.Instance.Flag(this);
        }
    }


    [JsonIgnore]
    public long CountChangeBins
    {
        get => countChangeBins;
        set
        {
            countChangeBins = value;

            SaveSystem.Instance.Flag(this);
        }
    }

    [JsonIgnore]
    public bool CatchTrashMidAir
    {
        get => catchTrashMidAir;
        set
        {
            if (!catchTrashMidAir && tutorialFinished)
            {
                catchTrashMidAir = value;
                SaveSystem.Instance.Flag(this);
            }

        }
    }
    [JsonIgnore]
    public bool TutorialFinished
    {
        get => tutorialFinished;
        set
        {
            tutorialFinished = value;
            if (tutorialFinished)
            {
                BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);//refreshuje scenu bez pomagaca i tutorijal spawna
            }
            SaveSystem.Instance.Flag(this);
        }
    }

    public void AddHashMapTrivia(string question)//added when answered correctly
    {
        if (!triviaHashset.Contains(question))
        {
            triviaHashset.Add(question);
            SaveSystem.Instance.Flag(this);
        }
    }
    public int GetTriviaCount()//added when answered correctly
    {
        return triviaHashset.Count;
    }

    //multipliers
    /*[JsonIgnore]
    public float TotalScoreMultiplier
    {
        get => totalMultiplier;
        set
        {
            totalMultiplier = value;
            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int PaperMultiplier
    {
        get => paperMultiplier;
        set
        {
            paperMultiplier = value;
            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int GlassMultiplier
    {
        get => glassMultiplier;
        set
        {
            glassMultiplier = value;
            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int PlasticMultiplier
    {
        get => plasticMultiplier;
        set
        {
            plasticMultiplier = value;
            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int OrganicMultiplier
    {
        get => organicMultiplier;
        set
        {
            organicMultiplier = value;
            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int ElectronicsBatteriesMultiplier
    {
        get => electronicsBatteriesMultiplier;
        set
        {
            electronicsBatteriesMultiplier = value;
            SaveSystem.Instance.Flag(this);
        }
    }*/

    [JsonIgnore]
    public static int[] totalMultiplierTresholds = {500,750,1000,1500,3000 };
    [JsonIgnore]
    public static int[] binMultiplierTresholds = { 100, 200, 400, 800, 1600 };
    [JsonIgnore]
    public static int[] changeBinsRankTresholds = { 50,100,200};
    public float GetTotalMultiplier()
    {
        if (totalCount < totalMultiplierTresholds[0])
            return 1;
        else if (totalCount < totalMultiplierTresholds[1])
            return 1.2f;
        else if (totalCount < totalMultiplierTresholds[2])
            return 1.4f;
        else if (totalCount < totalMultiplierTresholds[3])
            return 1.6f;
        else if (totalCount < totalMultiplierTresholds[4])
            return 1.8f;
        else
            return 2f;
    }
    public int GetBinMultiplier(long count)//== rank
    {
        if (count < binMultiplierTresholds[0])
            return 1;
        else if (count < binMultiplierTresholds[1])
            return 2;
        else if (count < binMultiplierTresholds[2])
            return 3;
        else if (count < binMultiplierTresholds[3])
            return 4;
        else if (count < binMultiplierTresholds[4])
            return 5;
        else
            return 6;
    }
    public int GetRankBin(long count)
    {
        return GetBinMultiplier(count);
    }
    public int GetRankCountChangeBins()
    {
        if (countChangeBins < changeBinsRankTresholds[0])
            return 1;
        else if (countChangeBins < changeBinsRankTresholds[1])
            return 4;
        else if (countChangeBins < changeBinsRankTresholds[2])
            return 5;
        else
            return 6;
    }
    public int GetRankTotalCount()
    {
        switch (GetTotalMultiplier())
        {
            case 1:
                return 1;
            case 1.2f:
                return 2;
            case 1.4f:
                return 3;
            case 1.6f:
                return 4;
            case 1.8f:
                return 5;
            case 2:
                return 6;
        }
        return -1;
    }
    public int GetRankTrivia()
    {
        if (GetTriviaCount() == Quiz.quizData.Count)
        {
            return 6;
        }
        return 1;
    }
    public int GetRankCatchMidAir()
    {
        if (catchTrashMidAir)
        {
            return 6;
        }
        return 1;
    }
    public long GetTotalScore()
    {
        return (long)totalScore;
    }
    public long GetHighScore()
    {
        return (long)highScore;
    }



}
