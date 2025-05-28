using Newtonsoft.Json;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerSave : GenericSave
{
    //base fields
    [JsonProperty]
    private float totalScore;

    //base fields
    [JsonProperty]
    private int totalCount;
    [JsonProperty]
    private int paperCount;
    [JsonProperty]
    private int glassCount;
    [JsonProperty]
    private int plasticMetalCount;
    [JsonProperty]
    private int organicCount;
    [JsonProperty]
    private int electronicsBatteriesCount;


    //multiplier
    [JsonProperty]
    private float totalMultiplier = 1;
    [JsonProperty]
    private int paperMultiplier = 1;
    [JsonProperty]
    private int glassMultiplier = 1;
    [JsonProperty]
    private int plasticMetalMultiplier = 1;
    [JsonProperty]
    private int organicMultiplier = 1;
    [JsonProperty]
    private int electronicsBatteriesMultiplier = 1;



    public float ScoreIncrease(RecyclingType recyclingType)
    {
        TotalCount++;

        float multiplier = totalMultiplier;
        switch (recyclingType)
        {
            case RecyclingType.Paper:
                PaperCount++;
                multiplier *= paperMultiplier;
                break;
            case RecyclingType.PlasticMetal:
                PlasticMetalCount++;
                multiplier *= plasticMetalMultiplier;
                break;
            case RecyclingType.Glass:
                GlassCount++;
                multiplier *= glassMultiplier;
                break;
            case RecyclingType.Organic:
                OrganicCount++;
                multiplier *= organicMultiplier;
                break;
            case RecyclingType.ElectronicsBatteries:
                ElectronicsBatteriesCount++;
                multiplier *= electronicsBatteriesMultiplier;
                break;
        }

        totalScore += multiplier;

        SaveSystem.Instance.Flag(this);
        return multiplier;
    }
    //base
    [JsonIgnore]
    public int TotalCount
    {
        get => totalCount;
        set
        {
            totalCount = value;

            float newTotalScoreMultiplier = GetTotalMultiplier();
            if (newTotalScoreMultiplier != totalMultiplier)
            {
                //rank

                totalMultiplier = newTotalScoreMultiplier;
            }

            SaveSystem.Instance.Flag(this);
        }
    }


    [JsonIgnore]
    public int PaperCount
    {
        get => paperCount;
        set
        {
            paperCount = value;

            int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != paperMultiplier)
            {
                //rank

                paperMultiplier = newMultiplier;
            }

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int GlassCount
    {
        get => glassCount;
        set
        {
            glassCount = value;

            int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != glassMultiplier)
            {
                //rank

                glassMultiplier = newMultiplier;
            }

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int PlasticMetalCount
    {
        get => plasticMetalCount;
        set
        {
            plasticMetalCount = value;

            int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != plasticMetalMultiplier)
            {
                //rank

                plasticMetalMultiplier = newMultiplier;
            }

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int OrganicCount
    {
        get => organicCount;
        set
        {
            organicCount = value;

            int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != organicMultiplier)
            {
                //rank

                organicMultiplier = newMultiplier;
            }

            SaveSystem.Instance.Flag(this);
        }
    }
    [JsonIgnore]
    public int ElectronicsBatteriesCount
    {
        get => electronicsBatteriesCount;
        set
        {
            electronicsBatteriesCount = value;

            int newMultiplier = GetBinMultiplier(value);
            if (newMultiplier != electronicsBatteriesMultiplier)
            {
                //rank

                electronicsBatteriesMultiplier = newMultiplier;
            }

            SaveSystem.Instance.Flag(this);
        }
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
    public int GetBinMultiplier(int count)
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
    public int GetTotalScore()
    {
        return (int)totalScore;
    }


}
